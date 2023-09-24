﻿using System.ComponentModel.DataAnnotations;

namespace FlowLabourApi.ViewModels
{
    public class WeChatPayPubPayV3ViewModel
    {
        [Required]
        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }

        [Required]
        [Display(Name = "description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "total")]
        public int Total { get; set; }

        [Required]
        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }

        [Required]
        [Display(Name = "openid")]
        public string OpenId { get; set; }
    }
}