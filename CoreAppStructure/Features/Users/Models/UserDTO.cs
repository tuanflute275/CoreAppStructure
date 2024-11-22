using CoreAppStructure.Core.Helpers;

namespace CoreAppStructure.Features.Users.Models
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public string? UserAvatar { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserAddress { get; set; }
        public bool? UserGender { get; set; } = true;
        public int UserActive { get; set; } = 0;
        public int? FailedLoginAttempts { get; set; } = 0;
        public DateTime? UserCurrentTime { get; set; }
        public DateTime? UserUnlockTime { get; set; }
        public string? SecurityStamp { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiry { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? UserBio { get; set; }
        public string? SocialLinks { get; set; }

        // Thông tin theo dõi tạo, cập nhật, xóa
        public string? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? DeleteBy { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string? DeleteFlag { get; set; } // Y or N
        public List<string>? RoleName { get; set; }
    }
}
