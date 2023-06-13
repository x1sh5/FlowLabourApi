using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// --not use
/// </summary>
public partial class ContentType
{
    public int Id { get; set; }

    public string AppLabel { get; set; } = null!;

    public string Model { get; set; } = null!;
}
