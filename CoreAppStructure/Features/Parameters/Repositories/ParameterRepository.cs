using CoreAppStructure.Data;
using CoreAppStructure.Features.Parameters.Interfaces;
using CoreAppStructure.Features.Parameters.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreAppStructure.Features.Parameters.Repositories
{
    public class ParameterRepository : IParameterRepository
    {
        private readonly ApplicationDbContext _context;

        public ParameterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Parameter>> FindAllAsync(string? name, string? sort)
        {
            var roles = _context.Parameters.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                roles = roles.Where(x => x.ParaName.Contains(name));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        roles = roles.OrderBy(x => x.ParaId);
                        break;
                    case "Id-DESC":
                        roles = roles.OrderByDescending(x => x.ParaId);
                        break;
                    case "Name-ASC":
                        roles = roles.OrderBy(x => x.ParaName);
                        break;
                    case "Name-DESC":
                        roles = roles.OrderByDescending(x => x.ParaName);
                        break;
                }
            }

            return await roles.ToListAsync();
        }

        public async Task<Parameter> FindByIdAsync(int id)
        {
            return await _context.Parameters.FindAsync(id);
        }

        public async Task<Parameter> FindByNameAsync(string name)
        {
            return await _context.Parameters.FirstOrDefaultAsync(x => x.ParaName == name);
        }

        public async Task AddAsync(Parameter parameter)
        {
            await _context.Parameters.AddAsync(parameter);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Parameter parameter)
        {
            _context.Parameters.Update(parameter);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Parameter parameter)
        {
            _context.Parameters.Remove(parameter);
            await _context.SaveChangesAsync();
        }
    }
}
