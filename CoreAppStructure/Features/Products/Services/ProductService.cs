using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Features.Categories.Interfaces;
using CoreAppStructure.Features.Categories.Repositories;
using CoreAppStructure.Features.Products.Interfaces;
using CoreAppStructure.Features.Products.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using X.PagedList.Extensions;

namespace CoreAppStructure.Features.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        public ProductService(IProductRepository productRepository, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _productRepository = productRepository;
            _environment = environment;
        }

        public async Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1)
        {
            var products = await _productRepository.FindAllAsync(name, sort);

            if (products.Count > 0)
            {
                int totalRecords = products.Count();
                int limit = 10;
                page = page <= 1 ? 1 : page;
                var pageData = products.ToPagedList(page, limit);

                int totalPages = (int)Math.Ceiling((double)totalRecords / limit);

                var productDTO = products.Select(p => new ProductDTO
                {
                    ProductId = p.ProductId,
                    ProductImage = p.ProductImage,
                    ProductName = p.ProductName,
                    ProductSlug = p.ProductSlug,
                    ProductPrice = p.ProductPrice,
                    ProductSalePrice = p.ProductSalePrice,
                    ProductStatus = p.ProductStatus,
                    ProductDescription = p.ProductDescription,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.CategoryName,
                    CategorySlug = p.Category.CategorySlug
                }).ToList();

                var response = new
                {
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    Data = productDTO
                };

                return new ResponseObject(200, "Query data successfully", response);
            }
            return new ResponseObject(200, "Query data successfully", products);
        }

        public async Task<ResponseObject> FindByIdAsync(int id)
        {
            var product = await _productRepository.FindByIdAsync(id);
            if (product == null)
            {
                return new ResponseObject(404, $"Cannot find data with id {id}", null);
            }
            var productDTO = new ProductDTO
            {
                ProductId = product.ProductId,
                ProductImage = product.ProductImage,
                ProductName = product.ProductName,
                ProductSlug = product.ProductSlug,
                ProductPrice = product.ProductPrice,
                ProductSalePrice = product.ProductSalePrice,
                ProductStatus = product.ProductStatus,
                ProductDescription = product.ProductDescription,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.CategoryName,
                CategorySlug = product.Category.CategorySlug
            };
            return new ResponseObject(200, "Query data successfully", productDTO);
        }

        public async Task<ResponseObject> FindBySlugAsync(string slug)
        {
            var product = await _productRepository.FindBySlugAsync(slug);
            if (product == null)
            {
                return new ResponseObject(404, $"Cannot find data with slug {slug}", null);
            }
            var productDTO = new ProductDTO
            {
                ProductId = product.ProductId,
                ProductImage = product.ProductImage,
                ProductName = product.ProductName,
                ProductSlug = product.ProductSlug,
                ProductPrice = product.ProductPrice,
                ProductSalePrice = product.ProductSalePrice,
                ProductStatus = product.ProductStatus,
                ProductDescription = product.ProductDescription,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.CategoryName,
                CategorySlug = product.Category.CategorySlug
            };
            return new ResponseObject(200, "Query data successfully", productDTO);
        }

        public async Task<ResponseObject> SaveAsync(ProductViewModel model, string request)
        {
            try
            {
                if (model.ImageFile == null)
                {
                    return new ResponseObject(400, "Image Is Required", null);
                }

                Product product = new Product
                {
                    ProductName = model.ProductName,
                    ProductSlug = Util.GenerateSlug(model.ProductName),
                    ProductPrice = model.ProductPrice,
                    ProductSalePrice = model.ProductSalePrice,
                    ProductStatus = model.ProductStatus,
                    CategoryId = model.CategoryId,
                    ProductDescription = model.ProductDescription
                };

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Đường dẫn lưu file
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                    var filePath = Path.Combine(uploadsFolder, model.ImageFile.FileName);

                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Lưu file hình ảnh
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // Cập nhật đường dẫn hình ảnh vào thuộc tính sản phẩm
                    product.ProductImage = $"http://{request}/uploads/products/{model.ImageFile.FileName}";
                }

                await _productRepository.AddAsync(product);
                return new ResponseObject(200, "Insert data successfully", product);

            }
            catch (Exception ex)
            {
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public async Task<ResponseObject> UpdateAsync(int id, ProductViewModel model, string request)
        {
            try
            {
                if (model.ImageFile == null)
                {
                    return new ResponseObject(400, "Image Is Required", null);
                }

                Product product = new Product
                {
                    ProductName = model.ProductName,
                    ProductSlug = Util.GenerateSlug(model.ProductName),
                    ProductPrice = model.ProductPrice,
                    ProductSalePrice = model.ProductSalePrice,
                    ProductStatus = model.ProductStatus,
                    CategoryId = model.CategoryId,
                    ProductDescription = model.ProductDescription
                };

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Đường dẫn lưu file
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                    var filePath = Path.Combine(uploadsFolder, model.ImageFile.FileName);

                    // Xóa hình ảnh cũ nếu tồn tại
                    if (!string.IsNullOrEmpty(model.OldImage))
                    {
                        var oldFileName = model.OldImage.Split($"{request}/uploads/products/").LastOrDefault();
                        var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Tạo thư mục nếu chưa tồn tại
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Lưu file hình ảnh
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    // Cập nhật đường dẫn hình ảnh vào thuộc tính sản phẩm
                    product.ProductImage = $"http://{request}/uploads/products/{model.ImageFile.FileName}";
                }
                else
                {
                    // Nếu không có hình ảnh mới, giữ nguyên hình ảnh cũ
                    product.ProductImage = model.OldImage;
                }

                await _productRepository.UpdateAsync(product);
                return new ResponseObject(200, "Insert data successfully", product);

            }
            catch (Exception ex)
            {
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public async Task<ResponseObject> DeleteAsync(int id)
        {
            var product = await _productRepository.FindByIdAsync(id);
            if (product == null)
            {
                return new ResponseObject(404, $"Cannot find data with id {id}", null);
            }

            await _productRepository.DeleteAsync(product);
            return new ResponseObject(200, "Delete data successfully");
        }
    }
}
