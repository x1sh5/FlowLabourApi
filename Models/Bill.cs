namespace FlowLabourApi.Models
{
    public class Bill
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 主任务ID
        /// </summary>
        public int AssignmentId { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 是否已经结算
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 账单号
        /// </summary>
        public string BillNo { get; set; } = null!;
        /// <summary>
        /// 金额
        /// </summary>
        public int Mount { get; set; }

        public string WeChatBillNo { get; set; } = null!;

        public string Description { get; set; } = null!;
    }
}
