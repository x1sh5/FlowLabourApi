using System;
using System.Collections.Generic;

namespace FlowLabourApi.Models;

/// <summary>
/// 任务详细表，对应数据库中的表assignment
/// </summary>
public partial class Assignment
{
    /// <summary>
    /// 记录唯一标识号码
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 所属部门，对应branch表的id
    /// </summary>
    public int Branchid { get; set; }

    /// <summary>
    /// 任务类型的id，对应assignmenttype的id
    /// </summary>
    public int Typeid { get; set; }

    /// <summary>
    /// 发布人id，对应auth_user表中的id
    /// </summary>
    public int Publishid { get; set; }

    /// <summary>
    /// 0:代接，1：已结待完成，2：已完成。
    /// </summary>
    public sbyte Status { get; set; }

    /// <summary>
    /// 预计需要的时间。 单位：分钟
    /// </summary>
    public int? Presumedtime { get; set; }

    /// <summary>
    /// 1:固定值，单位：分。2：百分比，精度为小数点后两位。
    /// </summary>
    public sbyte Rewardtype { get; set; }

    /// <summary>
    /// 回报值。如果rewardtype的值为1，则表示固定回报的金额。
    /// 单位：分。如果rewardtype的值为2，表示百分比回报。
    /// 单位：万分之一，计算公式：值/100 * 100%（如填写1，则实际回报比为 0.01%，填写2000，则实际回报比为 20%）
    /// </summary>
    public int Reward { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime Publishtime { get; set; }

    /// <summary>
    /// 任务具体描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 任务被完成的时间。
    /// </summary>
    public DateTime? Finishtime { get; set; }

    /// <summary>
    /// 0:未审核通过，1：审核通过。
    /// </summary>
    public sbyte Verify { get; set; }

    /// <summary>
    /// 发布人信息
    /// </summary>
    public virtual AuthUser Publish { get; set; } = null!;
}
