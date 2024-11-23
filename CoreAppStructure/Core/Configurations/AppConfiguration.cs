using CoreAppStructure.Infrastructure.Caching;
using CoreAppStructure.Infrastructure.Email;
using CoreAppStructure.Infrastructure.Logging;


namespace CoreAppStructure.Core.Configurations
{
    public static class AppConfiguration
    {
       public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Load appsettings
            var appSetting = AppSetting.MapValues(configuration);

            // Cấu hình Logging (Serilog)
            services.AddSerilogConfiguration(configuration);

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

            // Cấu hình Email
            services.AddEmailConfiguration(configuration);

            // Cấu hình các dịch vụ nghiệp vụ
            services.AddServiceConfiguration();

        }
    }
}
