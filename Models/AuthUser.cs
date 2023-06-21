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
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// 密码经过 哈希 加密后的字符串。
    /// </summary>
    /// <example>8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92</example>
    public string Passwordhash { get; set; } = null!;

    /// <summary>
    /// 上一次登录日期。
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// 小程序页面显示的名字。并非真实姓名。
    /// </summary>
    /// <example>张三</example>
    public string Username { get; set; } = null!;

    /// <summary>
    /// 电话号码
    /// </summary>
    /// <example>12345678901</example>
    public string PhoneNo { get; set; } = null!;

    /// <summary>
    /// 邮件。选填。
    /// </summary>
    /// <example>12345678901@163.com</example>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 是否实名验证。未实名验证的用户只能浏览。
    /// </summary>
    /// <example>true</example>
    public bool IsActive { get; set; } = false;

    /// <summary>
    /// 加入日期。
    /// </summary>
    /// <example>2019-01-01</example>
    public DateTime DateJoined { get; set; }

    /// <summary>
    /// 该用户发布的任务
    /// </summary>
    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

}
