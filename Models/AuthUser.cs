using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 用户表
/// </summary>
public partial class AuthUser
{
    /// <summary>
    /// 记录标识ID
    /// </summary>
    /// <example>2</example>
    public int Id { get; set; }

    /// <summary>
    /// 密码哈希
    /// </summary>
    public string Passwordhash { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public String? SecurityStamp { get; set; }

    /// <summary>
    /// 程序页面显示的名字
    /// </summary>
    /// <example>zhangsan</example>
    public string UserName { get; set; }

    /// <summary>
    /// 电话号码
    /// </summary>
    /// <example>13800138000</example>
    public string PhoneNo { get; set; }

    /// <summary>
    /// 邮件
    /// </summary>
    /// <example>zhangsan@localhost.com</example>
    public string Email { get; set; }

    /// <summary>
    /// 是否实名验证。为实名验证的用户只能浏览。
    /// </summary>
    /// <example>false</example>
    public bool IsActive { get; set; } = false;

    /// <summary>
    /// 加入日期
    /// </summary>
    /// <example>2023-06-26 11:00:37</example>
    public DateTime DateJoined { get; set; } = DateTime.Now;

    /// <summary>
    /// 
    /// </summary>
    public string? ConcurrencyStamp { get; set; }

    /// <summary>
    /// 多次登录失败后的锁定时间，单位：分钟。
    /// </summary>
    public int LockoutEnd { set; get; } = 0;

    /// <summary>
    /// 锁定状态，1：锁定，0：非锁定。
    /// </summary>
    public sbyte LockoutEnabled { get; set; } = 0;

    /// <summary>
    /// 失败次数
    /// </summary>
    public int AccessFailedCount { set; get; } = 0;

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

}
