using CoreAppStructure.Data.Entities;
using CoreAppStructure.Data.Models;
using CoreAppStructure.Features.Auth.Models;
using CoreAppStructure.Features.Roles.Models;
using CoreAppStructure.Features.Users.Models;

namespace CoreAppStructure.Features.Auth.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task AddAsync(User user);
        Task<User> FindByUsernameOrEmailAsync(string usernameOrEmail);
        Task<List<UserRoleDto>> GetUserRolesAsync(int userId);
        Task<List<UserRole>> FindUserRoleAsync(int userId);
        Task AddUserRoleAsync(UserRole userRole);
        Task DeleteUserRoleAsync(List<UserRole> userRoles);
        Task<Role> FindByNameAsync(string name);

        // token
        Task<List<Tokens>> GetUserTokensAsync(int userId);
        Task<Tokens> GetRefreshTokenAsync(string refreshToken);
        Task SaveTokenAsync(Tokens token);
        Task UpdateTokenAsync(int tokenId, string token, DateTime expirationDate, string refreshToken, DateTime refreshTokenDate);
        Task DeleteTokenAsync(int tokenId); 
    }
}
