using CoreAppStructure.Features.Auth.Models;
using CoreAppStructure.Features.Users.Models;

namespace CoreAppStructure.Features.Auth.Interfaces
{
    public interface IAuthRepository
    {
        Task RegisterAsync(User user);
        Task<User> FindByUsernameOrEmailAsync(string usernameOrEmail);
        Task<List<UserRoleDto>> GetUserRolesAsync(int userId);
    }
}
