using CoreAppStructure.Features.Categories.Interfaces;
using CoreAppStructure.Features.Categories.Repositories;
using CoreAppStructure.Features.Categories.Services;
using CoreAppStructure.Features.Parameters.Interfaces;
using CoreAppStructure.Features.Parameters.Repositories;
using CoreAppStructure.Features.Parameters.Services;
using CoreAppStructure.Features.Products.Interfaces;
using CoreAppStructure.Features.Products.Repositories;
using CoreAppStructure.Features.Products.Services;
using CoreAppStructure.Features.Roles.Interfaces;
using CoreAppStructure.Features.Roles.Repositories;
using CoreAppStructure.Features.Roles.Servicces;
using CoreAppStructure.Features.Users.Interfaces;
using CoreAppStructure.Features.Users.Repositories;
using CoreAppStructure.Features.Users.Servicces;

namespace CoreAppStructure.Core.Configurations
{
    public static class ServiceConfiguration
    {
        public static void AddServiceConfiguration(this IServiceCollection services)
        {
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
        }
    }
}
