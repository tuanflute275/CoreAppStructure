using CoreAppStructure.Features.Products.Models;
using AutoMapper;

namespace CoreAppStructure.Features.Products.Mappings
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.CategorySlug, opt => opt.MapFrom(src => src.Category.CategorySlug));
        }
    }
}
