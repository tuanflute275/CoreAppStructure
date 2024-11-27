namespace CoreAppStructure.Features.Products.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ProductId")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string ProductName { get; set; }

        [Required]
        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string ProductSlug { get; set; }


        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string ProductImage { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double ProductPrice { get; set; }

        [Range(0, double.MaxValue)]
        public double ProductSalePrice { get; set; }

        [Required]
        [Column]
        public int CategoryId { get; set; }

        [Column(TypeName = "ntext")]
        public string ProductDescription { get; set; }

        [Column]
        public bool ProductStatus { get; set; }

        [Column] // IsFeatured: Đánh dấu sản phẩm nổi bật trên trang chính.     
        public bool IsFeatured { get; set; } = false;


        // Thêm các trường theo dõi tạo, cập nhật, xóa
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? CreateBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? UpdateBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }

        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? DeleteBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }

        [StringLength(1)]
        [Column(TypeName = "nvarchar(1)")]
        public string? DeleteFlag { get; set; } // Y or N

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
