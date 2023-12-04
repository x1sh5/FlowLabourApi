
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace FlowLabourApi.Authentication
{
    public class BlacklistService : IBlacklistService
    {
        private readonly MemoryCache _memCache;

        public BlacklistService()
        {
            _memCache = new MemoryCache(new MemoryCacheOptions() { SizeLimit=512*1024*1024});
        }

        public void Add(string token)
        {
            _memCache.Set(token, token);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void ClearExpired()
        {
            throw new NotImplementedException();
        }

        public void ClearExpired(string token)
        {
            throw new NotImplementedException();
        }

        public void ClearExpired(IEnumerable<string> tokens)
        {
            throw new NotImplementedException();
        }

        public bool IsBlacklisted(string token)
        {
            var exists =  _memCache.TryGetValue(token, out var value);
            return exists;
        }

        public void Remove(string token)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }
    }
}
