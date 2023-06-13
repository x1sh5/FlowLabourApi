using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 任务接取情况
/// </summary>
public partial class Assignmentuser
{
    public int Assignmentid { get; set; }

    public int Userid { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

    public virtual AuthUser User { get; set; } = null!;
}
