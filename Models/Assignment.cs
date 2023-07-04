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
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    /// <example>任务1</example>
    public string? Title { get; set; }

    /// <summary>
    /// 所属部门，对应branch表的id
    /// </summary>
    /// <example>1</example>
    public int Branchid { get; set; }

    /// <summary>
    /// 任务类型的id，对应assignmenttype的id
    /// </summary>
    /// <example>1</example>
    public int Typeid { get; set; }

    /// <summary>
    /// 0:代接，1：已结待完成，2：已完成。
    /// </summary>
    /// <example>1</example>
    public sbyte Status { get; set; }

    /// <summary>
    /// 预计需要的时间。 单位：分钟
    /// </summary>
    /// <example>60</example>
    public int? Presumedtime { get; set; }

    /// <summary>
    /// 1:固定值，单位：分。2：百分比，精度为小数点后两位。
    /// </summary>
    /// <example>1</example>
    public sbyte Rewardtype { get; set; }

    /// <summary>
    /// 回报值。如果rewardtype的值为1，则表示固定回报的金额。
    /// 单位：分。如果rewardtype的值为2，表示百分比回报。
    /// 单位：万分之一，计算公式：值/100 * 100%（如填写1，则实际回报比为 0.01%，填写2000，则实际回报比为 20%）
    /// </summary>
    /// <example>100</example>
    public int Reward { get; set; }

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
    /// 发布人信息
    /// </summary>
    public virtual AuthUser? Publish { get; set; }
}
