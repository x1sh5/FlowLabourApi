using FlowLabourApi.Models;

namespace FlowLabourApi.ViewModels
{
    public class AdminLogView
    {
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public sbyte ActionType { get; set; }

        public int UserId { get; set; }

        public string? Describe { get; set; }
        public int Id { get; set; }
    }
}
