namespace CoreAppStructure.Features.Products.Mappings
{
    public class ProductMapping : AutoMapper.Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(src => src.Category.CategorySlug));
        }
    }
}
