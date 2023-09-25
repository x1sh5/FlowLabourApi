using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

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

        //[Required]
        //[Display(Name = "openid")]
        //public string OpenId { get; set; }

        [Required]
        [Display(Name = "js_code")]
        public string JsCode { get; set; }
    }

    public class Jscode2Session
    {
        [JsonProperty(PropertyName = "session_key")]
        public string SessionKey { get; set; }

        [JsonProperty(PropertyName = "unionid")]
        public string UnionId { get; set; }

        [JsonProperty(PropertyName = "errmsg")]
        public int Errmsg { get; set; }

        [JsonProperty(PropertyName = "openid")]
        public string OpenId { get; set; }

        [JsonProperty(PropertyName = "errcode")]
        public int ErrCode { get; set; } = int.MaxValue;
    }

    public class WeChatPayQueryByTransactionIdViewModel
    {
        [Required]
        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }
    }

    public class WeChatPayQueryByOutTradeNoViewModel
    {
        [Required]
        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }
    }

    public class WeChatPayOutTradeNoCloseViewModel
    {
        [Required]
        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }
    }

    public class WeChatPayBillDownloadViewModel
    {
        [Required]
        [Display(Name = "download_url")]
        public string DownloadUrl { get; set; }
    }

    public class WeChatPayV3RefundQueryViewModel
    {
        [Display(Name = "refund_id")]
        public string RefundId { get; set; }

        [Display(Name = "out_refund_no")]
        public string OutRefundNo { get; set; }

        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }
    }

    public class WeChatPayV3RefundViewModel
    {
        [Required]
        [Display(Name = "out_refund_no")]
        public string OutRefundNo { get; set; }

        [Display(Name = "transaction_id")]
        public string TransactionId { get; set; }

        [Display(Name = "out_trade_no")]
        public string OutTradeNo { get; set; }

        [Display(Name = "notify_url")]
        public string NotifyUrl { get; set; }

        [Required]
        [Display(Name = "amount.refund")]
        public int RefundAmount { get; set; }

        [Required]
        [Display(Name = "amount.total")]
        public int TotalAmount { get; set; }

        [Required]
        [Display(Name = "currency")]
        public string Currency { get; set; }
    }
}