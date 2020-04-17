using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Subscriber.Controller;
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
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var config = LoadConfiguration();
            services.AddSingleton(config);

            services.AddSingleton(x => new RedisChannel(config["CacheConnection"]));
            services.AddTransient<SubscribeController>();
            services.AddTransient<RedisSubscriber>();
            services.AddTransient<App>();
            
            return services;
        }

        public static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("./Settings/appsetting.json", optional: true, reloadOnChange: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly(), false);

            return builder.Build();
        }
    }
}
