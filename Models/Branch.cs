using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 部门类型表
/// </summary>
public partial class Branch
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
