using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 登录历史
/// </summary>
public partial class SigninLog
{
    public int Userid { get; set; }

    public DateTime Lasttime { get; set; }

    public string? Lastiplocation { get; set; }
}
