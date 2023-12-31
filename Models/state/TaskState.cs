﻿namespace FlowLabourApi.Models.state
{
    /// <summary>
    /// 
    /// </summary>
    public enum TaskState : sbyte
    {
        /// <summary>
        /// 待接状态
        /// </summary>
        WaitForAccept = 0,
        /// <summary>
        /// 未完成状态
        /// </summary>
        Unfinished = 1,
        /// <summary>
        /// 已完成状态
        /// </summary>
        Finished = 2,
        /// <summary>
        /// 公示
        /// </summary>
        Announcement = 3,
        /// <summary>
        /// 任务失败
        /// </summary>
        Failed= 4,
    }
}
