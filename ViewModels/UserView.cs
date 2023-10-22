namespace FlowLabourApi.ViewModels
{
    public class UserView
    {
        /// <summary>
        /// 记录标识ID
        /// </summary>
        /// <example>2</example>
        public int Id { get; set; }

        /// <summary>
        /// 程序页面显示的名字
        /// </summary>
        /// <example>zhangsan</example>
        public string UserName { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        /// <example>13800138000</example>
        public string PhoneNo { get; set; }

        /// <summary>
        /// 邮件
        /// </summary>
        /// <example>zhangsan@localhost.com</example>
        public string Email { get; set; }

        /// <summary>
        /// 是否实名验证。为实名验证的用户只能浏览。
        /// </summary>
        /// <example>false</example>
        public bool IsActive { get; set; } = false;
        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; } = "";

        /// <summary>
        /// 加入日期
        /// </summary>
        /// <example>2023-06-26 11:00:37</example>
        public DateTime DateJoined { get; set; } = DateTime.Now;

        /// <summary>
        /// access
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// refresh
        /// </summary>
        public string RefreshToken { get; set; } = null!;
    }
}
