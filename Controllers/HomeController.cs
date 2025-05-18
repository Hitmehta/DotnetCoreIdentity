using System.Diagnostics;
using DotnetCoreIdentity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCoreIdentity.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _identityUser;
        public HomeController(ILogger<HomeController> logger, SignInManager<IdentityUser> signInManager,UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _identityUser = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        public async Task<ActionResult> ClearCache()
        {
            var user = await _identityUser.FindByNameAsync(User.Identity?.Name.ToString());
            await _signInManager.RefreshSignInAsync(user);
            return Ok();
        }
    }
}
