using Azure.Core;

namespace CoreAppStructure.Features.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository      _authRepository;
        private readonly IConfiguration       _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService        _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(
            IAuthRepository      authRepository,
            IConfiguration       configuration,
            ILogger<AuthService> logger, 
            IEmailService        emailService,
            IHttpContextAccessor httpContextAccessor) 
        {
            _authRepository      = authRepository;
            _configuration       = configuration;
            _logger              = logger;
            _emailService        = emailService;   
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseObject> LoginAsync(LoginViewModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.UsernameOrEmail))
                    return new ResponseObject(400, "Email or username must be provided.");
                if (string.IsNullOrEmpty(model.Password))
                    return new ResponseObject(400, "Password must be provided.");

                // kiểm tra tài khoản có tồn tại
                var user = await _authRepository.FindByUsernameOrEmailAsync(model.UsernameOrEmail);
                if (user == null)
                    return new ResponseObject(400, "Account does not exist.");

                // Kiểm tra trạng thái tài khoản
                var lockStatusResponse = CheckAccountLockStatus(user);
                if (lockStatusResponse != null) 
                    return lockStatusResponse;
                
                // Kiểm tra độ dài mật khẩu
                if (model.Password.Length < 6)
                {
                    user.FailedLoginAttempts++;
                    UpdateLockoutStatus(user); // Hàm cập nhật trạng thái khóa

                    await _authRepository.UpdateAsync(user);
                    LogHelper.LogError(_logger, null, "POST", $"/api/auth/login", "Password must be longer than 6 characters.");
                    return new ResponseObject(400, "Password must be longer than 6 characters.");
                }    

                // Kiểm tra mật khẩu có khớp dữ liệu không
                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.UserPassword))
                {
                    user.FailedLoginAttempts++; // tăng số lần đăng nhập fail
                    UpdateLockoutStatus(user); // Hàm cập nhật trạng thái khóa

                    await _authRepository.UpdateAsync(user);
                    LogHelper.LogError(_logger, null, "POST", $"/api/auth/login", "Incorrect password.");
                    return new ResponseObject(400, "Incorrect password.");
                }

                // Reset trạng thái nếu đã hết thời gian khóa
                ResetLockoutStatus(user);
                await _authRepository.UpdateAsync(user);

                // lấy ra quyền của tài khoản
                var roles = await _authRepository.GetUserRolesAsync(user.UserId);
                if (roles == null || !roles.Any())
                    return new ResponseObject(400, "You do not have access.");

                // generate token and refresh token
                var accessToken = TokenHelper.GenerateJwtToken(user.UserId, user.UserEmail, roles.Select(r => r.RoleName), _configuration);
                var refreshToken = TokenHelper.GenerateRefreshToken();

                //save token vào database
                await ManageTokens(user.UserId, accessToken, refreshToken);
                LogHelper.LogInformation(_logger, "POST", "/api/auth/login", null,new { AccessToken = accessToken, RefreshToken = refreshToken });
                return new ResponseObject(200, "Login successfully", new { AccessToken = accessToken, RefreshToken = refreshToken });
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "POST", $"/api/auth/login");
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        private ResponseObject CheckAccountLockStatus(User user)
        {
            if (user.UserActive == 2)
            {
                return new ResponseObject(403, "Your account has been permanently banned.");
            }
            if (user.UserActive == 1)
            {
                if (user.UserUnlockTime.HasValue && DateTime.Now < user.UserUnlockTime.Value)
                {
                    var remainingTime = user.UserUnlockTime.Value - DateTime.Now;
                    int remainingMinutes = (int)remainingTime.TotalMinutes;
                    int remainingSeconds = (int)remainingTime.Seconds;
                    if (remainingMinutes > 0)
                    {
                        return new ResponseObject(403, $"Your account is temporarily locked. Try again in {remainingMinutes} minute(s) and {remainingSeconds} second(s).");
                    }
                    else
                    {
                        return new ResponseObject(403, $"Your account is temporarily locked. Try again in {remainingSeconds} second(s).");
                    }
                }
            }
            return null; // Trả về null nếu tài khoản không bị khóa
        }

        private void UpdateLockoutStatus(User user)
        {
            double lockDurationInMinutes;
            if (user.FailedLoginAttempts == 3)
            {
                lockDurationInMinutes = 0.5;  // 30 giây
            }
            else if (user.FailedLoginAttempts == 5)
            {
                lockDurationInMinutes = 1;  // 1 phút
            }
            else if (user.FailedLoginAttempts == 6)
            {
                lockDurationInMinutes = 2;  // 2 phút
            }
            else if (user.FailedLoginAttempts == 7)
            {
                lockDurationInMinutes = 3;  // 3 phút
            }
            else if (user.FailedLoginAttempts == 8)
            {
                lockDurationInMinutes = 4;  // 4 phút
            }
            else if (user.FailedLoginAttempts >= 9)
            {
                lockDurationInMinutes = 5;  // 5 phút
            }
            else
            {
                return;
            }

            LockUser(user, lockDurationInMinutes);
        }

        private void LockUser(User user, double minutes)
        {
            user.UserActive = 1;
            user.UserCurrentTime = DateTime.Now;
            user.UserUnlockTime = DateTime.Now.AddMinutes(minutes);
        }

        private void ResetLockoutStatus(User user)
        {
            user.FailedLoginAttempts = 0;
            user.UserActive = 0;
            user.UserUnlockTime = null;
            user.UserCurrentTime = null;
        }

        private async Task ManageTokens(int userId, string accessToken, string refreshToken)
        {
            var userTokens = await _authRepository.GetUserTokensAsync(userId);
            if (userTokens.Count >= 3)
            {
                var tokenToDelete = userTokens.OrderBy(t => t.IsMobile).ThenBy(t => t.CreatedAt).First();
                await _authRepository.DeleteTokenAsync(tokenToDelete.Id);
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            string userAgent = request.Headers["User-Agent"].FirstOrDefault();
            bool isMobile = Util.IsMobileDevice(userAgent);

            Tokens token = new Tokens
            {
                UserId = userId,
                IsMobile = isMobile,
                IsRevoked = false,
                TokenType = "AccessToken",
                Token = accessToken,
                ExpirationDate = DateTime.UtcNow.AddHours(1),
                RefreshToken = refreshToken,
                RefreshTokenDate = DateTime.UtcNow.AddDays(15),
                CreatedAt = DateTime.UtcNow
            };
            await _authRepository.SaveTokenAsync(token);
        }


        public async Task<ResponseObject> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                 var storedToken = await _authRepository.GetRefreshTokenAsync(refreshToken);

                 if (storedToken == null || storedToken.IsRevoked || storedToken.RefreshTokenDate <= DateTime.UtcNow)
                     return new ResponseObject(400, "Invalid or expired refresh token.");

                 // Generate new Access Token
                 var user = await _authRepository.GetUserByIdAsync(storedToken.UserId);
                 var roles = await _authRepository.GetUserRolesAsync(user.UserId);

                 var newAccessToken = TokenHelper.GenerateJwtToken(
                       user.UserId,
                       user.UserEmail,
                       roles.Select(r => r.RoleName),
                       _configuration
                   );

                 // Optionally: Generate a new Refresh Token
                 var newRefreshToken = TokenHelper.GenerateRefreshToken();
                 await _authRepository.UpdateTokenAsync(storedToken.Id, newAccessToken, DateTime.UtcNow.AddHours(15),  newRefreshToken, DateTime.UtcNow.AddDays(15));

                 return new ResponseObject(200, "Token refreshed successfully", new
                 {
                     AccessToken = newAccessToken,
                     RefreshToken = newRefreshToken
                 });
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "POST", "/api/auth/refresh-token");
                return new ResponseObject(500, "Internal server error. Please try again later.");
            }
        }

        public async Task<ResponseObject> RegisterAsync(RegisterViewModel model)
        {
            if (model == null)
                return new ResponseObject(400, "Invalid request.");
            
            if (model.Password.Length < 6)
                return new ResponseObject(400, "Password must be longer than 6 characters !");
            
            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, 12);
                User user           = new User
                {
                    UserName     = model.UserName,
                    UserFullName = model.FullName,
                    UserEmail    = model.Email,
                    UserPassword = passwordHash
                };

                await _authRepository.AddAsync(user);

                // Lấy userId vừa tạo
                var userId = user.UserId;
                List<string> listRole = new List<string>();
                listRole.Add("User");

                // Lặp qua danh sách các vai trò và thêm vào bảng UserRole
                foreach (var roleName in listRole) 
                {
                    var role = await _authRepository.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        var userRole = new UserRole
                        {
                            UserId = userId,
                            RoleId = role.RoleId
                        };

                        _authRepository.AddUserRoleAsync(userRole);
                    }
                }

                // Gửi email xác nhận đăng ký
                LogHelper.LogInformation(_logger, "GET", "/api/auth/register", null, null);
                await _emailService.SendEmailAsync(model.Email, "Welcome to Our Service", BodyRegisterMail(model.FullName));
                return new ResponseObject(200, "Register successfully,please check email!", model);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "POST", $"/api/auth/register");
                return new ResponseObject(500, "Internal server error. Please try again later.");
            }
        }

        private string BodyRegisterMail(string fullName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Infrastructure\\Email\\Templates", "RegisterSuccessMail.cshtml");
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Template file not found at: {path}");
            }

            string body = File.ReadAllText(path);
            return body.Replace("{{fullName}}", fullName);
        }

        public async Task<ResponseObject> GoogleCallbackAsync(HttpContext httpContext)
        {
            var authenticateResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return new ResponseObject(400, "Google authentication failed", null);
            }

            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (email == null || name == null)
            {
                return new ResponseObject(404, "Required user information not found", new
                {
                    Name = name,
                    Email = email
                });
            }
            var refreshToken = TokenHelper.GenerateRefreshToken();
            return new ResponseObject(200, "Login successfully", new
            {
                Name = name,
                Email = email,
                AccessToken = 1,
                RefreshToken = refreshToken
            });
        }

        public async Task<ResponseObject> FacebookCallbackAsync(HttpContext httpContext)
        {
            try
            {
                var authenticateResult = await httpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (!authenticateResult.Succeeded)
                {
                    return new ResponseObject(400, "Facebook authentication failed", null);
                }

                var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
                var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                if (email == null || name == null)
                {
                    return new ResponseObject(404, "Required user information not found", new
                    {
                        Name = name,
                        Email = email
                    });
                }

                // Lưu user vào database nếu cần
                var userCheck = await _authRepository.FindByEmailAsync(email);
                string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456", 12);
                if (userCheck == null)
                {
                    // Tạo tài khoản mới
                    User user = new User
                    {
                        UserName = name,
                        UserFullName = name,
                        UserEmail = email,
                        UserPassword = passwordHash
                    };

                    await _authRepository.AddAsync(user);

                    // Lấy userId vừa tạo
                    var userId = user.UserId;
                    var userRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = 2
                    };

                    _authRepository.AddUserRoleAsync(userRole);

                    // Đăng nhập và tạo token
                    List<UserRoleDto> roles = new List<UserRoleDto>();
                    UserRoleDto dto = new UserRoleDto
                    {
                        UserId = user.UserId,
                        RoleName = "User"
                    };
                    roles.Add(dto);
                    var accessToken = TokenHelper.GenerateJwtToken(
                             user.UserId,
                             email,
                             roles.Select(r => r.RoleName),
                             _configuration
                         );
                    var refreshToken = TokenHelper.GenerateRefreshToken();

                    // check if >=3 then delete old token with priority to delete web token first
                    var userTokens = await _authRepository.GetUserTokensAsync(user.UserId);
                    if (userTokens.Count >= 3)
                    {
                        var tokenToDelete = userTokens
                            .OrderBy(t => t.IsMobile)
                            .ThenBy(t => t.CreatedAt)
                            .First();

                        await _authRepository.DeleteTokenAsync(tokenToDelete.Id);
                    }

                    // save new token
                    // Lấy User-Agent từ request header
                    var request = _httpContextAccessor.HttpContext?.Request;
                    string userAgent = request.Headers["User-Agent"].FirstOrDefault();
                    bool isMobile = Util.IsMobileDevice(userAgent);
                    Tokens token = new Tokens
                    {
                        UserId = user.UserId,
                        IsMobile = isMobile,
                        IsRevoked = false,
                        TokenType = "Oauth2",
                        Token = accessToken,
                        ExpirationDate = DateTime.UtcNow.AddHours(1),
                        RefreshToken = refreshToken,
                        RefreshTokenDate = DateTime.UtcNow.AddDays(15),
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _authRepository.SaveTokenAsync(token);
                    LogHelper.LogInformation(_logger, "POST", "/api/auth/signin-facebook", null, new
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    });
                    return new ResponseObject(200, "Login successfully", new
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    });
                }

                // đã tồn tại tài koanr thì đăng nhập luôn
                List<UserRoleDto> roles2 = new List<UserRoleDto>();
                UserRoleDto dto2 = new UserRoleDto
                {
                    UserId = userCheck.UserId,
                    RoleName = "User"
                };
                roles2.Add(dto2);
                var accessToken2 = TokenHelper.GenerateJwtToken(
                     userCheck.UserId,
                     userCheck.UserEmail,
                     roles2.Select(r => r.RoleName),
                     _configuration
                 );
                var refreshToken2 = TokenHelper.GenerateRefreshToken();
                var userTokens2 = await _authRepository.GetUserTokensAsync(userCheck.UserId);
                if (userTokens2.Count >= 3)
                {
                    var tokenToDelete2 = userTokens2
                        .OrderBy(t => t.IsMobile)
                        .ThenBy(t => t.CreatedAt)
                        .First();

                    await _authRepository.DeleteTokenAsync(tokenToDelete2.Id);
                }

                var request2 = _httpContextAccessor.HttpContext?.Request;
                string userAgent2 = request2.Headers["User-Agent"].FirstOrDefault();
                bool isMobile2 = Util.IsMobileDevice(userAgent2);
                Tokens token2 = new Tokens
                {
                    UserId = userCheck.UserId,
                    IsMobile = isMobile2,
                    IsRevoked = false,
                    TokenType = "Oauth2",
                    Token = accessToken2,
                    ExpirationDate = DateTime.UtcNow.AddHours(1),
                    RefreshToken = refreshToken2,
                    RefreshTokenDate = DateTime.UtcNow.AddDays(15),
                    CreatedAt = DateTime.UtcNow,
                };
                await _authRepository.SaveTokenAsync(token2);
                LogHelper.LogInformation(_logger, "POST", "/api/auth/signin-facebook", null, new
                {
                    AccessToken = accessToken2,
                    RefreshToken = refreshToken2
                });
                return new ResponseObject(200, "Login successfully", new
                {
                    AccessToken = accessToken2,
                    RefreshToken = refreshToken2
                });
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "POST", $"/api/auth/signin-facebook");
                return new ResponseObject(500, "Internal server error. Please try again later.");
            }
        }
    }
}
