using Nest;
namespace CoreAppStructure.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDerivativeTradeServices(this IServiceCollection services, IConfiguration configuration, AppSetting appSetting)
        {
            services
                .AddHttpContextAccessorService()
                .AddSerilogConfiguration(configuration)
                .AddOauth2Configuration(configuration)
                .AddAutoMapper()
                .AddScopedServices()
                .AddSingletonServices()
                .AddTransientServices()
                .AddCorsConfiguration()
                .AddJwtConfiguration(configuration)
                .AddEmailConfiguration(configuration)
                .AddRabbitMQConfiguration(configuration)
                .AddMornitorConfiguration(configuration)
                .AddElasticSearchConfiguration(configuration)
                .AddCacheConfiguration(appSetting.RedisConnection)
                .AddSqlServerConfiguration(appSetting.SqlServerConnection);
            return services;
        }

        // add scope
        private static IServiceCollection AddScopedServices(this IServiceCollection services)
        {
            // Đăng ký dịch vụ EmailService
            services.AddScoped<IEmailService, EmailService>();

            // Dịch vụ liên quan đến Product
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();

            // Dịch vụ liên quan đến Category
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            // Dịch vụ liên quan đến Parameter
            services.AddScoped<IParameterRepository, ParameterRepository>();
            services.AddScoped<IParameterService, ParameterService>();

            // Dịch vụ liên quan đến Role
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();

            // Dịch vụ liên quan đến User
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            // Dịch vụ liên quan đến Auth
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        // add singleton
        private static IServiceCollection AddHttpContextAccessorService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            return services;
        }

        // add singleton
        private static IServiceCollection AddSingletonServices(this IServiceCollection services)
        {
            return services;
        }

        // add trabsient
        private static IServiceCollection AddTransientServices(this IServiceCollection services)
        {
            return services;
        }

        // Cấu hình mornitoring (Promethues)
        public static IServiceCollection AddMornitorConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(otp =>
                    {
                        otp.Endpoint = new Uri(configuration["Otlp:Endpoint"]);
                    });
            })
            .WithMetrics(otp =>
                otp
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(otp => {
                        otp.Endpoint = new Uri(configuration["Otlp:Endpoint"]);
                    })
            );
            services.AddSingleton(Metrics.DefaultRegistry);
            services.AddEndpointsApiExplorer();
            return services;
        }

        // Cấu hình logging (Serilog)
        public static IServiceCollection AddSerilogConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory); // Tạo thư mục nếu chưa tồn tại
            }

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
            services.AddSingleton<ILogger>(Log.Logger);
            return services;
        }
       
        // Đăng ký các dịch vụ scoped
        public static IServiceCollection AddSqlServerConfiguration(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            return services;
        }

        // Cấu hình AutoMapper
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services.AddSingleton<IMapper>(sp =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductMapping>();
                    cfg.AddProfile<UserMapping>();
                    // thêm các mapping khác
                });
                return config.CreateMapper();
            });
            return services;    
        }

        // Cấu hình CORS
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", policy =>
                {
                    policy  // Cho phép nguồn từ localhost:3000
                           .WithOrigins("http://localhost:3000")     
                          .AllowAnyHeader()                         // Cho phép bất kỳ header nào
                          .AllowAnyMethod()                        // Cho phép bất kỳ phương thức HTTP nào
                          .AllowCredentials();                    // Cho phép cookies hoặc thông tin xác thực khác
                });
            });
            return services;
        }

        // Cấu hình JWT
        public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"];
            var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));

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
            return services;
        }

        // Cấu hình bộ nhớ cache (Redis hoặc fallback MemoryCache)
        public static IServiceCollection AddCacheConfiguration(this IServiceCollection services, string connectionString)
        {
            // Cấu hình kết nối Redis với fallback sử dụng bộ nhớ nếu Redis không khả dụng
            var redisConnectionString = connectionString;
            var isRedisConnected = false;
            IConnectionMultiplexer redis = null;

            try
            {
                // Thử kết nối Redis
                redis = ConnectionMultiplexer.Connect(redisConnectionString);
                isRedisConnected = redis.IsConnected;
                CacheState.IsRedisConnectedStatic = redis.IsConnected;
            }
            catch (Exception ex)
            {
                // Ghi log nếu không thể kết nối Redis
                Console.WriteLine($"Không thể kết nối Redis: {ex.Message}");
            }
            finally
            {
                redis?.Dispose();
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
            return services;
        }

        // Cấu hình dịch vụ email
        public static IServiceCollection AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind cấu hình Email từ appsettings.json
            var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailModel>();

            // Đăng ký cấu hình email như một singleton
            services.AddSingleton(emailConfig);
            return services;
        }

        // Cấu hình dịch vụ elasticSearch
        public static IServiceCollection AddElasticSearchConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var uri = configuration["Elasticsearch:Uri"];
            var index = configuration["Elasticsearch:Index"];

            var settings = new ConnectionSettings(new Uri(uri))
                           .DefaultIndex(index);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
            return services;
        }

        // Cấu hình rabbitMQ
        public static IServiceCollection AddRabbitMQConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IRabbitService, RabbitService>(sp => {
                var logger = sp.GetRequiredService<ILogger<RabbitService>>();
                var rabbitSettingConfigurations = configuration.GetSection(nameof(RabbitSetting)).GetChildren();

                var rabbitSettings = new List<RabbitSetting>();
                foreach (var rabbitSettingConfiguration in rabbitSettingConfigurations)
                {
                    var rabbit = rabbitSettingConfiguration.Get<RabbitSetting>();
                    if (!rabbitSettings.Contains(rabbit))
                        rabbitSettings.Add(rabbit);
                }

                var configHNX = rabbitSettings.FirstOrDefault(e => e.Id.Equals(Constants.HNXSettingId));
                var configFixReceive = rabbitSettings.FirstOrDefault(e => e.Id.Equals(Constants.FixReceiveSettingId));
                var factoryHNX = new ConnectionFactory()
                {
                    UserName = configHNX.UserName,
                    Password = configHNX.Password,
                    HostName = configHNX.HostName,
                };
                var factoryFixReceive = new ConnectionFactory()
                {
                    UserName = configFixReceive.UserName,
                    Password = configFixReceive.Password,
                    HostName = configFixReceive.HostName,
                };

                return new RabbitService(factoryHNX, factoryFixReceive, logger, configHNX, configFixReceive);
            });
            return services;
        }

        public static IServiceCollection AddOauth2Configuration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            })
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"];
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/api/auth/google-callback";
                options.SaveTokens = true;
                //options.Scope.Add("email");
                options.Scope.Add("profile");
            })
            .AddFacebook(options =>
            {
                options.AppId = configuration["Authentication:Facebook:AppId"];
                options.AppSecret = configuration["Authentication:Facebook:AppSecret"];
                options.Scope.Add("public_profile");
                options.Fields.Add("picture");
                //options.Scope.Add("email");
            });

            return services;
        }
    }
}
