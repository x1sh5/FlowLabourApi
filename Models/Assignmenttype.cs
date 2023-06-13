using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

public partial class Assignmenttype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
