using CoreAppStructure.Core.Middlewares;
using Serilog;

namespace CoreAppStructure.Core.Configurations
{
    public static class AppConfiguration
    {
        public static void ConfigureMiddleware(IApplicationBuilder app)
        {
            // Cấu hình các middleware
            app.UseCors("AllowOrigin");  // CORS policy
            app.UseStaticFiles();        // Cung cấp các file tĩnh (nếu có)
            app.UseHttpsRedirection();   // Chuyển hướng tất cả yêu cầu HTTP sang HTTPS

            // Middleware cho xác thực và phân quyền
            app.UseAuthentication();
            app.UseAuthorization();

            // Cấu hình middleware cho xử lý ngoại lệ (ExceptionMiddleware)
            app.UseMiddleware<ExceptionMiddleware>();

            // Ghi log các request vào Serilog
            app.UseSerilogRequestLogging(); // Ghi log các request HTTP
        }
    }
}
