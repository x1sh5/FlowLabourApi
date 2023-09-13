namespace FlowLabourApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ReferEdit
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 原有内容
        /// </summary>
        public string Old { get; set; }

        /// <summary>
        /// 改变的内容
        /// </summary>
        public string Change { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ReferId { get; set; }

        /// <summary>
        /// 修改内容
        /// </summary>
        public DateTime EditTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Reference? Reference { get; set; }

    }
}
