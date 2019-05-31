using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth3Demo.Controllers
{
    [Route("/home")]
    public class HomeController : Controller
    {
        [AllowAnonymous]
        [HttpGet("")]
        public IActionResult Index()
        {
            var name = "";
            var user = ControllerContext?.HttpContext?.User;
            if (user != null && user.HasClaim(c => c.Type == ClaimTypes.Name))
                name = user.Claims.First(c => c.Type == ClaimTypes.Name).Value;

            return Ok($"huhu:{name}");
        }

        [Authorize(Policy = "Users")]
        [HttpGet("secret")]
        public IActionResult Secret()
        {
            var name = "";
            var user = ControllerContext?.HttpContext?.User;
            if (user != null && user.HasClaim(c => c.Type == ClaimTypes.Name))
                name = user.Claims.First(c => c.Type == ClaimTypes.Name).Value;

            return Ok($"huhu:{name}");
        }
    }}