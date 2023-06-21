using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 
/// </summary>
public partial class Grouprole
{
    public int Groupid { get; set; }

    public int Roleid { get; set; }

    public virtual AuthUserGroup Group { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
