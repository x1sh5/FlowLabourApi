namespace FlowLabourApi.Models;

/// <summary>
/// 管理员登录记录,对应数据库中的表admin_log
/// </summary>
public partial class AdminLog
{
    /// <summary>
    /// 操作日期
    /// </summary>
    /// example: 2019-01-01
    public DateTime Time { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    /// example: 1
    public sbyte ActionType { get; set; }

    /// <summary>
    /// 用户id,对应auth_user表中的id
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 补充说明。
    /// </summary>
    public string? Describe { get; set; }

    /// <summary>
    /// 记录唯一标识号码
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 所属用户
    /// </summary>
    public virtual AuthUser User { get; set; } = null!;
}
