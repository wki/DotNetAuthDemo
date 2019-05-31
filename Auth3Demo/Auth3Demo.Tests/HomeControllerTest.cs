using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Auth3Demo.Tests
{
    public class HomeControllerTest
    {
        private Task<IHost> BuildHost() =>
            Program.CreateHostBuilder(new string[]{})
                .ConfigureWebHost(webBuilder => webBuilder.UseTestServer())
                .StartAsync();

        [Fact]
        public async Task ValuesController_Index_Returns200()
        {
            // Arrange
            var host = await BuildHost();

            // Act
            var response = await host.GetTestClient()
                .GetAsync("/home");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal("huhu:", await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ValuesController_Secret_Returns401()
        {
            // Arrange
            var host = await BuildHost();

            // Act
            var response = await host.GetTestClient()
                .GetAsync("/home/secret");

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task ValuesController_SecretWithCredentials_Returns200()
        {
            // Arrange
            var host = await BuildHost();

            // Act
            var byteArray = Encoding.ASCII.GetBytes("username:password1234");
            var authValue = Convert.ToBase64String(byteArray);

            var request = new HttpRequestMessage(HttpMethod.Get, "/home/secret");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await host.GetTestClient()
                .SendAsync(request);

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode);
        }
    }
}