using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Auth2Demo.Tests
{
    public class ValueControllerTest
    {
        private readonly IUserRepository _userRepository;
        private readonly WebApplicationFactory<Startup> _factory;

        public ValueControllerTest()
        {
            _userRepository = A.Fake<IUserRepository>();
            A.CallTo(() => _userRepository.LoadUser(A<string>._, A<string>._))
                .Returns(142);

            _factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                    {
                        builder.ConfigureServices(services =>
                        {
                            services.AddSingleton(_userRepository);
                        });
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
        public async Task ValueController_Secret_Returns401()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/values/secret");

            Assert.Equal(
                System.Net.HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task ValueController_SecretAuthorized_Returns200()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var byteArray = Encoding.ASCII.GetBytes("joedoe:secret");
            var authValue = Convert.ToBase64String(byteArray);

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/values/secret");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await client
                .SendAsync(request);

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