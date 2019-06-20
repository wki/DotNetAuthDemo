using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Auth3Demo.Hubs
{
    [AllowAnonymous]
    public class EchoHub: Hub
    {
        private readonly ILogger<EchoHub> _logger;

        public EchoHub(ILogger<EchoHub> logger)
        {
            _logger = logger;
        }

        public async Task SendToClient(string message)
        {
            _logger.LogInformation("Sending to all clients: {0}", message);
            await Clients.All.SendAsync("Message", message);
        }
    }
}