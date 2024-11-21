using CoreAppStructure.Core.Extensions;
using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Features.Users.Interfaces;
using CoreAppStructure.Features.Users.Models;

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

        public Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1)
        {
            throw new NotImplementedException();
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
