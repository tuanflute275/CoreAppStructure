using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Features.Categories.Interfaces;
using CoreAppStructure.Features.Categories.Models;
using X.PagedList.Extensions;

namespace CoreAppStructure.Features.Categories.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1)
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

                return new ResponseObject(200, "Query data successfully", response);
            }
            return new ResponseObject(200, "Query data successfully", categories);
        }

        public async Task<ResponseObject> FindListAllAsync()
        {
            var categories = await _categoryRepository.FindListAllAsync();
            return new ResponseObject(200, "Query data successfully", categories);
        }

        public async Task<ResponseObject> FindByIdAsync(int id)
        {
            var category = await _categoryRepository.FindByIdAsync(id);
            if (category == null)
            {
                return new ResponseObject(404, $"Cannot find data with id {id}", null);
            }
            return new ResponseObject(200, "Query data successfully", category);
        }

        public async Task<ResponseObject> FindBySlugAsync(string slug)
        {
            var category = await _categoryRepository.FindBySlugAsync(slug);
            if (category == null)
            {
                return new ResponseObject(404, $"Cannot find data with slug {slug}", null);
            }
            return new ResponseObject(200, "Query data successfully", category);
        }

        public async Task<ResponseObject> SaveAsync(CategoryViewModel model)
        {
            var existingCategory = await _categoryRepository.FindBySlugAsync(model.CategoryName);
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
            return new ResponseObject(200, "Insert data successfully", category);
        }

        public async Task<ResponseObject> UpdateAsync(int id, CategoryViewModel model)
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
            return new ResponseObject(200, "Update data successfully", category);
        }

        public async Task<ResponseObject> DeleteAsync(int id)
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
            try
            {
                await _categoryRepository.DeleteAsync(category);
                return new ResponseObject(200, "Delete data successfully", null);
            }
            catch (Exception ex)
            {
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }
    }
}
