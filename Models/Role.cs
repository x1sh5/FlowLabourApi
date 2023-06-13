using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 赋予user，group相应的权限。
/// </summary>
public partial class Role
{
    public int Roleid { get; set; }

    public string? Descrpt { get; set; }

    public string Privilege { get; set; } = null!;

    public virtual ICollection<AuthUser> Users { get; set; } = new List<AuthUser>();
}
