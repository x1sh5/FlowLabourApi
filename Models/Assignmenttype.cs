using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 任务类型，对应数据库中的表assignmenttype 
/// </summary>
public partial class Assignmenttype
{
    /// <summary>
    /// 记录唯一标识号码
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// 任务类型名字。
    /// </summary>
    /// <example>技术</example>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 任务类型描述。
    /// </summary>
    /// <example>技术</example>
    public string? Description { get; set; }
}
