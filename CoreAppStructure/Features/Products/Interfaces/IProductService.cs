using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Features.Categories.Models;
using CoreAppStructure.Features.Products.Models;

namespace CoreAppStructure.Features.Products.Interfaces
{
    public interface IProductService
    {
        Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1);
        Task<ResponseObject> FindByIdAsync(int id);
        Task<ResponseObject> FindBySlugAsync(string slug);
        Task<ResponseObject> SaveAsync(ProductViewModel model, string request);
        Task<ResponseObject> UpdateAsync(int id, ProductViewModel model, string request);
        Task<ResponseObject> DeleteAsync(int id);
    }
}
