﻿using Nest;

namespace CoreAppStructure.Core.Configurations
{
    public static class AppConfiguration
    {
        public static void ConfigureMiddleware(this IApplicationBuilder app)
        {
            // Cấu hình các middleware
            app.UseCors("AllowOrigin");  // CORS policy
            app.UseStaticFiles();        // Cung cấp các file tĩnh (nếu có)
            app.UseHttpsRedirection();   // Chuyển hướng tất cả yêu cầu HTTP sang HTTPS

            // Middleware cho xác thực và phân quyền
            app.UseAuthentication();
            app.UseAuthorization();

            // Cấu hình middleware cho xử lý ngoại lệ (ExceptionMiddleware)
            //app.UseMiddleware<ExceptionMiddleware>();

            // Ghi log các request vào Serilog
            app.UseSerilogRequestLogging(); // Ghi log các request HTTP

            // Cấu hình Elasticsearch client cho LogHelper
            var elasticClient = app.ApplicationServices.GetRequiredService<IElasticClient>();
            LogHelper.Configure(elasticClient);

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax,
                HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always
            });
        }
    }
}
