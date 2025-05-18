using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCoreIdentity.Controllers
{
    [Authorize]
    public class UserRoleController : Controller
    {
        [Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
