using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.AspNetCore.Mvc;

namespace FlowLabourApi.Controllers
{
    [Route("/api/messages")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MessagesController : ControllerBase
    {
        private readonly XiangxpContext _context;

        public MessagesController(XiangxpContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Message>> GetMessages()
        {
            return _context.Messages.ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Message>> GetMessage(int senderId)
        {
            var messages = _context.Messages.Where(m => m.From == senderId).ToList();

            if (messages.Count != 0)
            {
                return NotFound();
            }

            return messages;
        }

        [HttpPost]
        public ActionResult<Message> CreateMessage(Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetMessage), new { id = message.From }, message);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateMessage(int senderId, Message message)
        {
            if (senderId != message.From)
            {
                return BadRequest();
            }

            _context.Entry(message).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMessage(int senderId)
        {
            var message = _context.Messages.FirstOrDefault(m => m.From == senderId);

            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
