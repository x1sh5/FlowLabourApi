using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 部门类型表,对应数据库中的表branch
/// </summary>
public partial class Branch
{
    /// <summary>
    /// 部门id，用于部门的唯一标识
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    /// <example>技术</example>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 简要描述，可以不填
    /// </summary>
    /// <example>技术</example>
    public string? Description { get; set; } = null;
}
