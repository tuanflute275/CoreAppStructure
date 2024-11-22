using System.ComponentModel.DataAnnotations;

namespace CoreAppStructure.Features.Users.Models
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string UserFullName { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? OldImage { get; set; }
        [EmailAddress]
        public string UserEmail { get; set; }
        [MinLength(6)]
        public string? UserPassword { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserAddress { get; set; }
        public bool? UserGender { get; set; } = true;
        public int UserActive { get; set; } = 0;
        public List<string> Roles { get; set; } = new List<string>();

        public DateTime? DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? UserBio { get; set; }
        public string? SocialLinks { get; set; }
    }
}
