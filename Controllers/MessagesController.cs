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

            if (ms.Count() == 0)
            {
                return NotFound();
            }

            return new ActionResult<IEnumerable<Message>>(ms);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiverId"></param>
        /// <param name="count"></param>
        /// <param name="lastid">take items where less than lastid</param>
        /// <returns></returns>
        [HttpGet("receives")]
        public ActionResult<IEnumerable<Message>> Receives([Required]int receiverId,
            [Required]int count, int? lastid)
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var ms = _messageService
                .GetMessages(receiverId, Convert.ToInt32(id), count, lastid);

            if (ms.Count() == 0)
            {
                return NoContent();
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

        /// <summary>
        /// Œ¥∂¡–≈œ¢
        /// </summary>
        /// <returns></returns>
        [HttpGet("unread")]
        public async Task<ActionResult> UnRead()
        {
            var id = User.Claims.FirstOrDefault(User => User.Type == JwtClaimTypes.IdClaim).Value;
            var unreads =await _messageService.LastUnRead(int.Parse(id));
            List<UnReadMsg> messages = new List<UnReadMsg>();
            if(unreads.Count() > 0)
            {
                foreach(IGrouping<int, Message> unread in unreads)
                {
                    messages.Add(new UnReadMsg
                    {
                        Count = unread.Count(),
                        Last = unread.FirstOrDefault()
                    });
                }
                return Ok(messages);
            }
            return NoContent();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public record class UnReadMsg
    {
        /// <summary>
        /// unread count messages
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// last unread message
        /// </summary>
        public Message? Last { get; set; }
    }
}
