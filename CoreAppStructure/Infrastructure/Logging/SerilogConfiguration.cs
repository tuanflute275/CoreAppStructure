using Serilog;
using ILogger = Serilog.ILogger;

namespace CoreAppStructure.Infrastructure.Logging
{
    public static class SerilogConfiguration
    {
        public static void AddSerilogConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory); // Tạo thư mục nếu chưa tồn tại
            }

            // cấu hình logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();       // Ghi log ra Console
                builder.ClearProviders();   // Xóa nhà cung cấp log mặc định
                builder.AddDebug();         // Thêm Debug logger
            });

            // Cấu hình Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)  // Đọc cấu hình từ appsettings.json
                .Enrich.FromLogContext()                // Thêm ngữ cảnh log
                .WriteTo.Console()                      // Ghi log ra Console
                .WriteTo.File(
                    Path.Combine(logDirectory, "log-.txt"),  // Đường dẫn tới thư mục LogFileDirectory
                    rollingInterval: RollingInterval.Day,     // Log mỗi ngày vào file mới
                    retainedFileCountLimit: 7                // Giới hạn số file log giữ lại (ví dụ 7 ngày)
                )
                .CreateLogger();

            // Đăng ký Serilog làm logger chính
            services.AddSingleton<ILogger>(Log.Logger);
        }
    }
}
