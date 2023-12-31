﻿namespace FlowLabourApi.ViewModels
{
    /// <summary>
    /// 注册时传入的数据
    /// </summary>
    public class RegisterView
    {
        /// <summary>
        /// 邮件
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = null!;

        /// <summary>
        /// 电话
        /// </summary>
        public string? PhoneNo { get; set; }

        /// <summary>
        /// 程序页面显示的名字
        /// </summary>
        public string UserName { get; set; } = null!;
    }
}