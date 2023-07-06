namespace FlowLabourApi.Models;

/// <summary>
/// 用户分组表，对应数据库中的表auth_user_groups
/// </summary>
public partial class AuthUserGroup
{
    /// <summary>
    /// 组唯一标识id
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// 组名。
    /// </summary>
    /// <example>技术</example>
    public string? Gooupname { get; set; }

    /// <summary>
    /// 组描述。
    /// </summary>
    /// <example>技术</example>
    public string? Groupdescipt { get; set; }

    /// <summary>
    /// 创建日期
    /// </summary>
    public DateTime Createtime { get; set; }
}
