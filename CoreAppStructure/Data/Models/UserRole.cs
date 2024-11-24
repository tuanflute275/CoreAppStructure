﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CoreAppStructure.Features.Users.Models;
using CoreAppStructure.Features.Roles.Models;

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
        public virtual Role Role { get; set; }
    }
}
