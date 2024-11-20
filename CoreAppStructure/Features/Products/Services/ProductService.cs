﻿using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Features.Products.Interfaces;
using CoreAppStructure.Features.Products.Models;
using X.PagedList;
using CoreAppStructure.Core.Extensions;
using CoreAppStructure.Infrastructure.Caching;
using Newtonsoft.Json;

namespace CoreAppStructure.Features.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly ILogger<ProductService> _logger;
        private readonly RedisCacheService _redisCacheService;
        private readonly string _cacheKeyPrefix = "product_";

        public ProductService(IProductRepository productRepository, 
            Microsoft.AspNetCore.Hosting.IHostingEnvironment environment,
            ILogger<ProductService> logger,
            RedisCacheService redisCacheService)
        {
            _productRepository = productRepository;
            _environment = environment;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        }

        public async Task<ResponseObject> FindAllAsync(string? name, string? sort, int page = 1)
        {
           try
            {
                //var cacheKey = $"{_cacheKeyPrefix}{name ?? "all"}_{sort ?? "default"}_page_{page}";
                var cacheKey = $"{_cacheKeyPrefix}";
                var cachedData = await _redisCacheService.GetCacheAsync(cacheKey);

                if (cachedData != null)
                {
                    // Nếu có dữ liệu trong cache, trả về dữ liệu từ cache
                    var response = JsonConvert.DeserializeObject<ResponseDTO>(cachedData);
                    return new ResponseObject(200, "Query data successfully", response);
                }

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
                    // Lưu vào Redis
                    await _redisCacheService.SetCacheAsync(cacheKey, JsonConvert.SerializeObject(response), TimeSpan.FromMinutes(10));
                    LogHelper.LogInformation(_logger, "GET", "/api/product", null, response);
                    return new ResponseObject(200, "Query data successfully", response);
                }
                LogHelper.LogInformation(_logger, "GET", "/api/product", null, products);
                return new ResponseObject(200, "Query data successfully", products);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/product");
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public async Task<ResponseObject> FindByIdAsync(int id)
        {
            try
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
                LogHelper.LogInformation(_logger, "GET", "/api/product/{id}", id, productDTO);
                return new ResponseObject(200, "Query data successfully", productDTO);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/product/{id}", id);
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public async Task<ResponseObject> FindBySlugAsync(string slug)
        {
            try
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
                LogHelper.LogInformation(_logger, "GET", "/api/product/{slug}", slug, productDTO);
                return new ResponseObject(200, "Query data successfully", productDTO);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "GET", $"/api/product/{slug}", slug);
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
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
                    product.ProductImage = $"http://{request}/uploads/product/{model.ImageFile.FileName}";
                }

                await _productRepository.AddAsync(product);
                // Cập nhật lại cache Redis
                await UpdateRedisCacheAsync();
                LogHelper.LogInformation(_logger, "POST", "/api/product", model, product);
                return new ResponseObject(200, "Insert data successfully", product);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "POST", "/api/product", model);
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
                var product = await _productRepository.FindByIdAsync(id);
                if (product == null)
                {
                    return new ResponseObject(404, "Product not found", null);
                }

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

                product.ProductName = model.ProductName;
                product.ProductSlug = Util.GenerateSlug(model.ProductName);
                product.ProductPrice = model.ProductPrice;
                product.ProductSalePrice = model.ProductSalePrice;
                product.ProductStatus = model.ProductStatus;
                product.CategoryId = model.CategoryId;
                product.ProductDescription = model.ProductDescription;

                await _productRepository.UpdateAsync(product);
                // Cập nhật lại cache Redis
                await UpdateRedisCacheAsync();
                LogHelper.LogInformation(_logger, "PUT", $"/api/product/{id}", model, product);
                return new ResponseObject(200, "Update data successfully", product);

            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "PUT", $"/api/product/{id}", model);
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        public async Task<ResponseObject> DeleteAsync(int id)
        {
            try
            {
                var product = await _productRepository.FindByIdAsync(id);
                if (product == null)
                {
                    return new ResponseObject(404, $"Cannot find data with id {id}", null);
                }

                await _productRepository.DeleteAsync(product);
                // Cập nhật lại cache Redis
                await UpdateRedisCacheAsync();
                LogHelper.LogInformation(_logger, "DELETE", $"/api/product/{id}", id, "Deleted successfully");
                return new ResponseObject(200, "Delete data successfully");
            }
            catch (Exception ex)
            {
                LogHelper.LogError(_logger, ex, "DELETE", $"/api/product/{id}", id);
                return new ResponseObject(500, "Internal server error. Please try again later.", null);
            }
        }

        private async Task UpdateRedisCacheAsync()
        {
            // Tạo key cache cho danh sách sản phẩm (có thể thêm thông tin phân trang hoặc lọc nếu cần)
            var cacheKey = $"{_cacheKeyPrefix}";

            // Lấy toàn bộ danh sách sản phẩm từ cơ sở dữ liệu
            var products = await _productRepository.FindAllAsync(null, null);

            // Chuyển dữ liệu thành dạng mà bạn muốn lưu trữ trong cache (ví dụ DTO)
            var productDTOs = products.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductPrice = p.ProductPrice,
                ProductSalePrice = p.ProductSalePrice,
                ProductImage = p.ProductImage,
                ProductStatus = p.ProductStatus,
                CategoryId = p.CategoryId
            }).ToList();


            if (products.Count > 0)
            {
                int totalRecords = products.Count();
                int limit = 10;
                int page = 1;
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

                // Lưu toàn bộ dữ liệu sản phẩm vào Redis
                await _redisCacheService.SetCacheAsync(cacheKey, JsonConvert.SerializeObject(response), TimeSpan.FromMinutes(10));

                // Log thông tin cache đã được cập nhật
                LogHelper.LogInformation(_logger, "Cache updated after adding product", cacheKey, null, productDTOs);
            }
           
        }
    }
}
