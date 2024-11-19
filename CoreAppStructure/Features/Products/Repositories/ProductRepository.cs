using CoreAppStructure.Data;
using CoreAppStructure.Features.Categories.Models;
using CoreAppStructure.Features.Products.Interfaces;
using CoreAppStructure.Features.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace CoreAppStructure.Features.Products.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> FindAllAsync(string? name, string? sort)
        {
            var products = _context.Products.Include(x => x.Category).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(x => x.ProductName.Contains(name));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort)
                {
                    case "Id-ASC":
                        products = products.OrderBy(x => x.CategoryId);
                        break;
                    case "Id-DESC":
                        products = products.OrderByDescending(x => x.CategoryId);
                        break;
                    case "Name-ASC":
                        products = products.OrderBy(x => x.ProductName);
                        break;
                    case "Name-DESC":
                        products = products.OrderByDescending(x => x.ProductName);
                        break;
                    case "Price-ASC":
                        products = products.OrderBy(x => x.ProductPrice);
                        break;
                    case "Price-DESC":
                        products = products.OrderByDescending(x => x.ProductSalePrice);
                        break;
                }
            }

            return await products.ToListAsync();
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            return await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductId == id);
        }

        public async Task<Product> FindBySlugAsync(string slug)
        {
            return await _context.Products.Include(x => x.Category).FirstOrDefaultAsync(x => x.ProductSlug == slug);
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
