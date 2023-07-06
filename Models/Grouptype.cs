namespace FlowLabourApi.Models;

/// <summary>
/// 用户组类型表
/// </summary>
public partial class Grouptype
{
    /// <summary>
    /// 用户组类型id，用于用户组类型的唯一标识
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// 用户组类型
    /// </summary>
    /// <example>管理员组</example>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 描述
    /// </summary>
    /// <example>管理员组</example>
    public string? Description { get; set; }
}
