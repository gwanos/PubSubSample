using System;
using System.Threading.Tasks;
using Common;

namespace Subscriber
{
    public interface ISubscriber
    {
        void Subscribe(string channel, Action<string> action);
        Task SubscribeAsync(string channel, Action<string> action);
    }

    public class RedisSubscriber : ISubscriber, IDisposable
    {
        private readonly RedisChannel _channel;

        public RedisSubscriber(RedisChannel channel)
        {
            _channel = channel;
        }

        public void Dispose()
        {
            _channel.Connection.Dispose();
        }

        public void Subscribe(string channel, Action<string> action)
        {
            var subscriber = _channel.Connection.GetSubscriber();
            var queue = subscriber.Subscribe(channel);
            queue.OnMessage((handler) =>
            {
                action.Invoke(handler.Message);
            });
        }

        public Task SubscribeAsync(string channel, Action<string> action)
        {
            _channel.Connection.Dispose();
            throw new NotImplementedException();
        }
    }
}
