using System.ComponentModel.DataAnnotations;

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
        /// 所属部门(类型)，对应branch表的id
        /// </summary>
        /// <example>1</example>
        public int Branchid { get; set; }

        /// <summary>
        /// 详细描述
        /// </summary>
        /// <example>任务的详细描述</example>
        public string? Description { get; set; }

        /// <summary>
        /// 完成日期
        /// </summary>
        public DateTime? Finishtime { get; set; }

        /// <summary>
        /// 截止日期
        /// </summary>
        public DateTime Deadline { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public DateTime Publishtime { get; set; }

        /// <summary>
        /// 固定回报的金额。
        /// </summary>
        /// <example>100</example>
        public int FixedReward { get; set; }

        /// <summary>
        /// 百分比回报的金额。
        /// </summary>
        /// <example>0</example>
        public int? PercentReward { get; set; }

        /// <summary>
        /// 1:固定值，单位：分。2：百分比，精度为小数点后两位。
        /// </summary>
        /// <example>1</example>
        public sbyte Rewardtype { get; set; }

        /// <summary>
        /// 0:代接，1：已结待完成，2：已完成。3:公示。
        /// </summary>
        /// <example>0</example>
        public sbyte Status { get; set; }

        /// <summary>
        /// 主任务。0：不是，1：是。
        /// </summary>
        public sbyte Main { get; set; }

        /// <summary>
        /// 能否被接取，0：不能，1：能。
        /// </summary>
        public sbyte CanTake { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// task的tag
        /// </summary>
        /// <example>1</example>
        public string Tag { get; set; }

        /// <summary>
        /// 0:未审核通过，1：审核通过。
        /// </summary>
        /// <example>1</example>
        public sbyte Verify { get; set; }

        /// <summary>
        /// 是否支付，0:未支付，1：支付。
        /// </summary>
        /// <example>1</example>
        [Required]
        public sbyte Payed { get;  set; }

        /// <summary>
        /// 用户图片
        /// </summary>
        public virtual ICollection<string> Images{ get; set; } = new List<string>();
    }
}