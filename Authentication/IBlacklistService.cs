namespace FlowLabourApi.Authentication
{
    public interface IBlacklistService
    {
        bool IsBlacklisted(string token);

        void Add(string token);

        void Remove(string token);

        void RemoveAll();

        void Clear();

        void ClearExpired();

        void ClearExpired(string token);

        void ClearExpired(IEnumerable<string> tokens);
    }
}