using AutoMapper;
using CoreAppStructure.Features.Products.Mappings;

namespace CoreAppStructure.Core.Configurations
{
    public static class AutoMapperConfiguration
    {
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddSingleton<IMapper>(sp =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile<ProductMapping>();
                    // thêm các mapping khác
                });
                return config.CreateMapper();
            });
        }
    }
}
