using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Features.Parameters.Models;

namespace CoreAppStructure.Features.Parameters.Interfaces
{
    public interface IParameterService
    {
        Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1);
        Task<ResponseObject> FindByIdAsync(int id);
        Task<ResponseObject> SaveAsync(ParameterViewModel model);
        Task<ResponseObject> UpdateAsync(int id, ParameterViewModel model);
        Task<ResponseObject> DeleteAsync(int id);
    }
}
