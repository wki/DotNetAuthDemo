using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Auth3Demo.Tests
{
    public class WebAppFactory : WebApplicationFactory<Startup>
    {
        private readonly IUserRepository _userRepository;

        public WebAppFactory(IUserRepository userRepository = null)
        {
            _userRepository = userRepository;
        }

        protected override IWebHostBuilder CreateWebHostBuilder() =>
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // builder.UseContentRoot(".");
            builder
                .ConfigureServices(services =>
                {
                    if (_userRepository != null)
                        services.AddSingleton(_userRepository);
                });
            base.ConfigureWebHost(builder);
        }
    }
}