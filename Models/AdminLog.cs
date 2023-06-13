using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 管理员登录记录
/// </summary>
public partial class AdminLog
{
    /// <summary>
    /// 操作时间
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    public sbyte ActionType { get; set; }

    public int UserId { get; set; }

    public string? Describe { get; set; }

    public virtual AuthUser User { get; set; } = null!;
    public int Id { get; set; }
}
