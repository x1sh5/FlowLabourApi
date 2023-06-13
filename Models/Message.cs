using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

public partial class Message
{
    public int From { get; set; }

    public int To { get; set; }

    public string? Message1 { get; set; }

    public string? Messagetype { get; set; }

    public DateTime? Date { get; set; }

    public virtual AuthUser FromNavigation { get; set; } = null!;

    public virtual AuthUser ToNavigation { get; set; } = null!;
}
