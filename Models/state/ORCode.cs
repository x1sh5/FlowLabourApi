namespace FlowLabourApi.Models.state
{
    /// <summary>
    /// 业务操作响应码
    /// </summary>
    public static class ORCode
    {
        /// <summary>
        /// AdminLog添加成功
        /// </summary>
        public static int AdminLogAddS = 01200;

        /// <summary>
        /// AdminLog添加失败
        /// </summary>
        public static int AdminLogAddF = 01400;
        /// <summary>
        /// 任务接取成功
        /// </summary>
        public static int AsgmTakeS = 02200;
        /// <summary>
        /// 任务接取失败,任务已被接取
        /// </summary>
        public static int AsgmHasPicked = 02401;

        /// <summary>
        /// 任务接取失败,任务以不存在
        /// </summary>
        public static int AsgmNotFound = 02402;
    }
}
