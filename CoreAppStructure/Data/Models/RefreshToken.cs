using CoreAppStructure.Features.Users.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoreAppStructure.Data.Models
{
    [Table("RefreshTokens")]
    public class RefreshToken
    {
        [Key] // Đánh dấu đây là khóa chính
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] // Bắt buộc phải có giá trị
        [MaxLength(200)] // Giới hạn độ dài token
        public string Token { get; set; }

        [Required] // Khóa ngoại bắt buộc
        [ForeignKey("User")] // Ràng buộc tới bảng User
        public int UserId { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; } // Thời gian hết hạn của token

        [Required] // Bắt buộc phải có giá trị
        public bool IsRevoked { get; set; } // Đánh dấu token đã bị thu hồi hay chưa

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Thời điểm tạo token

        public DateTime? RevokedAt { get; set; } // Thời điểm token bị thu hồi (có thể null)

        // Liên kết với User
        public virtual User User { get; set; }
    }
}
