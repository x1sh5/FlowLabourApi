namespace FlowLabourApi.Models;

public partial class UserRole
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public virtual AuthUser? User { get; set; }

    public virtual Role? Role { get; set; }
}
