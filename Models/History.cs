namespace FlowLabourApi.Models
{
    /// <summary>
    /// 浏览历史
    /// </summary>
    public class History
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 任务id
        /// </summary>
        public int AssigmentId { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 任务
        /// </summary>
        public virtual Assignment Assignment { get; set; } = null!;
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
    }
}
