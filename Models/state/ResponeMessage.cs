namespace FlowLabourApi.Models.state
{
    /// <summary>
    /// 返回状态信息
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ResponeMessage<TValue> where TValue : class
    {
        /// <summary>
        /// 状态码，等效与http状态码
        /// </summary>
        public int Code { get; set; }
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
