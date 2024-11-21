namespace CoreAppStructure.Core.Configurations
{
    public static class CorsConfiguration
    {
        public static void AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")  // Cho phép nguồn từ localhost:3000
                          .AllowAnyHeader()                      // Cho phép bất kỳ header nào
                          .AllowAnyMethod()                      // Cho phép bất kỳ phương thức HTTP nào
                          .AllowCredentials();                   // Cho phép cookies hoặc thông tin xác thực khác
                });
            });
        }
    }
}
