using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FriendifyMain.Models;

namespace FriendifyMain.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        // GET: api/Home
        [HttpGet]
        public IActionResult Index()
        {
            var data = new
            {
                message = "Welcome to Friendify",
                links = new[] {
                    new { href = "/api/Post/Create", text = "Create a new Post" },
                    new { href = "/api/Post/List", text = "View your Post" },
                    new { href = "/api/Account/Login", text = "Log in to your account" },
                    new { href = "/api/Account/Register", text = "Register a new account" }
                }
            };
            return Json(data);
        }
    }
}
