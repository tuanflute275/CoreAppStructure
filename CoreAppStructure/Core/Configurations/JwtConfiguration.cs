using CoreAppStructure.Core.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace CoreAppStructure.Core.Configurations
{
    public static class JwtConfiguration
    {
        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"];
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
        }
    }
}
