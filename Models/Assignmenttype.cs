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
    public int Id { get; set; }

    /// <summary>
    /// 任务类型名字。
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 任务类型描述。
    /// </summary>
    public string? Description { get; set; }
}
