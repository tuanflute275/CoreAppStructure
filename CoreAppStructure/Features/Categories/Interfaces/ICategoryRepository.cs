namespace CoreAppStructure.Features.Categories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> FindAllAsync(string? name, string? sort);
        Task<List<Category>> FindListAllAsync();
        Task<Category> FindByIdAsync(int id);
        Task<Category> FindBySlugAsync(string slug);
        Task<Category> FindByNameAsync(string name);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Category category);
        Task<List<Product>> GetProductsByCategoryIdAsync(int id);
        Task DeleteProducts(List<Product> products);
    }
}
