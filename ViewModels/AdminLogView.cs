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
        /// <example>114</example>
        public sbyte ActionType { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        /// <example>1919810</example> 
        public int UserId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        /// <example>这是一个描述</example> 
        public string? Describe { get; set; }

        /// <summary>
        /// 记录id
        /// </summary>
        /// <example>810</example> 
        public int Id { get; set; }
    }
}
