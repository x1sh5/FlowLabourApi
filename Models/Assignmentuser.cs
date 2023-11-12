namespace FlowLabourApi.Models;

/// <summary>
/// 用户任务接取情况，对应数据库中的表assignmentuser
/// </summary>
public partial class AssignmentUser
{
    /// <summary>
    /// 任务id,对应assignmen表中的id
    /// </summary>
    /// <example>1</example>
    public int AssignmentId { get; set; }

    /// <summary>
    /// 用户id，对应auth_user表中的id。
    /// </summary>
    /// <example>1</example>
    public int UserId { get; set; }

    /// <summary>
    /// 是否归档，yes：是，no:否，归档后不能再对任务进行操作。
    /// </summary>
    public string Archive { get; set; } = "no";

    public DateTime? Date { get; set; } = DateTime.Now;
    public DateTime? ArchiveDate { get; set; }

    /// <summary>
    /// 任务信息
    /// </summary>
    public Assignment? Assignment { get; set; }
    //public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    /// <summary>
    /// 用户信息
    /// </summary>
    public virtual AuthUser? User { get; set; }
 
}
