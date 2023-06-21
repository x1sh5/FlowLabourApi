namespace FlowLabourApi.ViewModels
{
    /// <summary>
    /// 前端登录时传入的数据
    /// </summary>
    public class LoginView
    {
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// 程序页面显示的名字
        /// </summary>
        public string Username { get; set; } = null!;
    }
}
