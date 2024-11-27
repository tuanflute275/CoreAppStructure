namespace CoreAppStructure.Features.Categories.Interfaces
{
    public interface ICategoryService
    {
        Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1);
        Task<ResponseObject> FindListAllAsync();
        Task<ResponseObject> FindByIdAsync(int id);
        Task<ResponseObject> FindBySlugAsync(string slug);
        Task<ResponseObject> SaveAsync(CategoryViewModel model);
        Task<ResponseObject> UpdateAsync(int id, CategoryViewModel model);
        Task<ResponseObject> DeleteAsync(int id);
    }
}
