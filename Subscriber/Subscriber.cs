using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace Subscriber
{
    public interface ISubscriber
    {
        void Subscribe(List<string> channel, Action<string> action);
        Task SubscribeAsync(List<string> channel, Action<string> action);
    }

    public class RedisSubscriber : ISubscriber
    {
        private readonly RedisChannel _channel;

        public RedisSubscriber(RedisChannel channel)
        {
            _channel = channel;
        }

        public void Subscribe(List<string> channel, Action<string> action)
        {
            var subscriber = _channel.Connection.GetSubscriber();
            channel.ForEach(x =>
            {
                subscriber.Subscribe(x, (channel, message) =>
                {
                    action.Invoke(message);
                });
            });
        }

        public Task SubscribeAsync(List<string> channel, Action<string> action)
        {
            throw new NotImplementedException();
        }
    }
}
