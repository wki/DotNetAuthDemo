using Carter;
using Microsoft.AspNetCore.Http;

namespace CarterDemo.Features.Home
{
    public class HomeModule: CarterModule
    {
        public HomeModule()
        {
            Get("/",
                (req, res, routeData) => res.WriteAsync("Hello World!"));
        }
    }
}