namespace CoreAppStructure.Core.Configurations
{
    public static class LoggingConfiguration
    {
        public static void AddLoggingConfiguration(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();       // Ghi log ra Console
                builder.ClearProviders();   // Xóa nhà cung cấp log mặc định
                builder.AddDebug();         // Thêm Debug logger
            });
        }
    }
}
