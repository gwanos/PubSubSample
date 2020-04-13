using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Subscriber
{
    public class App
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Run()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var tasks = new List<Task>();
                var lst = new List<string> { "blue", "green" };
                foreach (var each in lst)
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        Console.WriteLine(each);
                        var subscriber = scope.ServiceProvider.GetService<RedisSubscriber>();
                        subscriber.Subscribe(each, (message) =>
                        {
                            Console.WriteLine(message);
                        });
                        Thread.Sleep(10000);
                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
            }
        }
    }
}
