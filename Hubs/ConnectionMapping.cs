namespace FlowLabourApi.Hubs
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, string> _connections =
            new Dictionary<T, string>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                _connections.Add(key, connectionId);

            }
        }

        public string GetConnection(T key)
        {
            string connection;
            if (_connections.TryGetValue(key, out connection))
            {
                return connection;
            }

            return string.Empty;
        }

        public void Remove(T key)
        {
            lock (_connections)
            {
                _connections.Remove(key);
            }
        }

        public IEnumerable<string> GetConnections()
        {
            return _connections.Values;
        }

        public void Update(T key, string connectionId)
        {
            lock (_connections)
            {
                _connections[key] = connectionId;
            }
        }
    }
}
