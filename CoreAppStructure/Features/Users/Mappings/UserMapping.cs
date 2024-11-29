namespace CoreAppStructure.Features.Users.Mappings
{
    public class UserMapping : AutoMapper.Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.UserFullName,             opt => opt.MapFrom(src => src.UserFullName))  // Ánh xạ UserFullName
                .ForMember(dest => dest.RoleName,                 opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.RoleName).ToList()))
                .ForMember(dest => dest.UserActive,               opt => opt.MapFrom(src => src.UserActive))
                .ForMember(dest => dest.FailedLoginAttempts,      opt => opt.MapFrom(src => src.FailedLoginAttempts))
                .ForMember(dest => dest.UserCurrentTime,          opt => opt.MapFrom(src => src.UserCurrentTime))
                .ForMember(dest => dest.UserUnlockTime,           opt => opt.MapFrom(src => src.UserUnlockTime))
                .ForMember(dest => dest.SecurityStamp,            opt => opt.MapFrom(src => src.SecurityStamp))
                .ForMember(dest => dest.ResetPasswordToken,       opt => opt.MapFrom(src => src.ResetPasswordToken))
                .ForMember(dest => dest.ResetPasswordTokenExpiry, opt => opt.MapFrom(src => src.ResetPasswordTokenExpiry))
                .ForMember(dest => dest.LastLoginDate,            opt => opt.MapFrom(src => src.LastLoginDate))
                .ForMember(dest => dest.DateOfBirth,              opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.PlaceOfBirth,             opt => opt.MapFrom(src => src.PlaceOfBirth))
                .ForMember(dest => dest.Nationality,              opt => opt.MapFrom(src => src.Nationality))
                .ForMember(dest => dest.UserBio,                  opt => opt.MapFrom(src => src.UserBio))
                .ForMember(dest => dest.SocialLinks,              opt => opt.MapFrom(src => src.SocialLinks))
                .ForMember(dest => dest.CreateBy,                 opt => opt.MapFrom(src => src.CreateBy))
                .ForMember(dest => dest.CreateDate,               opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.UpdateBy,                 opt => opt.MapFrom(src => src.UpdateBy))
                .ForMember(dest => dest.UpdateDate,               opt => opt.MapFrom(src => src.UpdateDate))
                .ForMember(dest => dest.DeleteBy,                 opt => opt.MapFrom(src => src.DeleteBy))
                .ForMember(dest => dest.DeleteDate,               opt => opt.MapFrom(src => src.DeleteDate))
                .ForMember(dest => dest.DeleteFlag,               opt => opt.MapFrom(src => src.DeleteFlag));
        }
    }
}
