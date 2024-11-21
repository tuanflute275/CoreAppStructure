using System.ComponentModel.DataAnnotations.Schema;

namespace CoreAppStructure.Features.Users.Enums
{
    public enum AccountStatus
    {
        Active = 0,      // 0: Active    (Mở khóa)
        Locked = 1,      // 1: Locked    (Tạm khóa) 
        Suspended = 2    // 2: Suspended (Cấm)
    }
}
