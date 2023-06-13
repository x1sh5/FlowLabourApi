using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 用户身份信息表
/// </summary>
public partial class Identityinfo
{
    public int Id { get; set; }

    public string Realname { get; set; } = null!;

    public string IdentityNo { get; set; } = null!;

    public sbyte Checked { get; set; }

    public sbyte? Age { get; set; }

    public DateTime Checkeddate { get; set; }

    public virtual AuthUser User { get; set; }
}
