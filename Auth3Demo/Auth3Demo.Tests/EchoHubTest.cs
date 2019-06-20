using System;
using System.Net.Http;
using System.Threading.Tasks;
using Auth3Demo.Hubs;
using Microsoft.AspNetCore.SignalR;
using Xunit;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Auth3Demo.Tests
{
    public class EchoHubTest : IDisposable
    {
        private WebAppFactory _factory;
//        private IHubContext<EchoHub> _hub;

        public EchoHubTest()
        {
            _factory = new WebAppFactory();
//            _hub = _factory.Services.GetService<IHubContext<EchoHub>>();
        }

        public void Dispose()
        {
            _factory?.Dispose();
        }

        private async Task<HubConnection> StartConnectionAsync(HttpMessageHandler handler, string hubName)
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"ws://localhost/hubs/{hubName}",
                    o => { o.HttpMessageHandlerFactory = _ => handler; })
                .Build();
            await hubConnection.StartAsync();
            return hubConnection;
        }

        [Fact]
        public async Task EchoHub_SendToClient_InformsClient()
        {
            // Arrange
            var server = _factory.Server;
            var connection = await StartConnectionAsync(server.CreateHandler(), "echo");

            var msg = "";
            connection.On<string>(
                "Message",
                m => { msg = m; });

            // Act
            // await _hub.Clients.All.SendAsync("Message", "huhu");
            await connection.InvokeAsync("Message", "huhu");

            // Assert
            Assert.Equal("huhu", msg);
        }
    }
}