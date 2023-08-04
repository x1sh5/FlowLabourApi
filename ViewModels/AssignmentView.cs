namespace FlowLabourApi.ViewModels
{
    /// <summary>
    /// 任务详细的前端视图
    /// </summary>
    public class AssignmentView
    {
        /// <summary>
        /// id
        /// </summary>
        /// <example>用户名</example>
        public int Id { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名，对应auth_user表的username
        /// </summary>

        public string? Username { get; set; }
        /// <summary>
        /// 所属部门，对应branch表的id
        /// </summary>
        /// <example>1</example>
        public int Branchid { get; set; }

        /// <summary>
        /// 详细描述
        /// </summary>
        /// <example>任务的详细描述</example>
        public string? Description { get; set; }

        /// <summary>
        /// 预计完成日期
        /// </summary>
        public DateTime Finishtime { get; set; }

        /// <summary>
        /// 单位：分钟
        /// </summary>
        /// <example>30</example>
        public int? Presumedtime { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime Publishtime { get; set; }

        /// <summary>
        /// 回馈值
        /// </summary>
        public int Reward { get; set; }

        /// <summary>
        /// 1:固定值，单位：分。2：百分比，精度为小数点后两位。
        /// </summary>
        public sbyte Rewardtype { get; set; }

        /// <summary>
        /// 0:代接，1：已结待完成，2：已完成。
        /// </summary>
        public sbyte Status { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// tasktype的id外键
        /// </summary>
        /// <example>1</example>
        public int Typeid { get; set; }

        /// <summary>
        /// 0:未审核通过，1：审核通过。
        /// </summary>
        public sbyte Verify { get; set; }

        /// <summary>
        /// 用户图片
        /// </summary>
        public virtual ICollection<string> Images{ get; set; } = new List<string>();
    }
}