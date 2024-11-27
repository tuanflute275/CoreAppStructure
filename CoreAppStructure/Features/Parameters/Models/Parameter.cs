namespace CoreAppStructure.Features.Parameters.Models
{
    [Table("Parameters")]
    public class Parameter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParaId { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        public string ParaScope { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public string ParaName { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? ParaShortValue { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ParaLobValue { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string? ParaDesc { get; set; }

        [Column(TypeName = "nvarchar(256)")]
        public string ParaType { get; set; }

        [Column(TypeName = "nvarchar(1)")]
        public string? UserAccessibleFlag { get; set; }  // Y or N

        [Column(TypeName = "nvarchar(1)")]
        public string? AdminAccessibleFlag { get; set; } // Y or N

        [Column(TypeName = "nvarchar(16)")]
        public string? SystemId { get; set; }

        [Column(TypeName = "nvarchar(64)")]
        public string? CreateBy { get; set; }

        [Column(TypeName = "nvarchar(64)")]
        public string? CreateWorkstnId { get; set; }

        [Column]
        public DateTime? CreateDatetime { get; set; }

        [Column(TypeName = "nvarchar(64)")]
        public string? UpdateBy { get; set; }

        [Column(TypeName = "nvarchar(64)")]
        public string? UpdateWorkstnId { get; set; }

        [Column]
        public DateTime? UpdateDatetime { get; set; }

        [Column(TypeName = "nvarchar(64)")]
        public string? DeleteBy { get; set; }

        [Column(TypeName = "nvarchar(64)")]
        public string? DeleteWorkstnId { get; set; }

        [Column]
        public DateTime? DeleteDatetime { get; set; }

        [Column(TypeName = "nvarchar(1)")]
        public string? DeleteFlag { get; set; }
    }
}
