using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 关联任务
/// </summary>
public partial class Relatedtask
{
    /// <summary>
    /// 主任务的id
    /// </summary>
    public int? Taskid { get; set; }

    /// <summary>
    /// 相关任务的id
    /// </summary>
    public int? Relatedid { get; set; }
}
