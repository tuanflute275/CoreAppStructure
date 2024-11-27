namespace CoreAppStructure.Features.Parameters.Interfaces
{
    public interface IParameterRepository
    {
        Task<List<Parameter>> FindAllAsync(string? name, string? sort);
        Task<Parameter> FindByIdAsync(int id);
        Task<Parameter> FindByNameAsync(string name);
        Task AddAsync(Parameter parameter);
        Task UpdateAsync(Parameter parameter);
        Task DeleteAsync(Parameter parameter);
    }
}
