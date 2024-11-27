namespace CoreAppStructure.Features.Auth.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserRole>> FindUserRoleAsync(int userId)
        {
            return await _context.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();
        }

        public async Task AddUserRoleAsync(UserRole userRole)
        {
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteUserRoleAsync(List<UserRole> userRoles)
        {
            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();
        }

        public async Task<User> FindByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.Users.FirstOrDefaultAsync(x =>
                        x.UserEmail == usernameOrEmail || x.UserName == usernameOrEmail);
        }

        public async Task<List<UserRoleDto>> GetUserRolesAsync(int userId)
        {
            return await (from ur in _context.UserRoles
                          join r in _context.Roles on ur.RoleId equals r.RoleId
                          where ur.UserId == userId
                          select new UserRoleDto
                          {
                              UserId = ur.UserId,
                              RoleName = r.RoleName
                          }).ToListAsync();
        }

        public async Task<RoleModel.Role> FindByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == name);
        }


        // token
        public async Task<Tokens> GetRefreshTokenAsync(string refreshToken)
        {
            return await _context.Tokens.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
        }

        public async Task SaveTokenAsync(Tokens token)
        {
            await _context.Tokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTokenAsync(int tokenId, string token, DateTime expirationDate, string refreshToken, DateTime refreshTokenDate)
        {
            var data = await _context.Tokens.FindAsync(tokenId);
            if (data != null)
            {
                data.Token = token;
                data.ExpirationDate = expirationDate;
                data.RefreshToken = refreshToken;
                data.RefreshTokenDate = refreshTokenDate;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Tokens>> GetUserTokensAsync(int userId)
        {
           return await _context.Tokens.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task DeleteTokenAsync(int tokenId)
        {
            var token = await _context.Tokens.FindAsync(tokenId);
            if (token != null)
            {
                _context.Tokens.Remove(token);
                await _context.SaveChangesAsync();
            }
        }
    }
}
