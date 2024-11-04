using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserService _userService;

        public RegisterController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string name, string username, string password, string confirmPassword)
        {
            if (ModelState.IsValid && password == confirmPassword)
            {
                if (_userService.UserExists(username))
                {
                    ModelState.AddModelError(string.Empty, "Username already exists.");
                    return View();
                }

                _userService.Register(name, username, password);
                await SignInUser(username);
                return RedirectToAction("Index", "ChatMessages");
            }

            return View();
        }

        private async Task SignInUser(string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                "CookieAuth",
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
