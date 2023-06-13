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

    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// 程序页面显示的名字
    /// </summary>
    public string Username { get; set; } = null!;

    public string PhoneNo { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime DateJoined { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Identityinfo Identity { get; set; }

    public virtual Role Role { get; set; }
}
