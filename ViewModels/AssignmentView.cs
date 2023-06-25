namespace FlowLabourApi.ViewModels
{
    /// <summary>
    /// 任务详细的前端视图
    /// </summary>
    public class AssignmentView
    {

        /// <summary>
        /// 所属部门，对应branch表的id
        /// </summary>
        public int Branchid { get; set; }

        /// <summary>
        /// 详细描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 预计完成日期
        /// </summary>
        public DateTime Finishtime { get; set; }

        /// <summary>
        /// id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 单位：分钟
        /// </summary>
        public int? Presumedtime { get; set; }

        /// <summary>
        /// 发布人id，对应auth_user表中的id
        /// </summary>
        public int Publishid { get; set; }

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
        public int Typeid { get; set; }

        /// <summary>
        /// 0:未审核通过，1：审核通过。
        /// </summary>
        public sbyte Verify { get; set; }
    }
}