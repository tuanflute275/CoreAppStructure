using CoreAppStructure.Features.Users.Models;

namespace CoreAppStructure.Features.Users.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> FindAllAsync(string? name, string? sort);
        Task<User> FindByIdAsync(int id);
        Task AddAsync(User product);
        Task UpdateAsync(User product);
        Task DeleteAsync(User product);
    }
}
