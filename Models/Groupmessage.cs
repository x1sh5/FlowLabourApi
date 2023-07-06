namespace FlowLabourApi.Models;

public partial class Groupmessage
{
    public int From { get; set; }

    public int To { get; set; }

    public string Message { get; set; } = null!;

    public string? Messagetype { get; set; }

    public DateTime? Date { get; set; }

    public virtual AuthUser FromNavigation { get; set; } = null!;

    public virtual AuthUserGroup ToNavigation { get; set; } = null!;
}
