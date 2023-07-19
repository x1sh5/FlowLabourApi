namespace FlowLabourApi.Models
{
    /// <summary>
    /// 图片表，对应数据库中的表images
    /// </summary>
    public class Image
    {
        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string Url { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string Md5 { get; set; } = null!;

        /// <summary>
        /// 任务id
        /// </summary>
        public int? AssignmentId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //public virtual Assignment? Assignment { get; set; }
    }
}
