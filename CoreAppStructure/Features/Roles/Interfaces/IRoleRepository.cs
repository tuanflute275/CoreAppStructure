namespace CoreAppStructure.Features.Roles.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<RoleModel.Role>> FindAllAsync(string? name, string? sort);
        Task<List<RoleModel.Role>> FindListAllAsync();
        Task<RoleModel.Role> FindByIdAsync(int id);
        Task<RoleModel.Role> FindByNameAsync(string name);
        Task AddAsync(RoleModel.Role role);
        Task UpdateAsync(RoleModel.Role role);
        Task DeleteAsync(RoleModel.Role role);
    }
}
