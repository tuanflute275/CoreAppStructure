using CoreAppStructure.Features.Roles.Models;

namespace CoreAppStructure.Features.Roles.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> FindAllAsync(string? name, string? sort);
        Task<List<Role>> FindListAllAsync();
        Task<Role> FindByIdAsync(int id);
        Task<Role> FindByNameAsync(string name);
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Role role);
    }
}
