using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

public partial class Message
{
    /// <summary>
    /// id
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// 发信人ID
    /// </summary>
    public int From { get; set; }

    /// <summary>
    /// 收信人ID
    /// </summary>
    public int To { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// 发信日期
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual AuthUser FromNavigation { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    public virtual AuthUser ToNavigation { get; set; } = null!;
}
