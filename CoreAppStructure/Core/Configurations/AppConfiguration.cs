using CoreAppStructure.Data;
using CoreAppStructure.Features.Categories.Interfaces;
using CoreAppStructure.Features.Categories.Repositories;
using CoreAppStructure.Features.Categories.Services;
using CoreAppStructure.Features.Products.Interfaces;
using CoreAppStructure.Features.Products.Repositories;
using CoreAppStructure.Features.Products.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Text.Json;
using CoreAppStructure.Core.Helpers;
using StackExchange.Redis;
using CoreAppStructure.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace CoreAppStructure.Core.Configurations
{
    public static class AppConfiguration
    {
       public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Load appsettings
            var appSetting = AppSetting.MapValues(configuration);

            // Thêm logging vào DI container
            services.AddLogging(builder =>
            {
                builder.AddConsole();       // Ghi log ra Console (hoặc sử dụng AddDebug, AddFile, ...)
                builder.ClearProviders();   // Xóa nhà cung cấp log mặc định
                builder.AddDebug();         // Thêm Debug logger
            });

            // Cấu hình kết nối SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(appSetting.SqlServerConnection));


            // Cấu hình kết nối Redis với fallback sử dụng bộ nhớ nếu Redis không khả dụng
            var redisConnectionString = appSetting.RedisConnection;
            var isRedisConnected = false;
            IConnectionMultiplexer redis = null;

            try
            {
                // Thử kết nối Redis
                redis = ConnectionMultiplexer.Connect(redisConnectionString);
                isRedisConnected = redis.IsConnected;
            }
            catch (Exception ex)
            {
                // Ghi log nếu không thể kết nối Redis
                Console.WriteLine($"Không thể kết nối Redis: {ex.Message}");
            }

            if (isRedisConnected)
            {
                // Nếu kết nối Redis thành công, sử dụng RedisCache
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                });

                // Đảm bảo sử dụng RedisCacheService với kết nối Redis
                services.AddScoped<RedisCacheService, RedisCacheService>(sp =>
                    new RedisCacheService(redisConnectionString, null));
            }
            else
            {
                // Nếu không thể kết nối Redis, sử dụng MemoryCache (fallback)
                Console.WriteLine("Kết nối Redis thất bại, sử dụng MemoryCache.");
                services.AddMemoryCache(); // Thêm MemoryCache nếu không kết nối được Redis
                                           // Đảm bảo fallback sử dụng MemoryCache trong RedisCacheService
                services.AddScoped<RedisCacheService, RedisCacheService>(sp =>
                    new RedisCacheService(redisConnectionString, sp.GetRequiredService<IMemoryCache>()));
            }



            // Cấu hình CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")  // Cho phép nguồn từ localhost:3000
                          .AllowAnyHeader()                    // Cho phép bất kỳ header nào
                          .AllowAnyMethod()                    // Cho phép bất kỳ phương thức HTTP nào
                          .AllowCredentials();                 // Cho phép cookies hoặc thông tin xác thực khác
                });
            });


            // Cấu hình JWT
            var key = configuration["Jwt:Key"];  // Đảm bảo dùng IConfiguration trực tiếp
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;  // Cài đặt này cần bật khi triển khai ứng dụng thực tế
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],  // Lấy thông tin từ appsettings.json

                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],  // Lấy thông tin từ appsettings.json

                    IssuerSigningKey = signingKey,  // Đăng ký khóa ký cho JWT

                    RequireExpirationTime = true,
                    ValidateLifetime = true,  // Kiểm tra thời gian sống của token
                };

                options.Events = new JwtBearerEvents
                {
                    // Xử lý sự kiện khi không có token hoặc token không hợp lệ
                    OnChallenge = context =>
                    {
                        // Bỏ qua phản hồi mặc định của JWT Bearer
                        context.HandleResponse();

                        // Thiết lập trạng thái và kiểu nội dung phản hồi
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        // Tạo một đối tượng ResponseObject với mã lỗi và thông điệp tùy chỉnh
                        var result = JsonSerializer.Serialize(new ResponseObject(401, "Unauthorized. Token is invalid or missing."));

                        // Trả về phản hồi lỗi tùy chỉnh dưới dạng JSON
                        return context.Response.WriteAsync(result);
                    },

                    // Xử lý sự kiện khi token hợp lệ nhưng người dùng không có quyền truy cập vào tài nguyên
                    OnForbidden = context =>
                    {
                        // Thiết lập trạng thái và kiểu nội dung phản hồi
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        // Tạo một đối tượng ResponseObject với mã lỗi và thông điệp tùy chỉnh
                        var result = JsonSerializer.Serialize(new ResponseObject(403, "Forbidden. You do not have permission to access this resource."));

                        // Trả về phản hồi lỗi tùy chỉnh dưới dạng JSON
                        return context.Response.WriteAsync(result);
                    }
                };
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
