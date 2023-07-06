namespace FlowLabourApi.Models;

public partial class VideoInfo
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string SharedUrl { get; set; } = null!;

    public string? Text { get; set; }
}
