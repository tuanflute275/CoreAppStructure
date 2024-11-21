using CoreAppStructure.Data;
using Microsoft.EntityFrameworkCore;


namespace CoreAppStructure.Core.Configurations
{
    public static class AppConfiguration
    {
       public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Load appsettings
            var appSetting = AppSetting.MapValues(configuration);

            // Cấu hình Logging
            services.AddLoggingConfiguration();

            // Cấu hình SQL Server
            services.AddSqlServerConfiguration(appSetting.SqlServerConnection);

            // Cấu hình AutoMapper
            services.AddAutoMapper();

            // Cấu hình CORS
            services.AddCorsConfiguration();

            // Cấu hình JWT
            services.AddJwtConfiguration(configuration);

            // Cấu hình Cache (Redis or MemoryCache)
            services.AddCacheConfiguration(appSetting.RedisConnection);

            // Cấu hình các dịch vụ nghiệp vụ
            services.AddServiceConfiguration();

        }
    }
}
