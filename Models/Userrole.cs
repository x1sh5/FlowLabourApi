using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

public partial class Userrole
{
    public int Userid { get; set; }

    public int Roleid { get; set; }

    public virtual AuthUser User { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
