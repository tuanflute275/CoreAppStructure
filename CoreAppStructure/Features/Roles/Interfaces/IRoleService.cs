namespace CoreAppStructure.Features.Roles.Interfaces
{
    public interface IRoleService
    {
        Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1);
        Task<ResponseObject> FindListAllAsync();
        Task<ResponseObject> FindByIdAsync(int id);
        Task<ResponseObject> SaveAsync(RoleViewModel model);
        Task<ResponseObject> UpdateAsync(int id, RoleViewModel model);
        Task<ResponseObject> DeleteAsync(int id);
    }
}
