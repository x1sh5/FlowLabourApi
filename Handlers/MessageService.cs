using FlowLabourApi.Models;
using FlowLabourApi.Models.context;
using Microsoft.EntityFrameworkCore;

namespace FlowLabourApi.Handlers
{
    public class MessageService
    {
        private readonly XiangxpContext _xiangxpContext;

        public MessageService(XiangxpContext xiangxpContext)
        {
            _xiangxpContext = xiangxpContext;
        }

        public async void CreateMessage(Message message)
        {
            await _xiangxpContext.Messages.AddAsync(message);
            await _xiangxpContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetMessages(int senderId)
        {
            var messages = await _xiangxpContext.Messages.Where(x=>x.From==senderId).ToListAsync();
            return messages;
        }

        public async void DeleteMessages(int Id)
        {
            var message = await _xiangxpContext.Messages.FindAsync(Id);
            _xiangxpContext.Messages.Remove(message);
            await _xiangxpContext.SaveChangesAsync();
        }

        public async Task<Message> GetMessage(Message message)
        {
            var message1 = await _xiangxpContext.Messages.FindAsync();
            return message1;
        }
    }
}
