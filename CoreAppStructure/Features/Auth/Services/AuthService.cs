using CoreAppStructure.Core.Extensions;
using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Data.Entities;
using CoreAppStructure.Features.Auth.Interfaces;
using CoreAppStructure.Features.Auth.Models;
using CoreAppStructure.Features.Users.Models;
using CoreAppStructure.Infrastructure.Email;

namespace CoreAppStructure.Features.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IEmailService _emailService;
        public AuthService(IAuthRepository authRepository, IConfiguration configuration,
            ILogger<AuthService> logger, IEmailService emailService) 
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _logger = logger;
            _emailService = emailService;   
        }

        public async Task<ResponseObject> LoginAsync(LoginViewModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.UsernameOrEmail))
                    return new ResponseObject(400, "Email or username must be provided.");

                var user = await _authRepository.FindByUsernameOrEmailAsync(model.UsernameOrEmail);
                if (user == null)
                    return new ResponseObject(400, "Account does not exist.");

                if (string.IsNullOrEmpty(model.Password) || model.Password.Length < 6)
                    return new ResponseObject(400, "Password must be longer than 6 characters.");

                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.UserPassword))
                    return new ResponseObject(400, "Incorrect password.");

                var roles = await _authRepository.GetUserRolesAsync(user.UserId);
                if (roles == null || !roles.Any())
                    return new ResponseObject(400, "You do not have access.");


                var token = JwtHelper.GenerateJwtToken(
                      user.UserId,
                      user.UserEmail,
                      roles.Select(r => r.RoleName),
                      _configuration
                  );

                LogHelper.LogInformation(_logger, "POST", "/api/auth/login", null, token);
                return new ResponseObject(200, "Login successfully", token);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "POST", $"/api/auth/login");
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> RegisterAsync(RegisterViewModel model)
        {
            if (model == null)
            {
                return new ResponseObject(400, "Invalid request.");
            }
           
            if (model.Password.Length < 6)
            {
                return new ResponseObject(400, "Password must be longer than 6 characters !");
            }
            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, 12);
                User user = new User
                {
                    UserName = model.UserName,
                    UserFullName = model.FullName,
                    UserEmail = model.Email,
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
                await _emailService.SendEmailAsync(model.Email, "Welcome to Our Service", BodyRegisterMail(model.FullName));
                return new ResponseObject(200, "Register successfully,please check email!", model);
            }
            catch (Exception ex)
            {
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

    }
}
