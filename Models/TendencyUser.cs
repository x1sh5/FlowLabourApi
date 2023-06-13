using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

public partial class TendencyUser
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;
}
