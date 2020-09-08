using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MonitoramentoBasesSql.Clients;

namespace MonitoramentoBasesSql
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddHttpClient<CanalSlackClient>();
                });
    }
}