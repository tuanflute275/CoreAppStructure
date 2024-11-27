namespace CoreAppStructure.Features.Roles.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<RoleModel.Role>> FindAllAsync(string? name, string? sort)
        {
            var roles = _context.Roles.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                roles = roles.Where(x => x.RoleName.Contains(name));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        roles = roles.OrderBy(x => x.RoleId);
                        break;
                    case "Id-DESC":
                        roles = roles.OrderByDescending(x => x.RoleId);
                        break;
                    case "Name-ASC":
                        roles = roles.OrderBy(x => x.RoleName);
                        break;
                    case "Name-DESC":
                        roles = roles.OrderByDescending(x => x.RoleName);
                        break;
                }
            }

            return await roles.ToListAsync();
        }

        public async Task<List<RoleModel.Role>> FindListAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<RoleModel.Role> FindByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<RoleModel.Role> FindByNameAsync(string name)
        {
            return await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == name);
        }

        public async Task AddAsync(RoleModel.Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RoleModel.Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(RoleModel.Role role)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}
