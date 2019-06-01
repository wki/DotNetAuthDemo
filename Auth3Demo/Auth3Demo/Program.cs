using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Auth3Demo
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
                        .UseUrls("http://localhost:5000", "http://192.168.2.21:5000")
                        .ConfigureServices(services =>
                        {
                            // tryAdd dependencies needed as mocked services
                            // in tests
                            services.TryAddSingleton<IUserRepository, UserRepository>();

                            services.AddAuthentication(authBuilder => { authBuilder.DefaultScheme = "Basic"; })
                                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);
                            services.AddAuthorization(authOptions =>
                            {
                                authOptions.AddPolicy(
                                    "Users",
                                    builder =>
                                    {
                                        builder.AddAuthenticationSchemes("Basic");
                                        builder.RequireClaim(ClaimTypes.Name);
                                    });
                            });
                            services.AddControllers()
                                .AddNewtonsoftJson();
                            services.AddLogging(logging =>
                            {
                                logging.AddConsole();
                                logging.AddDebug();
                            });
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();
                            app.UseAuthentication();
                            app.UseAuthorization();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                            });
                        });
                });
    }
}