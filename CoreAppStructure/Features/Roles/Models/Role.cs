namespace CoreAppStructure.Features.Roles.Models
{
    [Table("Roles")]
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(200)")]
        public string RoleName { get; set; }

        [Column(TypeName = "ntext")]
        public string? RoleDescription { get; set; }

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
        [JsonIgnore]
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
