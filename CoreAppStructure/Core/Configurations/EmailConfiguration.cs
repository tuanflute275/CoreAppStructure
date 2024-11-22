using CoreAppStructure.Infrastructure.Email;

namespace CoreAppStructure.Core.Configurations
{
    public static class EmailConfiguration
    {
        public static void AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind cấu hình Email từ appsettings.json
            var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfig>();

            // Đăng ký cấu hình email như một singleton
            services.AddSingleton(emailConfig);

            // Đăng ký dịch vụ EmailService
            services.AddScoped<IEmailService, EmailService>();
        }
    }
}
