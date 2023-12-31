﻿using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Essensoft.Paylink.WeChatPay.V3;
using Essensoft.Paylink.Security;
using Essensoft.Paylink.WeChatPay.V3.Request;
using Essensoft.Paylink.WeChatPay;
using Microsoft.Extensions.Options;
using Essensoft.Paylink.WeChatPay.V3.Domain;
using MySqlX.XDevAPI;
using FlowLabourApi.ViewModels;
using Newtonsoft.Json;
using Essensoft.Paylink.WeChatPay.V3.Response;
using FlowLabourApi.Config;
using FlowLabourApi.Models;
using System.Drawing;
using System.ComponentModel.DataAnnotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlowLabourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BillController : ControllerBase
    {
        private readonly FlowContext _context;
        private readonly ILogger<BillController> _logger;
        private readonly IWeChatPayClient _client;
        private readonly IOptions<WeChatPayOptions> _optionsAccessor;

        public BillController(FlowContext context, ILogger<BillController> logger,
            IOptions<WeChatPayOptions> optionsAccessor, IWeChatPayClient client)
        {
            _context = context;
            _logger = logger;
            _optionsAccessor = optionsAccessor;
            _client = client;
        }

        // GET: api/<BillController>
        [HttpGet]
        public IEnumerable<Bill> Get([Required] uint count, [Required] int offset)
        {
            var userid = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.IdClaim)?.Value;
            return _context.Bills
                .Where(x=>x.UserId==int.Parse(userid)&&x.Id>=offset)
                .Take((int)count).ToList();
        }

        // GET api/<BillController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 获取微信小程序openid
        /// </summary>
        /// <param name="code">登录时获取的 code，可通过wx.login获取</param>
        /// <returns></returns>
        [HttpGet("openId")]
        public async Task<ActionResult> GetOpenId(string code)
        {
            Jscode2Session? j2s;
            using (var client = new HttpClient())
            {
                var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={_optionsAccessor.Value.AppId}&secret={_optionsAccessor.Value.AppSecret}&js_code={code}&grant_type=authorization_code";
                var response1 = await client.GetAsync(url);
                var result = await response1.Content.ReadAsStringAsync();
                j2s = JsonConvert.DeserializeObject<Jscode2Session>(result);
            }
            if (j2s == null || j2s.ErrCode != int.MaxValue)
            {
                return BadRequest("调用失败。");
            }
            return Ok(j2s.OpenId);
        }

        /// <summary>
        /// 小程序支付-JSAPI下单
        /// </summary>
        /// <param name="viewModel"></param>
        [HttpPost("pubPayV3")]
        public async Task<IActionResult> MiniProgramPayTask([FromBody] FlowWeChatBill viewModel)
        {
            var AssignmentId = viewModel.Attach.Replace("taskid=", "");
            
            var valid = int.TryParse(AssignmentId, out int assignmentId);
            if (!valid)
            {
                return BadRequest("任务信息有误。");
            }
            var obill = _context.Bills.SingleOrDefault(x=>x.AssignmentId==assignmentId);
            if (obill != null)
            {
                if(DateTime.Now - obill.Date < TimeSpan.FromMinutes(10))
                {
                    return BadRequest("订单已创建，请勿重复创建。订单状态可能会有延迟，请稍后刷新试试");
                }
                
            }
            var userid = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.IdClaim)?.Value;
            Jscode2Session? j2s;
            using (var client = new HttpClient())
            {
                var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={_optionsAccessor.Value.AppId}&secret={_optionsAccessor.Value.AppSecret}&js_code={viewModel.JsCode}&grant_type=authorization_code";
                var response1 = await client.GetAsync(url);
                var result = await response1.Content.ReadAsStringAsync();
                j2s = JsonConvert.DeserializeObject<Jscode2Session>(result);
            }
            if (j2s == null||j2s.ErrCode!=int.MaxValue)
            {
                return BadRequest("调用微信支付失败。");
            }

            WeChatPayTransactionsJsApiBodyModel? model = new WeChatPayTransactionsJsApiBodyModel
            {
                AppId = _optionsAccessor.Value.AppId,
                MchId = _optionsAccessor.Value.MchId,
                Amount = new Amount { Total = viewModel.Total, Currency = "CNY" },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = Guid.NewGuid().ToString().Replace("-", ""),
                Payer = new PayerInfo { OpenId = j2s.OpenId },
                Attach = viewModel.Attach,
            };

            WeChatPayTransactionsJsApiRequest? request = new WeChatPayTransactionsJsApiRequest();
            request.SetBodyModel(model);

            WeChatPayTransactionsJsApiResponse? response = await _client.ExecuteAsync(request, _optionsAccessor.Value);

            if (!response.IsError)
            {
                var req = new WeChatPayMiniProgramSdkRequest
                {
                    Package = "prepay_id=" + response.PrepayId
                };

                WeChatPayDictionary? parameter = await _client.ExecuteAsync(req, _optionsAccessor.Value);

                if (obill == null)
                {
                    var bill = new Bill
                    {
                        UserId = int.Parse(userid),
                        AssignmentId = assignmentId,
                        Date = DateTime.Now,
                        Status = 0,
                        BillNo = model.OutTradeNo,
                        Mount = viewModel.Total,
                        WeChatBillNo = response.PrepayId,
                        Description = viewModel.Description
                    };
                    try
                    {
                        _context.Add(bill);
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("创建账单失败: " + ex.Message);
                        return BadRequest("调用微信支付失败。");
                    }
                }
                else
                {
                    obill.Date = DateTime.Now;
                    obill.Description = viewModel.Description;
                    obill.BillNo = model.OutTradeNo;
                    obill.Mount = viewModel.Total;
                    obill.WeChatBillNo = response.PrepayId;
                    obill.UserId = int.Parse(userid);
                    obill.AssignmentId = assignmentId;
                    try
                    {
                        _context.Update(obill);
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("创建账单失败: " + ex.Message);
                        return BadRequest("调用微信支付失败。");
                    }
                }



                // 将参数(parameter)给 小程序端
                // https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter3_5_4.shtml
                return Ok(parameter);
            }
            return BadRequest(response.Body);
        }

        /// <summary>
        /// 修改账单状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// 查询单笔退款
        /// </summary>
        /// <param name="viewModel"></param>
        [HttpPost("RefundQuery")]
        public async Task<IActionResult> RefundQuery(WeChatPayV3RefundQueryViewModel viewModel)
        {
            var request = new WeChatPayRefundDomesticRefundsOutRefundNoRequest
            {
                OutRefundNo = viewModel.OutRefundNo
            };

            var response = await _client.ExecuteAsync(request, _optionsAccessor.Value);

            
            return Ok(response.Body);
        }

        /// <summary>
        /// 退款申请
        /// </summary>
        /// <param name="viewModel"></param>
        [HttpPost("Refund")]
        public async Task<IActionResult> Refund(WeChatPayV3RefundViewModel viewModel)
        {
            var model = new WeChatPayRefundDomesticRefundsBodyModel()
            {
                TransactionId = viewModel.TransactionId,
                OutTradeNo = viewModel.OutTradeNo,
                OutRefundNo = viewModel.OutRefundNo,
                NotifyUrl = viewModel.NotifyUrl,
                Amount = new RefundAmount { Refund = viewModel.RefundAmount, Total = viewModel.TotalAmount, Currency = viewModel.Currency }
            };

            var request = new WeChatPayRefundDomesticRefundsRequest();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, _optionsAccessor.Value);

            return Ok(response.Body);
        }

        /// <summary>
        /// 下载账单
        /// </summary>
        /// <param name="viewModel"></param>
        [HttpPost("BillDownload")]
        public async Task<IActionResult> BillDownload(WeChatPayBillDownloadViewModel viewModel)
        {
            var request = new WeChatPayBillDownloadRequest();
            request.SetRequestUrl(viewModel.DownloadUrl);

            var response = await _client.ExecuteAsync(request, _optionsAccessor.Value);

            return Ok(response.Body);
        }

        /// <summary>
        /// 关闭订单
        /// </summary>
        /// <param name="viewModel"></param>
        [HttpPost("OutTradeNoClose")]
        public async Task<IActionResult> OutTradeNoClose(WeChatPayOutTradeNoCloseViewModel viewModel)
        {
            var model = new WeChatPayTransactionsOutTradeNoCloseBodyModel
            {
                MchId = _optionsAccessor.Value.MchId,
            };

            var request = new WeChatPayTransactionsOutTradeNoCloseRequest
            {
                OutTradeNo = viewModel.OutTradeNo,
            };

            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, _optionsAccessor.Value);

            return Ok(response.Body);
        }

        /// <summary>
        /// 商户订单号查询
        /// </summary>
        /// <param name="viewModel"></param>
        [HttpPost("QueryByOutTradeNo")]
        public async Task<IActionResult> QueryByOutTradeNo(WeChatPayQueryByOutTradeNoViewModel viewModel)
        {
            var model = new WeChatPayTransactionsOutTradeNoQueryModel
            {
                MchId = _optionsAccessor.Value.MchId,
            };

            var request = new WeChatPayTransactionsOutTradeNoRequest
            {
                OutTradeNo = viewModel.OutTradeNo,
            };

            request.SetQueryModel(model);

            var response = await _client.ExecuteAsync(request, _optionsAccessor.Value);

            return Ok(response.Body);
        }

        /// <summary>
        /// 微信支付订单号查询
        /// </summary>
        /// <param name="viewModel"></param>
        [HttpPost("QueryByTransactionId")]
        public async Task<IActionResult> QueryByTransactionId(WeChatPayQueryByTransactionIdViewModel viewModel)
        {
            var model = new WeChatPayTransactionsIdQueryModel
            {
                MchId = _optionsAccessor.Value.MchId,
            };

            var request = new WeChatPayTransactionsIdRequest
            {
                TransactionId = viewModel.TransactionId
            };

            request.SetQueryModel(model);

            var response = await _client.ExecuteAsync(request, _optionsAccessor.Value);

            return Ok(response.Body);
        }

        // DELETE api/<BillController>/5
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
