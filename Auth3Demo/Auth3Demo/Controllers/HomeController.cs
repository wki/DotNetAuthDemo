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
            return Ok("huhu");
        }

        [Authorize]
        [HttpGet("secret")]
        public IActionResult Secret()
        {
            var name = "";
            var id = "";
            var user = ControllerContext?.HttpContext?.User;
            if (user != null && user.HasClaim(c => c.Type == ClaimTypes.Name))
                name = user.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            if (user != null && user.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
                id = user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return Ok($"huhu:{name}/{id}");
        }
    }
}