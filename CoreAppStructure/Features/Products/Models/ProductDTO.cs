namespace CoreAppStructure.Features.Products.Models
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSlug { get; set; }
        public string ProductImage { get; set; }
        public double ProductPrice { get; set; }
        public double ProductSalePrice { get; set; }
        public bool ProductStatus { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategorySlug { get; set; }
        public string ProductDescription { get; set; }
    }
}
