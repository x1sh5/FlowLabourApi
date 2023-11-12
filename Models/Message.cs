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
    /// 发信人姓名
    /// </summary>
    public string SenderName { get; set; } = null!;

    /// <summary>
    /// 收信人姓名
    /// </summary>
    public string ReceiverName { get; set; } = null!;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    public string? ContentType { get; set; } = "string";

    /// <summary>
    /// 发信日期
    /// </summary>
    public DateTime? Date { get; set; }

    /// <summary>
    /// 是否已读,1未读，0已读
    /// </summary>
    public sbyte Unread { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual AuthUser? FromNavigation { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual AuthUser? ToNavigation { get; set; }
}
