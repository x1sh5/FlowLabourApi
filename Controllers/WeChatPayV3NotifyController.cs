﻿using Essensoft.Paylink.WeChatPay.V3.Notify;
using Essensoft.Paylink.WeChatPay.V3;
using Essensoft.Paylink.WeChatPay;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using FlowLabourApi.Models.context;

namespace FlowLabourApi.Controllers
{
    [Route("api/wechatpay/v3/notify")]
    [ApiController]
    public class WeChatPayV3NotifyController : Controller
    {
        private readonly ILogger<WeChatPayV3NotifyController> _logger;
        private readonly IWeChatPayNotifyClient _client;
        private readonly FlowContext _context;
        private readonly IOptions<WeChatPayOptions> _optionsAccessor;

        public WeChatPayV3NotifyController(ILogger<WeChatPayV3NotifyController> logger,
            IWeChatPayNotifyClient client, IOptions<WeChatPayOptions> optionsAccessor,
            FlowContext context)
        {
            _logger = logger;
            _client = client;
            _optionsAccessor = optionsAccessor;
            _context = context;
        }

        /// <summary>
        /// 支付结果通知
        /// </summary>
        [Route("transactions")]
        [HttpPost]
        public async Task<IActionResult> Transactions()
        {
            try
            {
                WeChatPayTransactionsNotify? notify = await _client.ExecuteAsync<WeChatPayTransactionsNotify>(Request, _optionsAccessor.Value);
                if (notify.TradeState == WeChatPayTradeState.Success)
                {
                    _logger.LogInformation("支付结果通知 => OutTradeNo: " + notify.OutTradeNo+ ";TransactionId="+notify.TransactionId);
                    var bill = _context.Bills.FirstOrDefault(b => b.BillNo == notify.OutTradeNo);
                    if (bill != null)
                    {
                        bill.Status = 1;
                        var a = _context.Assignments.FirstOrDefault(a => a.Id == bill.AssignmentId);
                        if (a != null)
                        {
                            a.Payed = 1;
                        }
                        try
                        {
                            _context.Assignments.Update(a);
                            _context.Bills.Update(bill);
                            _context.SaveChanges();
                        }catch(Exception e)
                        {
                            _logger.LogError("更新账单"+notify.OutTradeNo + "失败: " + e.Message);
                        }

                    }
                    return WeChatPayNotifyResult.Success;
                }

                return WeChatPayNotifyResult.Failure;
            }
            catch (WeChatPayException ex)
            {
                _logger.LogWarning("出现异常: " + ex.Message);
                return WeChatPayNotifyResult.Failure;
            }
        }

        /// <summary>
        /// 退款结果通知
        /// </summary>
        [Route("refund")]
        [HttpPost]
        public async Task<IActionResult> Refund()
        {
            try
            {
                WeChatPayRefundDomesticRefundsNotify? notify = await _client.ExecuteAsync<WeChatPayRefundDomesticRefundsNotify>(Request, _optionsAccessor.Value);
                if (notify.RefundStatus == WeChatPayRefundStatus.Success)
                {
                    _logger.LogInformation("退款结果通知 => OutTradeNo: " + notify.OutTradeNo);

                    return WeChatPayNotifyResult.Success;
                }

                return WeChatPayNotifyResult.Failure;
            }
            catch (WeChatPayException ex)
            {
                _logger.LogWarning("出现异常: " + ex.Message);
                return WeChatPayNotifyResult.Failure;
            }
        }

        #region 微信支付分

        /// <summary>
        /// 开启/解除授权服务回调通知
        /// </summary>
        [Route("score/permissions")]
        [HttpPost]
        public async Task<IActionResult> Permissions()
        {
            try
            {
                var notify = await _client.ExecuteAsync<WeChatPayScoreUserOpenOrCloseNotify>(Request, _optionsAccessor.Value);
                if (notify.UserServiceStatus == WeChatPayScoreUserServiceStatus.Opened ||
                    notify.UserServiceStatus == WeChatPayScoreUserServiceStatus.Closed)
                {
                    _logger.LogInformation("开启/解除授权服务回调通知 => " + notify.Body);
                    return WeChatPayNotifyResult.Success;
                }

                return WeChatPayNotifyResult.Failure;
            }
            catch (WeChatPayException ex)
            {
                _logger.LogWarning("出现异常: " + ex.Message);
                return WeChatPayNotifyResult.Failure;
            }
        }

        /// <summary>
        /// 确认订单回调通知
        /// </summary>
        [Route("score/orderconfirm")]
        [HttpPost]
        public async Task<IActionResult> OrderConfirm()
        {
            try
            {
                var notify = await _client.ExecuteAsync<WeChatPayScoreUserConfirmNotify>(Request, _optionsAccessor.Value);
                if (notify.State == WeChatPayServiceOrderState.Doing)
                {
                    _logger.LogInformation("确认订单回调通知 => " + notify.Body);
                    return WeChatPayNotifyResult.Success;
                }

                return WeChatPayNotifyResult.Failure;
            }
            catch (WeChatPayException ex)
            {
                _logger.LogWarning("出现异常: " + ex.Message);
                return WeChatPayNotifyResult.Failure;
            }
        }

        /// <summary>
        /// 订单支付成功回调通知
        /// </summary>
        [Route("score/orderpaid")]
        [HttpPost]
        public async Task<IActionResult> OrderPaid()
        {
            try
            {
                WeChatPayScoreUserPaidNotify? notify = await _client.ExecuteAsync<WeChatPayScoreUserPaidNotify>(Request, _optionsAccessor.Value);
                if (notify.State == WeChatPayServiceOrderState.Done)
                {
                    _logger.LogInformation("订单支付成功回调通知 => " + notify.Body);
                    return WeChatPayNotifyResult.Success;
                }

                return WeChatPayNotifyResult.Failure;
            }
            catch (WeChatPayException ex)
            {
                _logger.LogWarning("出现异常: " + ex.Message);
                return WeChatPayNotifyResult.Failure;
            }
        }

        /// <summary>
        /// 订单确认 或 支付成功 回调通知
        /// </summary>
        [Route("score/confirmorpaid")]
        [HttpPost]
        public async Task<IActionResult> OrderConfirmOrPaid()
        {
            try
            {
                var notify = await _client.ExecuteAsync<WeChatPayScoreUserPaidNotify>(Request, _optionsAccessor.Value);
                if (notify.State == WeChatPayServiceOrderState.Doing || notify.State == WeChatPayServiceOrderState.Done)
                {
                    _logger.LogInformation("订单确认或支付成功回调通知: " + notify.Body);
                    return WeChatPayNotifyResult.Success;
                }

                return WeChatPayNotifyResult.Failure;
            }
            catch (WeChatPayException ex)
            {
                _logger.LogWarning("出现异常: " + ex.Message);
                return WeChatPayNotifyResult.Failure;
            }
        }

        #endregion
    }
}
