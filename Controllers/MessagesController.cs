using FlowLabourApi.Config;
using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using FlowLabourApi.Models.Services;
using FlowLabourApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FlowLabourApi.Controllers
{
    [Route("/api/messages")]
    [ApiController]
    [Authorize]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class MessagesController : ControllerBase
    {
        //private readonly XiangxpContext _context;
        private readonly MessageService _messageService;

        public MessagesController(MessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Message>> GetMessages()
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var ms =  _messageService.GetMessages(Convert.ToInt32(id));
            return Ok(ms);
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Message>> GetMessage(int senderId)
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var ms = _messageService.GetMessages(Convert.ToInt32(id));

            if (ms.Count() != 0)
            {
                return NotFound();
            }

            return new ActionResult<IEnumerable<Message>>(ms);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reciverId"></param>
        /// <param name="count"></param>
        /// <param name="lastid">take items where less than lastid</param>
        /// <returns></returns>
        [HttpGet("receives")]
        public ActionResult<IEnumerable<Message>> Receives([Required]int reciverId,
            [Required]int count, int? lastid)
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var ms = _messageService
                .GetMessages(Convert.ToInt32(id), reciverId, count, lastid);

            if (ms.Count() == 0)
            {
                return NotFound();
            }

            return new ActionResult<IEnumerable<Message>>(ms);
        }

        [HttpPost]
        public async Task<ActionResult<Message>> CreateMessage(MessageView message)
        {
            var e = await _messageService.Add(message);

            return CreatedAtAction(nameof(GetMessage), /* new { id = message.From }, */ e.Entity);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMessage(int senderId, Message message)
        {
            var m = _messageService.GetMessage(senderId);
            if (senderId != message.From)
            {
                return BadRequest();
            }

            _messageService.Update(message);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMessage(int Id)
        {
            var message = _messageService.GetMessage(Id);

            if (message == null)
            {
                return NotFound();
            }

            _messageService.Delete(Id);

            return NoContent();
        }
    }
}
