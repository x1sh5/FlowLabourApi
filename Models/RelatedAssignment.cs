namespace FlowLabourApi.Models;

/// <summary>
/// 关联任务
/// </summary>
public partial class RelatedAssignment
{
    public int Id { get; set; }

    public int AssignmentId { get; set; }

    public int RelatedId { get; set; }

}
