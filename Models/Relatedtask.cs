using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 关联任务
/// </summary>
public partial class Relatedtask
{
    public int? Taskid { get; set; }

    public int? Relatedid { get; set; }
}
