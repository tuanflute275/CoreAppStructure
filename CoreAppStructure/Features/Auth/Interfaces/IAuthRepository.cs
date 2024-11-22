using CoreAppStructure.Data.Entities;
using CoreAppStructure.Features.Auth.Models;
using CoreAppStructure.Features.Roles.Models;
using CoreAppStructure.Features.Users.Models;

namespace CoreAppStructure.Features.Auth.Interfaces
{
    public interface IAuthRepository
    {
        Task AddAsync(User user);
        Task<User> FindByUsernameOrEmailAsync(string usernameOrEmail);
        Task<List<UserRoleDto>> GetUserRolesAsync(int userId);
        Task<List<UserRole>> FindUserRoleAsync(int userId);
        Task AddUserRoleAsync(UserRole userRole);
        Task DeleteUserRoleAsync(List<UserRole> userRoles);
        Task<Role> FindByNameAsync(string name);
    }
}
