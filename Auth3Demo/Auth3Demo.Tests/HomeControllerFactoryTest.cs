using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FakeItEasy;

namespace Auth3Demo.Tests
{
    /// <summary>
    /// End-To-End Test employing a WebApplicationFactory
    /// </summary>
    public class HomeControllerFactoryTest : IDisposable
    {
        private IUserRepository _userRepository;
        private WebAppFactory _factory;

        public HomeControllerFactoryTest()
        {
            _userRepository = A.Fake<IUserRepository>();
            A.CallTo(() =>
                    _userRepository.LoadUser(
                        A<string>.That.Matches(u => u == "joedoe"),
                        A<string>.That.Matches(p => p == "secret")
                    )
                )
                .Returns(142);
            _factory = new WebAppFactory(_userRepository);
        }

        public void Dispose()
        {
            _factory?.Dispose();
        }

        [Fact]
        public async Task HomeControllerFactory_Secret_ReturnsOk()
        {
            // Arrange
            var client = _factory.CreateClient();
            var byteArray = Encoding.ASCII.GetBytes("joedoe:secret");
            var authValue = Convert.ToBase64String(byteArray);

            var request = new HttpRequestMessage(HttpMethod.Get, "/home/secret");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authValue);

            // Act
            var response = await client.SendAsync(request);

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