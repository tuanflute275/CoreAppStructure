using CoreAppStructure.Data;
using CoreAppStructure.Features.Categories.Interfaces;
using CoreAppStructure.Features.Categories.Models;
using CoreAppStructure.Features.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreAppStructure.Features.Categories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> FindAllAsync(string? name, string? sort)
        {
            var categories = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                categories = categories.Where(x => x.CategoryName.Contains(name));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        categories = categories.OrderBy(x => x.CategoryId);
                        break;
                    case "Id-DESC":
                        categories = categories.OrderByDescending(x => x.CategoryId);
                        break;
                    case "Name-ASC":
                        categories = categories.OrderBy(x => x.CategoryName);
                        break;
                    case "Name-DESC":
                        categories = categories.OrderByDescending(x => x.CategoryName);
                        break;
                }
            }

            return await categories.ToListAsync();
        }

        public async Task<List<Category>> FindListAllAsync()
        {
            return await _context.Categories.Where(x => x.CategoryStatus == true).ToListAsync();
        }

        public async Task<Category> FindByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<Category> FindBySlugAsync(string slug)
        {
            return await _context.Categories.FirstOrDefaultAsync(x => x.CategorySlug == slug);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> GetProductsByCategoryIdAsync(int id)
        {
            return await _context.Products.Where(x => x.CategoryId == id).ToListAsync();
        }

        public async Task DeleteProducts(List<Product> products)
        {
            _context.Products.RemoveRange(products);
            await _context.SaveChangesAsync();
        }
    }
}
