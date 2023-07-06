namespace FlowLabourApi.Models;

/// <summary>
/// 登录历史
/// </summary>
public partial class SigninLog
{
    public int Userid { get; set; }

    public DateTime Lasttime { get; set; }

    public string? Lastiplocation { get; set; }

    public string LoginProvider { get; set; }

    public string ProviderKey { get; set; }

    public AuthUser AuthUser { get; set; } = null!;
}
