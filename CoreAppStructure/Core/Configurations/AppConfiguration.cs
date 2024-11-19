using CoreAppStructure.Data;
using CoreAppStructure.Features.Categories.Interfaces;
using CoreAppStructure.Features.Categories.Repositories;
using CoreAppStructure.Features.Categories.Services;
using CoreAppStructure.Features.Products.Interfaces;
using CoreAppStructure.Features.Products.Repositories;
using CoreAppStructure.Features.Products.Services;
using Microsoft.EntityFrameworkCore;

namespace CoreAppStructure.Core.Configurations
{
    public static class AppConfiguration
    {
       public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Load appsettings
            var appSetting = AppSetting.MapValues(configuration);

            // Cấu hình kết nối SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(appSetting.SqlServerConnection));

            // Cấu hình kết nối Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = appSetting.RedisConnection;
            });

            // Thêm các dịch vụ liên quan đến Product
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();

            // Thêm các dịch vụ liên quan đến Category
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }
    }
}
