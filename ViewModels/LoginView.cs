﻿namespace FlowLabourApi.ViewModels
{
    public class LoginView
    {
        public string Passwordhash { get; set; } = null!;

        /// <summary>
        /// 程序页面显示的名字
        /// </summary>
        public string Username { get; set; } = null!;
    }
}
