namespace CoreAppStructure.Core.Extensions
{
    public static class HostExtention
    {
        /// <summary>
        /// Nạp dữ liệu vào memoryvà context của cơ sở dữ liệu khi khởi động ứng dụng.
        /// Phương thức này nhận một hành động (seeder) để khởi tạo dữ liệu cho cảmemoryvà cơ sở dữ liệu.
        /// </summary>
        /// <typeparam name="TInMemoryContext">Loại của context memory.</typeparam>
        /// <typeparam name="TDbContext">Loại của context cơ sở dữ liệu.</typeparam>
        /// <param name="host">Host của ứng dụng.</param>
        /// <param name="seeder">Hành động để khởi tạo dữ liệu cho memory và cơ sở dữ liệu.</param>
        /// <returns>AppHost.</returns>
        public static IHost LoadDataToMemory<TInMemoryContext, TDbContext>(this IHost host, Action<TInMemoryContext, TDbContext> seeder)
            where TDbContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services    = scope.ServiceProvider;
                var context     = services.GetService<TInMemoryContext>();
                var dbContext   = services.GetRequiredService<TDbContext>();
                seeder(context, dbContext);
            }
            return host;
        }

        /// <summary>
        /// Kiểm tra kết nối đến các dịch vụ cần thiết của ứng dụng (ví dụ: Kafka, Redis, Cơ sở dữ liệu).
        /// Phương thức này sử dụng trong ngữ cảnh của WebApplication.
        /// </summary>
        /// <param name="app">WebApplication.</param>
        /// <returns>WebApplication.</returns>
        public static WebApplication EnsureNetworkConnectivity(this WebApplication app)
        {
            app.EnsureNetworkConnectivity(app.Configuration, app.Logger);
            return app;
        }

        /// <summary>
        /// Kiểm tra kết nối đến các dịch vụ (Kafka, Redis, Elasticsearch, Cơ sở dữ liệu) và đảm bảo chúng khả dụng.
        /// Nếu bất kỳ dịch vụ nào không kết nối được, ứng dụng sẽ dừng.
        /// </summary>
        /// <param name="host">Host của ứng dụng.</param>
        /// <param name="configuration">Cấu hình của ứng dụng.</param>
        /// <param name="logger">Logger để ghi lại quá trình kiểm tra kết nối (tuỳ chọn).</param>
        /// <returns>Host.</returns>
        public static IHost EnsureNetworkConnectivity(this IHost host, IConfiguration configuration, MicrosoftLog.ILogger logger = null)
        {
            logger ??= LoggerFactory.Create(build => build.AddConsole()).CreateLogger("NetworkCheck");
            var checker = new NetworkChecker(logger);

            //// Kafka
            //var kafkaEndpoint = GetEndpoint(configuration["KafkaConfig:BootstrapServers"], 9092);
            //checker.EnsureConnectivity(kafkaEndpoint.Host, kafkaEndpoint.Port, "Kafka");

            //// Redis
            //var redisEndpoint = GetEndpoint(configuration["RedisConfig:Host"], 6379);
            //checker.EnsureConnectivity(redisEndpoint.Host, redisEndpoint.Port, "Redis");

            //// Elasticsearch
            //var elasticEndpoint = GetEndpoint(configuration["ElasticConfig:Uri"], 9200);
            //checker.EnsureConnectivity(elasticEndpoint.Host, elasticEndpoint.Port, "Elasticsearch");

            //// Database
            //var dbConnectionString = configuration.GetConnectionString("OrclDb");
            //var dbProvider = configuration["DatabaseConfig:Provider"]; // e.g., "SqlServer", "PostgreSQL"
            //checker.EnsureDatabaseConnectivity(dbConnectionString, dbProvider, "Database");

            if (checker.FailedCount != 0)
            {
                Environment.Exit(0);
            }

            return host;
        }

        /// <summary>
        /// Phân tích giá trị cấu hình để lấy thông tin host và cổng kết nối.
        /// </summary>
        /// <param name="configValue">Giá trị cấu hình chứa thông tin host và cổng (dạng host:port).</param>
        /// <param name="defaultPort">Cổng mặc định nếu không có cổng trong cấu hình.</param>
        /// <returns>Tuple chứa thông tin host và cổng.</returns>
        private static (string Host, int Port) GetEndpoint(string configValue, int defaultPort)
        {
            var parts = configValue.Split(':');
            var host = parts[0];

            if (parts.Length == 1) return (host, defaultPort);
            if (!int.TryParse(parts[1], out int port))
                throw new ArgumentException("Port không hợp lệ: " + parts[1]);

            return (host, port);
        }

    }
}
