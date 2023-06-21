using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 赋予user，group相应的权限。对应数据库表role
/// </summary>
public partial class Role
{
    /// <summary>
    /// 唯一标识id
    /// </summary>
    public int Roleid { get; set; }

    /// <summary>
    /// 权限描述，可以进行哪些相关的操作。
    /// </summary>
    public string? Descrpt { get; set; }

    /// <summary>
    /// 权限。(eg. can add, can delete) 
    /// </summary>
    public string Privilege { get; set; } = null!;

}
