using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Carter;

namespace CarterDemo
{
    public class Program
    {
        public static void Main(string[] args) =>
            CreateHostBuilder(args)
                .Build()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureServices(services =>
                        {
                            services.AddCarter();
                            services.AddLogging(logging =>
                            {
                                logging.AddConsole();
                                logging.AddDebug();
                            });
                        })
                        .Configure(app =>
                        {
                            var carterOptions = new CarterOptions(
                                before: null,
                                after: null,
                                openApiOptions: null);
                            app.UseCarter(carterOptions);
                        });
                });
    }
}