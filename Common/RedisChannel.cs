using System;
using StackExchange.Redis;

namespace Common
{
    public class RedisChannel
    {
        private readonly string _url;

        public ConnectionMultiplexer Connection { get; }

        public RedisChannel(string url)
        {
            _url = url;
            this.Connection = ConnectionMultiplexer.Connect(_url);
        }
    }
}
