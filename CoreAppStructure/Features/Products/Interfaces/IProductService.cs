using CoreAppStructure.Core.Exceptions;
using CoreAppStructure.Features.Products.Models;

namespace CoreAppStructure.Features.Products.Interfaces
{
    public interface IProductService
    {
        Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1);
        Task<ResponseObject> FindByIdAsync(int id);
        Task<ResponseObject> FindBySlugAsync(string slug);
        Task<ResponseObject> SaveAsync(ProductViewModel model, HttpRequest request);
        Task<ResponseObject> UpdateAsync(int id, ProductViewModel model, HttpRequest request);
        Task<ResponseObject> DeleteAsync(int id);
    }
}
