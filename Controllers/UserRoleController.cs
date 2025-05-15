using Microsoft.AspNetCore.Mvc;

namespace DotnetCoreIdentity.Controllers
{
    public class UserRoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
