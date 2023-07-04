using FlowLabourApi.Models.context;
using Microsoft.EntityFrameworkCore;

namespace FlowLabourApi.Models.Services
{
    public class AuthUserService : IDbService<AuthUser>
    {
        private readonly XiangxpContext _context;
        private readonly ILogger<AuthUserService> _logger;
        public AuthUserService(XiangxpContext context, ILogger<AuthUserService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public void Add(AuthUser entity)
        {
            _context.AuthUsers.Add(entity);
            _context.SaveChanges();
        }

        /// <inheritdoc/>
        public async Task AddAsync(AuthUser entity)
        {
            await _context.AuthUsers.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(int id)
        {
            _context.AuthUsers.Remove(_context.AuthUsers.Find(id));
            _context.SaveChanges();
        }

        public async Task<AuthUser> DeleteAsync(int id)
        {
            var e = _context.AuthUsers.Remove(await _context.AuthUsers.FindAsync(id));
            await _context.SaveChangesAsync();
            return e.Entity;
        }

        public AuthUser GetById(int id)
        {
            var e = _context.AuthUsers.Find(id);
            return e;
        }

        public async Task<AuthUser> GetByIdAsync(int id)
        {
            ValueTask<AuthUser?> e = _context.AuthUsers.FindAsync(id);
            return e.Result;
        }

        public AuthUser GetByName(string Name)
        {
            var e = _context.AuthUsers.Where(e => e.UserName == Name).FirstOrDefault();
            return e;
        }

        public async Task<AuthUser> GetByNameAsync(string Name)
        {
            var e = await _context.AuthUsers.FirstOrDefaultAsync(et=>et.UserName==Name);
            return e;
        }

        public void Update(AuthUser entity)
        {
            _context.AuthUsers.Update(entity);
            _context.SaveChanges();
        }

        public async Task UpdateAsync(AuthUser entity)
        {
            _context.AuthUsers.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
