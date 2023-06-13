namespace FlowLabourApi.ViewModels
{
    public class AssignmentView
    {

        /// <summary>
        /// branch表的id外键
        /// </summary>
        public int Branchid { get; set; }

        public string? Description { get; set; }

        public DateTime Finishtime { get; set; }
        public int Id { get; set; }

        /// <summary>
        /// 单位：分钟
        /// </summary>
        public int? Presumedtime { get; set; }

        /// <summary>
        /// auth_user的id外键
        /// </summary>
        public int Publishid { get; set; }

        public DateTime Publishtime { get; set; }

        public int Reward { get; set; }

        /// <summary>
        /// 1:固定值，单位：分。2：百分比，精度为小数点后两位。
        /// </summary>
        public sbyte Rewardtype { get; set; }

        /// <summary>
        /// 0:代接，1：已结待完成，2：已完成。
        /// </summary>
        public sbyte Status { get; set; }

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