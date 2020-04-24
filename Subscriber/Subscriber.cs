using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Subscriber
{
    public interface ISubscriber
    {
        void Subscribe(List<string> channels, Action<string> action);
        Task SubscribeAsync(List<string> channels, Action<string> action);
    }

    public class RedisSubscriber : ISubscriber
    {
        private readonly RedisChannel _channel;

        public RedisSubscriber(RedisChannel channel)
        {
            _channel = channel;
        }

        public void Subscribe(List<string> channels, Action<string> action)
        {
            var subscriber = _channel.Connection.GetSubscriber();
            channels.ForEach(channel =>
            {
                var queue = subscriber.Subscribe(channel);
                queue.OnMessage(message =>
                {
                    action.Invoke(message.Message);
                });
            });
        }

        public async Task SubscribeAsync(List<string> channels, Action<string> action)
        {
            var subscriber = _channel.Connection.GetSubscriber();
            var tasks = channels.Select(async channel => 
            {
                var queue = await subscriber.SubscribeAsync(channel).ConfigureAwait(false);
                queue.OnMessage(message =>
                {
                    action.Invoke(message.Message);
                });
            }).ToArray();
            await Task.WhenAll(tasks);
        }
    }
}
