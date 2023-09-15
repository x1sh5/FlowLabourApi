namespace FlowLabourApi.Models;

/// <summary>
/// 关联任务
/// </summary>
public partial class RelatedAssignment
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }

    public Assignment? Assignment { get; set; }

    public int RelatedId { get; set; }

    /// <summary>
    /// 关联任务
    /// </summary>
    public virtual List<Assignment>? Relates { get; set; } = new List<Assignment>();
}
