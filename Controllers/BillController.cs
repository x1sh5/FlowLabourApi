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
        [HttpPost]
        public async Task<IActionResult> MiniProgramPayTask([FromBody] WeChatPayPubPayV3ViewModel viewModel)
        {
            var model = new WeChatPayTransactionsJsApiBodyModel
            {
                AppId = _optionsAccessor.Value.AppId,
                MchId = _optionsAccessor.Value.MchId,
                Amount = new Amount { Total = viewModel.Total, Currency = "CNY" },
                Description = viewModel.Description,
                NotifyUrl = viewModel.NotifyUrl,
                OutTradeNo = viewModel.OutTradeNo,
                Payer = new PayerInfo { OpenId = viewModel.OpenId }
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

        // DELETE api/<BillController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
