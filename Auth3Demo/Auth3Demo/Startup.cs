using System;
using System.Security.Claims;
using Auth3Demo.Hubs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Auth3Demo
{
    public class Startup: IStartup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
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
                authOptions.DefaultPolicy = authOptions.GetPolicy("Users");
            });
            services.AddSignalR();
            services.AddControllers()
                .AddNewtonsoftJson();
            services.AddLogging(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            });

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSignalR(configure =>
            {
                configure.MapHub<EchoHub>("/hubs/echo");
            });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}