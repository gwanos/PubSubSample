using System.Reflection;
using Common;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Publisher.Startup))]
namespace Publisher
{
    /// <summary>
    /// 서비스 셋업과 등록을 목적으로 사용하는 클래스
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// 종속성 주입을 사용할 서비스를 등록한다. 등록하지 않은 서비스는 사용할 수 없다.
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly(), false)
                .Build();

            // Runtime Configuration
            // Azure application setting에 정의된 값을 사용할 수 있다.
            builder.Services.AddSingleton<IConfiguration>(config);

            builder.Services.AddSingleton(x => new RedisChannel(config["CacheConnection"]));
            builder.Services.AddTransient<IPublisher, RedisPublisher>();
        }
    }
}
