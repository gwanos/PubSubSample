using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber.Job
{
    public interface IJobController
    {
        void Configure();
        void Start();
    }

    public class SubscribeController : IJobController
    {
        private readonly IServiceProvider _serviceProvider;
        private List<SubscriberJob> _jobs;

        public SubscribeController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Configure()
        {
            var basePath = Directory.GetCurrentDirectory();
            var path = basePath + @"\Settings\subscribersetting.json";
            using (var reader = new StreamReader(path))
            {
                var json = reader.ReadToEnd();
                _jobs = JsonConvert.DeserializeObject<List<SubscriberJob>>(json);
            }
        }

        public void Start()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var tasks = new List<Task>();
                _jobs.ForEach(x =>
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        var sb = new StringBuilder(); ;
                        x.Channels.ForEach(channel => sb.Append($"{channel} "));
                        Console.WriteLine($"{x.Name} subscribe {sb}");
                        
                        var subscriber = scope.ServiceProvider.GetRequiredService<RedisSubscriber>();
                        subscriber.Subscribe(x.Channels, (message) =>
                        {
                            Console.WriteLine($"{x.Name} received a message: {message}");
                        });
                    });
                    tasks.Add(task);
                });

                Task.WhenAll(tasks.ToArray());
            }
        }
    }
}
