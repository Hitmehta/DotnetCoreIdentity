using Microsoft.AspNetCore.Mvc;

namespace DotnetCoreIdentity.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
