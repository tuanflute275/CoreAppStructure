using CoreAppStructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CoreAppStructure.Core.Configurations
{
    public static class SqlServerConfiguration
    {
        public static void AddSqlServerConfiguration(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
    }
}
