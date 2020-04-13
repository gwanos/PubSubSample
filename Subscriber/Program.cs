using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Common;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;

namespace Subscriber
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            
            serviceProvider.GetService<App>().Run();
            //Channel.Connection.Dispose();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);

            services.AddSingleton(x => new RedisChannel(config["CacheConnection"]));
            services.AddTransient<ISubscriber, RedisSubscriber>();
            services.AddTransient<RedisSubscriber>();
            services.AddTransient<App>();


            
            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsetting.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), false);

            return builder.Build();
        }
    }
}
