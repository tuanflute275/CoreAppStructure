namespace CoreAppStructure.Features.Categories.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column]
        public int CategoryId { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string CategoryName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string? CategorySlug { get; set; }

        [Column]
        public bool CategoryStatus { get; set; } = true;

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

        [System.Text.Json.Serialization.JsonIgnore]
        public virtual ICollection<Product> Products { get; set; }
    }
}
