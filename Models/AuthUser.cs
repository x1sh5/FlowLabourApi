using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 用户表
/// </summary>
public partial class AuthUser
{
    public int Id { get; set; }

    public string Passwordhash { get; set; } = null!;

    public String? SecurityStamp { get; set; }

    /// <summary>
    /// 程序页面显示的名字
    /// </summary>
    public string UserName { get; set; } = null!;

    public string PhoneNo { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsActive { get; set; } = false;

    public DateTime DateJoined { get; set; }

    public string? ConcurrencyStamp { get; set; }

    /// <summary>
    /// 多次登录失败后的锁定时间，单位：分钟。
    /// </summary>
    public int LockoutEnd { set; get; } = 0;

    /// <summary>
    /// 锁定状态，1：锁定，0：非锁定。
    /// </summary>
    public sbyte LockoutEnabled { get; set; } = 0;

    public int AccessFailedCount { set; get; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

}
