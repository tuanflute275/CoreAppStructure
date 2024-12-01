namespace CoreAppStructure.Features.Auth.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserByIdAsync(int id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByUsernameOrEmailAsync(string usernameOrEmail);
        Task<List<UserRoleDto>> GetUserRolesAsync(int userId);
        Task<List<UserRole>> FindUserRoleAsync(int userId);
        Task AddUserRoleAsync(UserRole userRole);
        Task DeleteUserRoleAsync(List<UserRole> userRoles);
        Task<RoleModel.Role> FindByNameAsync(string name);

        // token
        Task<List<DataModel.Tokens>> GetUserTokensAsync(int userId);
        Task<DataModel.Tokens> GetRefreshTokenAsync(string refreshToken);
        Task SaveTokenAsync(DataModel.Tokens token);
        Task UpdateTokenAsync(int tokenId, string token, DateTime expirationDate, string refreshToken, DateTime refreshTokenDate);
        Task DeleteTokenAsync(int tokenId); 
    }
}
