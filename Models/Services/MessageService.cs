using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NuGet.Protocol.Plugins;
using System.Linq.Expressions;

namespace FlowLabourApi.Models.Services
{
    public class MessageService
    {
        private readonly XiangxpContext _context;

        public MessageService(XiangxpContext context)
        {
            _context = context;
        }

        public AuthUser? HasUser(int userId)
        {
            var u = _context.AuthUsers.Find(userId);
            return u;
        }

        public async Task<EntityEntry<Message>> Add(MessageView messageView)
        {
            var m = new Message()
            {
                From = (int)messageView.From!,
                To = messageView.To,
                Content = messageView.Content,
                Date = messageView.Date,
                ContentType = messageView.ContentType
            };
            EntityEntry<Message>? e = _context.Messages.Add(m);
            await _context.SaveChangesAsync();
            return e;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Message>> GetMessages()
        {
            var messages = await _context.Messages.ToListAsync();
            return messages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns></returns>
        public IEnumerable<Message> GetMessages(int senderId)
        {
            var messages = _context.Messages.Where(m => m.From == senderId).ToList();
            return messages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="senderId"></param>
        public async void Delete(int senderId)
        {
            var message = _context.Messages.FirstOrDefault(m => m.From == senderId);

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
        }

        public Message GetMessage(int Id)
        {
            var message = _context.Messages.FirstOrDefault(m => m.Id == Id);
            return message;
        }

        /// <summary>
        /// 最近接收的消息
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="receiverId"></param>
        /// <param name="count"></param>
        /// <param name="lastid">take messages less than lastid</param>
        /// <returns></returns>
        public IEnumerable<Message> GetMessages(int senderId, int receiverId, int count, int? lastid)
        {
            Expression<Func<Message, bool>> expression;
            if(lastid==null)
            {
                expression = m => m.From == senderId && m.To == receiverId;
            }
            else
            {
                expression = m => m.From == senderId && m.To == receiverId && m.Id < lastid;
            }
            var message = _context.Messages
                .Where(expression)
                .Take(count).OrderBy(m=>m.Date);
            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public async void Update(Message message)
        {
            _context.Entry(message).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<List<IGrouping<int,Message>>> LastUnRead(int userId)
        {
            var l = await _context.Messages
                .Where(o=>o.To == userId&&o.Unread == 1)
                .OrderByDescending(o=>o.Date)
                .GroupBy(o=>o.From)
                .ToListAsync();
           
           return l;
        }
    }
}
