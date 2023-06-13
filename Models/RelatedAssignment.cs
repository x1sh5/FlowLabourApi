using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 关联任务
/// </summary>
public partial class RelatedAssignment
{
    public int? AssignmentId { get; set; }

    public Assignment Assignment { get; set; }

    public int? RelatedId { get; set; }

    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
