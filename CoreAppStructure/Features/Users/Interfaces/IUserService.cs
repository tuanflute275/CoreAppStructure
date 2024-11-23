using CoreAppStructure.Core.Exceptions;
using CoreAppStructure.Features.Users.Models;

namespace CoreAppStructure.Features.Users.Interfaces
{
    public interface IUserService
    {
        Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1);
        Task<ResponseObject> FindByIdAsync(int id);
        Task<ResponseObject> SaveAsync(UserViewModel model, string request);
        Task<ResponseObject> UpdateAsync(int id, UserViewModel model, string request);
        Task<ResponseObject> DeleteAsync(int id);
    }
}
