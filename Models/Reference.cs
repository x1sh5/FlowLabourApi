namespace FlowLabourApi.Models
{
    /// <summary>
    /// 审核区间
    /// </summary>
    public class Reference
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int AuthId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual IEnumerable<ReferEdit>? ReferEdits { get; set; } = new List<ReferEdit>();
    }
}
