namespace FlowLabourApi.Models.state
{
    /// <summary>
    /// 返回状态信息
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ResponeMessage<TValue> where TValue : class
    {
        /// <summary>
        /// 业务操作响应码
        /// </summary>
        public int ORCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? Message { get; set; } = "Ok";
        /// <summary>
        /// 返回数据
        /// </summary>
        public TValue? Data { get; set; }
    }
}
