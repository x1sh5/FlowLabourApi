namespace FlowLabourApi.ViewModels
{
    /// <summary>
    /// 前端登录时传入的数据
    /// </summary>
    public class LoginView
    {
        /// <summary>
        /// 程序页面显示的名字
        /// </summary>
        /// <example>zhangsan</example>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// 密码
        /// </summary>
        /// <example>zhangsan1234</example>
        public string Password { get; set; } = null!;

        /// <summary>
        /// 微信小程序的openid
        /// </summary>
        public string? OpenId { get; set; }
    }
}
