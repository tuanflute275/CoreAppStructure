using CoreAppStructure.Core.Extensions;
using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Features.Products.Models;
using CoreAppStructure.Features.Users.Interfaces;
using CoreAppStructure.Features.Users.Models;
using CoreAppStructure.Infrastructure.Caching;
using Newtonsoft.Json;
using X.PagedList;

namespace CoreAppStructure.Features.Users.Servicces
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository,
           Microsoft.AspNetCore.Hosting.IHostingEnvironment environment,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
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

                    //var userDTOs = null;

                    var response = new
                    {
                        TotalRecords = totalRecords,
                        TotalPages = totalPages,
                        //Data = userDTOs
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
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public Task<ResponseObject> FindByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseObject> SaveAsync(UserViewModel model, string request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseObject> UpdateAsync(int id, UserViewModel model, string request)
        {
            throw new NotImplementedException();
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
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }
    }
}
