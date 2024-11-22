using CoreAppStructure.Data.Entities;
using CoreAppStructure.Features.Users.Models;

namespace CoreAppStructure.Features.Users.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> FindAllAsync(string? name, string? sort);
        Task<User> FindByIdAsync(int id);
        Task<User> FindByUsernameAsync(string username);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);

        Task<List<UserRole>> FindUserRoleAsync(int userId);
        Task AddUserRoleAsync(UserRole userRole);
        Task DeleteUserRoleAsync(List<UserRole> userRoles);
    }
}
