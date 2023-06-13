using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 用户分组表
/// </summary>
public partial class AuthUserGroup
{
    public int Id { get; set; }

    public string? Gooupname { get; set; }

    public string? Groupdescipt { get; set; }

    public DateTime Createtime { get; set; }
}
