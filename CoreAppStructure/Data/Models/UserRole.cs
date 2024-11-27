namespace CoreAppStructure.Data.Entities
{
    [Table("UserRoles")]
    public class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserRoleId { get; set; }

        [Column]
        [Required]
        [StringLength(255)]
        public int UserId { get; set; }

        [Column]
        [Required]
        [StringLength(255)]
        public int RoleId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("RoleId")]
        public virtual Features.Roles.Models.Role Role { get; set; }
    }
}
