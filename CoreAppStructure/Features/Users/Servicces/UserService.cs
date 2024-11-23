using AutoMapper;
using CoreAppStructure.Core.Exceptions;
using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Data.Entities;
using CoreAppStructure.Features.Roles.Interfaces;
using CoreAppStructure.Features.Users.Interfaces;
using CoreAppStructure.Features.Users.Models;
using CoreAppStructure.Infrastructure.Logging;
using X.PagedList;

namespace CoreAppStructure.Features.Users.Servicces
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly ILogger<UserService> _logger;

        public UserService(IMapper mapper,IUserRepository userRepository,
           Microsoft.AspNetCore.Hosting.IHostingEnvironment environment,
            ILogger<UserService> logger, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _environment = environment;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1)
        {
            try
            {
                var users = await _userRepository.FindAllAsync(name, sort);
                if (users.Count > 0)
                {
                    int totalRecords = users.Count();
                    int limit = 10;
                    page = page <= 1 ? 1 : page;
                    var pageData = users.ToPagedList(page, limit);
                    int totalPages = (int)Math.Ceiling((double)totalRecords / limit);
                    var userDTOs = _mapper.Map<List<UserDTO>>(pageData);

                    var response = new
                    {
                        TotalRecords = totalRecords,
                        TotalPages = totalPages,
                        Data = userDTOs
                    };
                   
                    LogHelper.LogInformation(_logger, "GET", "/api/user", null, response);
                    return new ResponseObject(200, "Query data successfully", response);
                }
                LogHelper.LogInformation(_logger, "GET", "/api/user", null, users);
                return new ResponseObject(200, "Query data successfully", users);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/user");
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> FindByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.FindByIdAsync(id);
                if (user == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }
                var userDTO = _mapper.Map<UserDTO>(user);
                LogHelper.LogInformation(_logger, "GET", "/api/user/{id}", id, userDTO);
                return new ResponseObject(200, "Query data successfully", userDTO);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/user/{id}", id);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> SaveAsync(UserViewModel model, HttpRequest request)
        {
            try
            {
                User user = new User();
                var foundData = await _userRepository.FindByUsernameAsync(model.UserName);
                if (foundData != null)
                {
                    return new ResponseObject(400, "Username name already taken");
                }
                else
                {
                    var imageUrl = await FileUploadHelper.UploadImageAsync(model.ImageFile, model.OldImage,request.Scheme, request.Host.Value, "users");
                    user.UserAvatar = imageUrl;
                    string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.UserPassword, 12);
                    user.UserName = model.UserName;
                    user.UserFullName = model.UserName;
                    user.UserEmail = model.UserEmail;
                    user.UserPassword = passwordHash;
                    user.UserPhoneNumber = model.UserPhoneNumber;
                    user.UserAddress = model.UserAddress;
                    user.UserGender = model.UserGender;
                    user.UserActive = model.UserActive;
                    user.PlaceOfBirth = model.PlaceOfBirth;
                    user.DateOfBirth = model.DateOfBirth;
                    user.Nationality = model.Nationality;
                    user.UserBio = model.UserBio;
                    user.SocialLinks = model.SocialLinks;
                    await _userRepository.AddAsync(user);

                    // Lấy userId vừa tạo
                    var userId = user.UserId;

                    // Lặp qua danh sách các vai trò và thêm vào bảng UserRole
                    foreach (var roleName in model.Roles)  // Giả sử model.Roles là một List<string>
                    {
                        var role = await _userRepository.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            var userRole = new UserRole
                            {
                                UserId = userId,
                                RoleId = role.RoleId
                            };

                            _userRepository.AddUserRoleAsync(userRole);
                        }
                    }
                    LogHelper.LogInformation(_logger, "POST", "/api/user", model, user);
                    return new ResponseObject(200, "Insert data successfully", model);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "POST", "/api/user", model);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> UpdateAsync(int id, UserViewModel model, HttpRequest request)
        {
            try
            {
                var user = await _userRepository.FindByIdAsync(id);
                if (user != null)
                {
                    var imageUrl = await FileUploadHelper.UploadImageAsync(model.ImageFile, model.OldImage, request.Scheme, request.Host.Value, "users");
                    user.UserAvatar = imageUrl;

                    if (!string.IsNullOrEmpty(model.UserPassword))
                    {
                        user.UserPassword = BCrypt.Net.BCrypt.HashPassword(model.UserPassword, 12);
                    }
                    user.UserName = model.UserName;
                    user.UserFullName = model.UserName;
                    user.UserEmail = model.UserEmail;
                    user.UserPhoneNumber = model.UserPhoneNumber;
                    user.UserAddress = model.UserAddress;
                    user.UserGender = model.UserGender;
                    user.UserActive = model.UserActive;
                    user.PlaceOfBirth = model.PlaceOfBirth;
                    user.DateOfBirth = model.DateOfBirth;
                    user.Nationality = model.Nationality;
                    user.UserBio = model.UserBio;
                    user.SocialLinks = model.SocialLinks;
                    await _userRepository.UpdateAsync(user);

                    // Lấy userId vừa cập nhật
                    var userId = user.UserId;

                    // Xóa tất cả các vai trò cũ của người dùng
                    var oldRoles = await _userRepository.FindUserRoleAsync(userId);
                    if (oldRoles != null && oldRoles.Count > 0)
                    {
                        // Xóa quyền cũ
                        await _userRepository.DeleteUserRoleAsync(oldRoles);
                    }

                    // Lặp qua danh sách các vai trò và thêm vào bảng UserRole
                    foreach (var roleName in model.Roles)
                    {
                        var role = await _userRepository.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            var userRole = new UserRole
                            {
                                UserId = userId,
                                RoleId = role.RoleId
                            };

                            // Thêm quyền mới
                            await _userRepository.AddUserRoleAsync(userRole);
                        }
                    }

                    LogHelper.LogInformation(_logger, "PUT", $"/api/user/{id}", model);
                    return new ResponseObject(200, "Update data successfully", model);
                }
                else
                {
                    LogHelper.LogInformation(_logger, "PUT", $"/api/user/{id}", model);
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "PUT", $"/api/user/{id}", model);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }
        public async Task<ResponseObject> DeleteAsync(int id)
        {
            try
            {
                var user = await _userRepository.FindByIdAsync(id);
                if (user == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }

                await _userRepository.DeleteAsync(user);
                LogHelper.LogInformation(_logger, "DELETE", $"/api/user/{id}", id, "Deleted successfully");
                return new ResponseObject(200, "Delete data successfully");
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "DELETE", $"/api/user/{id}", id);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }
    }
}
