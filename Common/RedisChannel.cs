using System;
using StackExchange.Redis;

namespace Common
{
    public class RedisChannel : IDisposable
    {
        private readonly string _url;

        public ConnectionMultiplexer Connection { get; }

        public RedisChannel(string url)
        {
            _url = url;
            this.Connection = ConnectionMultiplexer.Connect(_url);
        }

        public void Dispose()
        {
            this.Connection.Dispose();  
        }
    }
}
