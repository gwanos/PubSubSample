using Microsoft.Extensions.DependencyInjection;
using Subscriber.Job;
using System;

namespace Subscriber
{
    public class App
    {
        private readonly IServiceProvider _serviceProvider;
        private bool _cancelKeyPressed = false;

        public App(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Run()
        {
            Console.Clear();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(controlCHandler);
            Console.WriteLine("Press CTRL+C to quit.");

            this.runSubscriberTask();

            while (true)
            {
                if (_cancelKeyPressed)
                {
                    Console.WriteLine("Bye.");
                    break;
                }
            }
        }

        private void runSubscriberTask()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var controller = scope.ServiceProvider.GetRequiredService<SubscribeController>();
                controller.Configure();
                controller.Start();
            }
        }

        private void controlCHandler(object sender, ConsoleCancelEventArgs args)
        {
            if (args.SpecialKey == ConsoleSpecialKey.ControlC)
            {
                _cancelKeyPressed = true;
                args.Cancel = true;
            }
        }
    }
}
