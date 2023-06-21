using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 用户分组表，对应数据库中的表auth_user_groups
/// </summary>
public partial class AuthUserGroup
{
    /// <summary>
    /// 组唯一标识id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 组名。
    /// </summary>
    public string? Gooupname { get; set; }

    /// <summary>
    /// 组描述。
    /// </summary>
    public string? Groupdescipt { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime Createtime { get; set; }
}
