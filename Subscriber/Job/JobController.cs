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
            var path = Path.Combine(basePath, "Settings", "subscribersetting.json");
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
                var tasks = _jobs.Select(job =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        var sb = new StringBuilder();
                        job.Channels.ForEach(channel => sb.Append($"{channel} "));
                        Console.WriteLine($"{job.Name} subscribe {sb}");
                        
                        var subscriber = scope.ServiceProvider.GetRequiredService<RedisSubscriber>();
                        subscriber.Subscribe(job.Channels, (message) =>
                        {
                            Console.WriteLine($"{job.Name} received a message: {message}");
                        });
                    });
                }).ToArray();

                Task.WaitAll(tasks);
                Console.WriteLine($"...{tasks.Count()} subscribers are ready.");
                Console.WriteLine($"========================================");
            }
        }
    }
}
