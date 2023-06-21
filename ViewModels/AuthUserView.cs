namespace FlowLabourApi.ViewModels
{
    /// <summary>
    /// 用户信息视图
    /// </summary>
    public class AuthUserView
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 加入日期
        /// </summary>
        public DateTime DateJoined { get; set; }

        /// <summary>
        /// 邮件
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// 电话
        /// </summary>
        public string PhoneNo { get; set; } = null!;

        /// <summary>
        /// 程序页面显示的名字
        /// </summary>
        public string Username { get; set; } = null!;
    }
}