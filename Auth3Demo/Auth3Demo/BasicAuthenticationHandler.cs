using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auth3Demo
{
    public class BasicAuthenticationHandler: AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserRepository _userRepository;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserRepository userRepository) : base(options, logger, encoder, clock)
        {
            _userRepository = userRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var auth = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(auth.Parameter);
                var nameAndPassword = Encoding.ASCII.GetString(bytes);
                var parts = nameAndPassword.Split(":");
                var (name, password) = (parts[0], parts[1]);

                var id = _userRepository.LoadUser(name, password);

                var principal = new ClaimsPrincipal(new []
                {
                    new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, id.ToString("0")),
                        new Claim(ClaimTypes.Name, name),
                    }),
                });

                return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail($"could not authenticate: {e.Message}");
            }
        }
    }
}