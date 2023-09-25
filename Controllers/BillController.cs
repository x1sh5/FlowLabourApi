using FlowLabourApi.Models.context;
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
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<BillController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 小程序支付-JSAPI下单
        /// </summary>
        /// <param name="viewModel"></param>
        [HttpPost("pubPayV3")]
        public async Task<IActionResult> MiniProgramPayTask([FromBody] WeChatPayPubPayV3ViewModel viewModel)
        {
            Jscode2Session? j2s;
            using (var client = new HttpClient())
            {
                var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={_optionsAccessor.Value.AppId}&secret={_optionsAccessor.Value.AppSecret}&js_code={viewModel.JsCode}&grant_type=authorization_code";
                var response1 = await client.GetAsync(url);
                var result = await response1.Content.ReadAsStringAsync();
                j2s = JsonConvert.DeserializeObject<Jscode2Session>(result);
            }
            if (j2s == null||j2s.ErrCode==0)
            {
                return BadRequest("调用微信支付失败。");
            }

            var model = new WeChatPayTransactionsJsApiBodyModel
            {
                AppId = _optionsAccessor.Value.AppId,
                MchId = _optionsAccessor.Value.MchId,
                Amount = new Amount { Total = viewModel.Total, Currency = "CNY" },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo,
                Payer = new PayerInfo { OpenId = j2s.OpenId }
            };

            var request = new WeChatPayTransactionsJsApiRequest();
            request.SetBodyModel(model);

            var response = await _client.ExecuteAsync(request, _optionsAccessor.Value);

            if (!response.IsError)
            {
                var req = new WeChatPayMiniProgramSdkRequest
                {
                    Package = "prepay_id=" + response.PrepayId
                };

                var parameter = await _client.ExecuteAsync(req, _optionsAccessor.Value);

                // 将参数(parameter)给 小程序端
                // https://pay.weixin.qq.com/wiki/doc/apiv3/apis/chapter3_5_4.shtml
                return Ok(parameter);
            }
            return BadRequest(response.Body);
        }

        // PUT api/<BillController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// 查询单笔退款
        /// </summary>
        /// <param name="viewModel"></param>
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpPost]
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
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
