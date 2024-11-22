using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CoreAppStructure.Data.Entities;
using CoreAppStructure.Core.Helpers;

namespace CoreAppStructure.Features.Users.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(200)")]
        public string UserName { get; set; }

        [Required]
        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string UserFullName { get; set; }

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string? UserAvatar { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string UserPassword { get; set; }

        [Phone]
        [StringLength(15)]
        [Column(TypeName = "nvarchar(15)")]
        public string? UserPhoneNumber { get; set; }

        [StringLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string? UserAddress { get; set; }

        [Column]
        public bool? UserGender { get; set; } = true;

        [Column(TypeName = "int")]
        public int UserActive { get; set; } = 0;       

        [Range(0, int.MaxValue)]
        [Column]
        public int? FailedLoginAttempts { get; set; } = 0;   // Số lần đăng nhập thất bại liên tiếp
                                                            // (dùng để khóa tài khoản tạm thời).

        [Column]
        public DateTime? UserCurrentTime { get; set; } // thời gian bắt đầu khóa tài khoản

        [Column]
        public DateTime? UserUnlockTime { get; set; } // thời gian tài khoản được mở khóa


        //
        public string? SecurityStamp { get; set; }              // Dùng để theo dõi các thay đổi bảo mật
        public string? ResetPasswordToken { get; set; }         // Lưu token phục vụ tính năng quên mật khẩu.
        public DateTime? ResetPasswordTokenExpiry { get; set; } // Hạn sử dụng của token quên mật khẩu.
        public DateTime? LastLoginDate { get; set; }            // Lưu thời điểm đăng nhập gần nhất.

        public DateTime? DateOfBirth { get; set; }           // Ngày sinh
        public string? PlaceOfBirth { get; set; }            // Nơi sinh
        public string? Nationality { get; set; }             // Quốc tịch của người dùng.
        public string? UserBio { get; set; }                 // Mô tả ngắn về người dùng.
        public string? SocialLinks { get; set; }             // Liên kết đến các tài khoản mạng xã hội (JSON)


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

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
