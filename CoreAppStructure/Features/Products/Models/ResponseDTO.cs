namespace CoreAppStructure.Features.Products.Models
{
    public class ResponseDTO
    {
        public int              TotalRecords { get; set; }
        public int              TotalPages   { get; set; }
        public List<ProductDTO> Data         { get; set; }
    }
}
