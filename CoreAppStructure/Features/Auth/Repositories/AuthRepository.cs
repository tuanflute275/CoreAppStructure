using CoreAppStructure.Data;
using CoreAppStructure.Data.Entities;
using CoreAppStructure.Data.Models;
using CoreAppStructure.Features.Auth.Interfaces;
using CoreAppStructure.Features.Auth.Models;
using CoreAppStructure.Features.Roles.Models;
using CoreAppStructure.Features.Users.Models;
using Microsoft.EntityFrameworkCore;
using System;

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

        public async Task<Role> FindByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == name);
        }


        // refresh token
        public async Task SaveRefreshTokenAsync(int userId, string token, DateTime expiresAt)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked);
        }

        public async Task UpdateRefreshTokenAsync(int tokenId, string newToken, DateTime newExpiresAt)
        {
            var token = await _context.RefreshTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.Token = newToken;
                token.ExpiresAt = newExpiresAt;
                await _context.SaveChangesAsync();
            }
        }
    }
}
