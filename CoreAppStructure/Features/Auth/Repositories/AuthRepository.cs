using CoreAppStructure.Data;
using CoreAppStructure.Features.Auth.Interfaces;
using CoreAppStructure.Features.Auth.Models;
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

        public async Task RegisterAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
    }
}
