namespace FlowLabourApi.Models;

public partial class Groupuser
{
    public int Groupid { get; set; }

    public int Userid { get; set; }

    public virtual AuthUserGroup Group { get; set; } = null!;

    public virtual AuthUser User { get; set; } = null!;
}
