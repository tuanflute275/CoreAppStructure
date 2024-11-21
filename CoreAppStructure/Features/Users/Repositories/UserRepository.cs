﻿using CoreAppStructure.Data;
using CoreAppStructure.Features.Products.Models;
using CoreAppStructure.Features.Users.Interfaces;
using CoreAppStructure.Features.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreAppStructure.Features.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> FindAllAsync(string? name, string? sort)
        {
            var users = _context.Users.Include(x => x.UserRoles).ThenInclude(u => u.Role).AsQueryable();
             

            if (!string.IsNullOrEmpty(name))
            {
                users = users.Where(x => x.UserName.Contains(name));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        users = users.OrderBy(x => x.UserId);
                        break;
                    case "Id-DESC":
                        users = users.OrderByDescending(x => x.UserId);
                        break;
                    case "Name-ASC":
                        users = users.OrderBy(x => x.UserName);
                        break;
                    case "Name-DESC":
                        users = users.OrderByDescending(x => x.UserName);
                        break;
                    case "Email-ASC":
                        users = users.OrderBy(x => x.UserEmail);
                        break;
                    case "Email-DESC":
                        users = users.OrderByDescending(x => x.UserEmail);
                        break;
                }
            }

            return await users.ToListAsync();
        }

        public async Task<User> FindByIdAsync(int id)
        {
            return await _context.Users.Include(x => x.UserRoles)
                .ThenInclude(u => u.Role).FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}