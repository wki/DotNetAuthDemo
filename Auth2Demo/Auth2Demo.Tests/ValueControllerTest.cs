using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Auth2Demo.Tests
{
    public class ValueControllerTest
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ValueControllerTest()
        {
            _factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {

                });
        }

        [Fact]
        public async Task ValueController_Index_Returns200()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/values");

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task ValueController_Secure_Returns401()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/values/secret");

            Assert.Equal(
                System.Net.HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task ValueController_SecureAuthorized_Returns200()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var byteArray = Encoding.ASCII.GetBytes("username:password1234");
            var authValue = Convert.ToBase64String(byteArray);

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/values/secret");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await client
                .SendAsync(request);

            // Assert
            Assert.Equal(
                System.Net.HttpStatusCode.OK,
                response.StatusCode);

        }
    }
}