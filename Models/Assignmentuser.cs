using System;
using System.Collections.Generic;

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
    public int Assignmentid { get; set; }

    /// <summary>
    /// 用户id，对应auth_user表中的id。
    /// </summary>
    /// <example>1</example>
    public int Userid { get; set; }

    /// <summary>
    /// 任务信息
    /// </summary>
    public virtual Assignment? Assignment { get; set; }
    //public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    /// <summary>
    /// 用户信息
    /// </summary>
    public virtual AuthUser? User { get; set; }
}
