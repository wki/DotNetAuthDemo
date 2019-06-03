using System;
using System.Net;
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
            A.CallTo(() =>
                    _userRepository.LoadUser(
                        A<string>.That.Matches(u => u == "joedoe"),
                        A<string>.That.Matches(p => p == "secret")
                    )
                )
                .Returns(142);

            _factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services => { services.AddSingleton(_userRepository); });
                });
        }

        private HttpClient Client => _factory.CreateClient();

        [Fact]
        public async Task ValueController_Index_Returns200()
        {
            // Act
            var response = await Client.GetAsync("/api/values");

            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task ValueController_Secret_Returns401()
        {
            // Act
            var response = await Client.GetAsync("/api/values/secret");

            // Assert
            Assert.Equal(
                HttpStatusCode.Unauthorized,
                response.StatusCode);
        }

        [Fact]
        public async Task ValueController_SecretAuthorized_Returns200()
        {
            // Act
            var byteArray = Encoding.ASCII.GetBytes("joedoe:secret");
            var authValue = Convert.ToBase64String(byteArray);

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/values/secret");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
            Assert.Equal(
                "huhu:joedoe/142",
                await response.Content.ReadAsStringAsync());
        }
    }
}