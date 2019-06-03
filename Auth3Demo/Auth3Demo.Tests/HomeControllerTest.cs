using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Auth3Demo.Tests
{
    public class HomeControllerTest : IAsyncLifetime
    {
        private IUserRepository _userRepository;
        private IHost _host;

        public async Task InitializeAsync()
        {
            _userRepository = A.Fake<IUserRepository>();
            A.CallTo(() =>
                    _userRepository.LoadUser(
                        A<string>.That.Matches(u => u == "joedoe"),
                        A<string>.That.Matches(p => p == "secret")
                    )
                )
                .Returns(142);
            _host = await BuildHost();
        }

        private HttpClient Client => _host.GetTestClient();

        public Task DisposeAsync() => Task.CompletedTask;

        private Task<IHost> BuildHost() =>
            Program.CreateHostBuilder(new string[] { })
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .ConfigureServices(services =>
                        {
                            services.AddSingleton(_userRepository);
                        })
                        .UseTestServer();
                })
                .StartAsync();

        [Fact]
        public async Task ValuesController_Index_Returns200()
        {
            // Act
            var response = await Client.GetAsync("/home");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(
                "huhu",
                await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ValuesController_Secret_Returns401()
        {
            // Act
            var response = await Client.GetAsync("/home/secret");

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task ValuesController_SecretWithCredentials_Returns200()
        {
            // Act
            var byteArray = Encoding.ASCII.GetBytes("joedoe:secret");
            var authValue = Convert.ToBase64String(byteArray);

            var request = new HttpRequestMessage(HttpMethod.Get, "/home/secret");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode);
            Assert.Equal(
                "huhu:joedoe/142",
                await response.Content.ReadAsStringAsync());
        }
    }
}