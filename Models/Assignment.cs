namespace FlowLabourApi.Models;

/// <summary>
/// 任务详细表，对应数据库中的表assignment
/// </summary>
public partial class Assignment
{
    /// <summary>
    /// 记录唯一标识号码
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    /// <example>任务1</example>
    public string Title { get; set; }

    /// <summary>
    /// 所属部门(类型)，对应branch表的id
    /// </summary>
    /// <example>1</example>
    public int Branchid { get; set; }

    /// <summary>
    /// 任务类型的tag
    /// </summary>
    /// <example>1</example>
    public string Tag { get; set; }

    /// <summary>
    /// 0:待接, 1:未完成, 2:已完成, 3:公示, 4:失败
    /// </summary>
    /// <example>1</example>
    public sbyte Status { get; set; }

    /// <summary>
    /// 主任务
    /// </summary>
    public sbyte Main { get; set; }

    /// <summary>
    /// 截止日期
    /// </summary>
    public DateTime Deadline { get;set;}

    /// <summary>
    /// 1:固定值，单位：分。2：百分比，精度为小数点后两位。
    /// </summary>
    /// <example>1</example>
    public sbyte Rewardtype { get; set; }

    /// <summary>
    /// 固定回报的金额。
    /// </summary>
    /// <example>100</example>
    public int FixedReward { get; set; }

    /// <summary>
    /// 百分比回报的金额。
    /// </summary>
    public int? PercentReward { get; set; }

    /// <summary>
    /// 发布日期
    /// </summary>
    /// <example>2019-01-01 12:23:12</example>
    public DateTime Publishtime { get; set; }

    /// <summary>
    /// 任务具体描述
    /// </summary>
    /// <example>任务1的描述</example>
    public string? Description { get; set; }

    /// <summary>
    /// 任务被完成的日期。
    /// </summary>
    /// <example>2019-01-04 13:23:12</example>
    public DateTime? Finishtime { get; set; }

    /// <summary>
    /// 0:未审核通过，1：审核通过。
    /// </summary>
    /// <example>1</example>
    public sbyte Verify { get; set; }

    /// <summary>
    /// 是否支付，0:未支付，1：支付。
    /// </summary>
    /// <example>1</example>
    public sbyte Payed { get; set; } = 0;

    /// <summary>
    /// 能否被接取，0：不能，1：能。
    /// </summary>
    public sbyte CanTake { get; set; }

    /// <summary>
    /// 发布人id
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 发布人信息
    /// </summary> 
    public virtual AuthUser? AuthUser { get; set; }

    /// <summary>
    /// 任务领取信息
    /// </summary>
    public virtual AssignmentUser? AssignmentUser { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual List<History> AsgHistories { get; set; } = new List<History>();

    //public virtual List<RelatedAssignment>? Relates { get; set; } = new List<RelatedAssignment>();

}
