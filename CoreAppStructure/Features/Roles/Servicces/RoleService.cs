using CoreAppStructure.Core.Extensions;
using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Features.Categories.Interfaces;
using CoreAppStructure.Features.Categories.Models;
using CoreAppStructure.Features.Categories.Repositories;
using CoreAppStructure.Features.Categories.Services;
using CoreAppStructure.Features.Roles.Interfaces;
using CoreAppStructure.Features.Roles.Models;
using X.PagedList;

namespace CoreAppStructure.Features.Roles.Servicces
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<CategoryService> _logger;

        public RoleService(IRoleRepository roleRepository, ILogger<CategoryService> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1)
        {
            try
            {
                var roles = await _roleRepository.FindAllAsync(name, sort);

                if (roles.Count > 0)
                {
                    int totalRecords = roles.Count();
                    int limit = 10;
                    page = page <= 1 ? 1 : page;
                    var pageData = roles.ToPagedList(page, limit);

                    int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                    var response = new
                    {
                        TotalRecords = totalRecords,
                        TotalPages = totalPages,
                        Data = pageData
                    };

                    LogHelper.LogInformation(_logger, "GET", "/api/role", null, response);
                    return new ResponseObject(200, "Query data successfully", response);
                }
                LogHelper.LogInformation(_logger, "GET", "/api/role", null, roles);
                return new ResponseObject(200, "Query data successfully", roles);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/category");
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public async Task<ResponseObject> FindListAllAsync()
        {
            try
            {
                var roles = await _roleRepository.FindListAllAsync();
                LogHelper.LogInformation(_logger, "GET", "/api/role/all", null, roles);
                return new ResponseObject(200, "Query data successfully", roles);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/role");
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public async Task<ResponseObject> FindByIdAsync(int id)
        {
            try
            {
                var role = await _roleRepository.FindByIdAsync(id);
                if (role == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }
                LogHelper.LogInformation(_logger, "GET", "/api/role/{id}", id, role);
                return new ResponseObject(200, "Query data successfully", role);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/role/{id}", id);
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public async Task<ResponseObject> SaveAsync(RoleViewModel model)
        {
            try
            {
                var existingRole = await _roleRepository.FindByNameAsync(model.RoleName);
                if (existingRole != null)
                {
                    return new ResponseObject(400, "Role name already taken");
                }

                var role = new Role
                {
                    RoleName = model.RoleName,
                    RoleDescription = model.RoleDescription,
                    CreateBy = "Admin",
                    CreateDate = DateTime.Now,
                };

                await _roleRepository.AddAsync(role);
                LogHelper.LogInformation(_logger, "POST", "/api/role", model, role);
                return new ResponseObject(200, "Insert data successfully", role);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "POST", "/api/role", model);
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public async Task<ResponseObject> UpdateAsync(int id, RoleViewModel model)
        {
            try
            {
                var role = await _roleRepository.FindByIdAsync(id);
                if (role == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }
                role.RoleName = model.RoleName;
                role.RoleDescription = model.RoleDescription;
                role.UpdateBy = "Admin";
                role.UpdateDate = DateTime.Now;
                await _roleRepository.UpdateAsync(role);
                LogHelper.LogInformation(_logger, "PUT", $"/api/role/{id}", model, role);
                return new ResponseObject(200, "Update data successfully", role);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "PUT", $"/api/role/{id}", model);
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public async Task<ResponseObject> DeleteAsync(int id)
        {
            try
            {
                var role = await _roleRepository.FindByIdAsync(id);
                if (role == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }
                await _roleRepository.DeleteAsync(role);
                LogHelper.LogInformation(_logger, "DELETE", $"/api/role/{id}", id, "Deleted successfully");
                return new ResponseObject(200, "Delete data successfully", null);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "DELETE", $"/api/role/{id}", id);
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }
    }
}
