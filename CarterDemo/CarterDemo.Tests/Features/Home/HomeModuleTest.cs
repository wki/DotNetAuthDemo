using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace CarterDemo.Tests.Features.Home
{
    public class HomeModuleTest : IAsyncLifetime
    {
        private IHost _host;

        public async Task InitializeAsync()
        {
            _host = await BuildHost();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        private Task<IHost> BuildHost() =>
            Program.CreateHostBuilder(new string[] { })
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseTestServer();
                    // webBuilder.ConfigureServices(services => { });
                })
                .StartAsync();

        [Fact]
        public async Task HomeModule_RootUrl_ReturnsHello()
        {
            // Act
            var response = await _host.GetTestClient()
                .GetAsync("/");

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
            Assert.Equal(
                "Hello World!",
                await response.Content.ReadAsStringAsync());
        }
    }
}