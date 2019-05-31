using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Auth2Demo
{
    public class BasicAuthenticationHandler: IAuthenticationHandler
    {
        private AuthenticationScheme _scheme;
        private HttpContext _context;

        public async Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _scheme = scheme;
            _context = context;
        }

        public async Task<AuthenticateResult> AuthenticateAsync()
        {
            try
            {
                var auth = AuthenticationHeaderValue.Parse(_context.Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(auth.Parameter);
                var nameAndPassword = Encoding.ASCII.GetString(bytes);
                var parts = nameAndPassword.Split(":");
                var (name, password) = (parts[0], parts[1]);

                // TODO: lookup

                var principal = new ClaimsPrincipal(new []
                {
                    new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "42"),
                        new Claim(ClaimTypes.Name, name),
                    }),
                });

                return AuthenticateResult.Success(new AuthenticationTicket(principal, _scheme.Name));
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail($"could not authenticate: {e.Message}");
            }
        }

        public async Task ChallengeAsync(AuthenticationProperties properties)
        {
            _context.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"FooBar\", charset=\"UTF-8\"";
            _context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }

        public async Task ForbidAsync(AuthenticationProperties properties)
        {
        }
    }
}