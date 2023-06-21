using FlowLabourApi.Models;

namespace FlowLabourApi.ViewModels
{
    /// <summary>
    /// 管理日志前端视图
    /// </summary>
    public class AdminLogView
    {
        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public sbyte ActionType { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Describe { get; set; }

        /// <summary>
        /// 记录id
        /// </summary>
        public int Id { get; set; }
    }
}
