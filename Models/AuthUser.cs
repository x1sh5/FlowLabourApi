using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 用户表，对应数据库中的表auth_user
/// </summary>
public partial class AuthUser
{
    /// <summary>
    /// 记录唯一标识号码
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 密码经过 哈希 加密后的字符串。
    /// </summary>
    public string Passwordhash { get; set; } = null!;

    /// <summary>
    /// 上一次登录时间。
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// 小程序页面显示的名字。并非真实姓名。
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// 电话号码
    /// </summary>
    public string PhoneNo { get; set; } = null!;

    /// <summary>
    /// 邮件。选填。
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 是否实名验证。未实名验证的用户只能浏览。
    /// </summary>
    public bool IsActive { get; set; } = false;

    /// <summary>
    /// 加入时间。
    /// </summary>
    public DateTime DateJoined { get; set; }

    /// <summary>
    /// 该用户发布的任务
    /// </summary>
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

}
