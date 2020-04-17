using System;
using System.Threading.Tasks;
using Common;

namespace Publisher
{
    public interface IPublisher
    {
        long Publish(string channel, string message);
        Task<long> PublishAsync(string channel, string message);
    }

    public class RedisPublisher : IPublisher
    {
        private readonly RedisChannel _channel;

        public RedisPublisher(RedisChannel channel)
        {
            _channel = channel;
        }

        public long Publish(string channel, string message)
        {
            var subscriber = _channel.Connection.GetSubscriber();
            return subscriber.Publish(channel, message);
        }

        public async Task<long> PublishAsync(string channel, string message)
        {
            var subscriber = _channel.Connection.GetSubscriber();
            return await subscriber.PublishAsync(channel, message).ConfigureAwait(false);
        }
    }
}
