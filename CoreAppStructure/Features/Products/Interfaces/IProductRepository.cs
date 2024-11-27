namespace CoreAppStructure.Features.Products.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> FindAllAsync(string? name, string? sort);
        Task<Product> FindByIdAsync(int id);
        Task<Product> FindBySlugAsync(string slug);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);
    }
}
