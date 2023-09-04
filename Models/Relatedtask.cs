namespace FlowLabourApi.Models;

/// <summary>
/// 关联任务
/// </summary>
public partial class Relatedtask
{
    /// <summary>
    /// 主任务的id
    /// </summary>
    public int? TaskId { get; set; }

    /// <summary>
    /// 相关任务的id
    /// </summary>
    public int? RelatedId { get; set; }
}
