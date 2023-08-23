﻿using FlowLabourApi.Models.context;
using FlowLabourApi.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NuGet.Protocol.Plugins;

namespace FlowLabourApi.Models.Services
{
    public class MessageService
    {
        private readonly XiangxpContext _context;

        public MessageService(XiangxpContext context)
        {
            _context = context;
        }

        public async Task<EntityEntry<Message>> Add(MessageView messageView)
        {
            var m = new Message()
            {
                From = (int)messageView.From!,
                To = messageView.To,
                Content = messageView.Content,
                Date = messageView.Date??DateTime.Now,
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
        /// 
        /// </summary>
        /// <param name="message"></param>
        public async void Update(Message message)
        {
            _context.Entry(message).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
