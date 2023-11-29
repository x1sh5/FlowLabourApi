namespace FlowLabourApi.Models
{
    /// <summary>
    /// 任务申请
    /// </summary>
    public class TaskRequest
    {
        public int Id { get; set; }
        /// <summary>
        /// 申请人ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 任务卡ID
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// 是否同意。0：不同意，1： 同意，2：未读。
        /// </summary>
        public sbyte Agree { get; set; }
        /// <summary>
        /// 任务卡标题  
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 申请人名称
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// 部门类型
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime RequestDate { get; set; }

        /// <summary>
        /// 同意日期
        /// </summary>
        public DateTime AgreeDate { get; set; }

        /// <summary>
        /// 留言
        /// </summary>
        public string? Comment { get; set; }

        public override string ToString()
        {
            return $"{{UserId:{UserId},TaskId:{TaskId},Agree:{Agree},RequestDate:" +
                $"{RequestDate},AgreeDate:{AgreeDate},Comment:{Comment}}}";
        }
    }
}
