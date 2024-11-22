using CoreAppStructure.Core.Extensions;
using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Features.Categories.Interfaces;
using CoreAppStructure.Features.Categories.Models;
using X.PagedList;

namespace CoreAppStructure.Features.Categories.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1)
        {
            try
            {
                var categories = await _categoryRepository.FindAllAsync(name, sort);

                if (categories.Count > 0)
                {
                    int totalRecords = categories.Count();
                    int limit = 10;
                    page = page <= 1 ? 1 : page;
                    var pageData = categories.ToPagedList(page, limit);

                    int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                    var response = new
                    {
                        TotalRecords = totalRecords,
                        TotalPages = totalPages,
                        Data = pageData
                    };

                    LogHelper.LogInformation(_logger, "GET", "/api/category", null, response);
                    return new ResponseObject(200, "Query data successfully", response);
                }
                LogHelper.LogInformation(_logger, "GET", "/api/category", null, categories);
                return new ResponseObject(200, "Query data successfully", categories);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/category");
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> FindListAllAsync()
        {
            try
            {
                var categories = await _categoryRepository.FindListAllAsync();
                LogHelper.LogInformation(_logger, "GET", "/api/category/all", null, categories);
                return new ResponseObject(200, "Query data successfully", categories);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/category");
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> FindByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.FindByIdAsync(id);
                if (category == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }
                LogHelper.LogInformation(_logger, "GET", "/api/category/{id}", id, category);
                return new ResponseObject(200, "Query data successfully", category);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/category/{id}", id);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> FindBySlugAsync(string slug)
        {
            try
            {
                var category = await _categoryRepository.FindBySlugAsync(slug);
                if (category == null)
                {
                    return new ResponseObject(404, $"Cannot find data with slug {slug}", null);
                }
                LogHelper.LogInformation(_logger, "GET", "/api/category/{slug}", slug, category);
                return new ResponseObject(200, "Query data successfully", category);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/category/{slug}", slug);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> SaveAsync(CategoryViewModel model)
        {
            try
            {
                var existingCategory = await _categoryRepository.FindByNameAsync(model.CategoryName);
                if (existingCategory != null)
                {
                    return new ResponseObject(400, "Category name already taken");
                }

                var category = new Category
                {
                    CategoryName = model.CategoryName,
                    CategoryStatus = model.CategoryStatus,
                    CategorySlug = Util.GenerateSlug(model.CategoryName)
                };

                await _categoryRepository.AddAsync(category);
                LogHelper.LogInformation(_logger, "POST", "/api/category", model, category);
                return new ResponseObject(200, "Insert data successfully", category);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "POST", "/api/category", model);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> UpdateAsync(int id, CategoryViewModel model)
        {
            try
            {
                var category = await _categoryRepository.FindByIdAsync(id);
                if (category == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }

                category.CategoryName = model.CategoryName;
                category.CategoryStatus = model.CategoryStatus;
                category.CategorySlug = Util.GenerateSlug(model.CategoryName);

                await _categoryRepository.UpdateAsync(category);
                LogHelper.LogInformation(_logger, "PUT", $"/api/category/{id}", model, category);
                return new ResponseObject(200, "Update data successfully", category);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "PUT", $"/api/category/{id}", model);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }

        public async Task<ResponseObject> DeleteAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.FindByIdAsync(id);
                var products = await _categoryRepository.GetProductsByCategoryIdAsync(id);
                if (category == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }
                if (products != null && products.Count > 0)
                {
                    _categoryRepository.DeleteProducts(products);
                }
                await _categoryRepository.DeleteAsync(category);
                LogHelper.LogInformation(_logger, "DELETE", $"/api/category/{id}", id, "Deleted successfully");
                return new ResponseObject(200, "Delete data successfully", null);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "DELETE", $"/api/category/{id}", id);
                return new ResponseObject(500, "Internal server error. Please try again later.", ex.Message);
            }
        }
    }
}
