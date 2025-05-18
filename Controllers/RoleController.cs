using DotnetCoreIdentity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoreIdentity.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _identityRole;
        private readonly ApplicationDbContext _context;
        public RoleController(RoleManager<IdentityRole> identityRole, ApplicationDbContext context)
        {
            _identityRole = identityRole;
            _context = context;
        }

        [AuthorizePermission("View")]
        public IActionResult Index()
        {
            ViewBag.MenuLinks = _context.MenuLinkTitles.ToList();
            ViewBag.RoleList = _identityRole.Roles.ToList();
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.MenuLinks = _context.MenuLinkTitles.ToList();
            return View();
        }


        [HttpGet]
        public IActionResult LoadEditForm(string roleId)
        {
            var role = _identityRole.FindByIdAsync(roleId).Result;
            var allMenus = _context.MenuLinkTitles.ToList();
            var selectedMenuIds = _context.RoleRights
                .Where(r => r.RoleId == roleId)
                .Select(r => r.MenuLinkId)
                .ToList();

            ViewBag.RoleId = role.Id;
            ViewBag.RoleName = role.Name;
            ViewBag.IsEdit = true;
            ViewBag.SelectedMenus = selectedMenuIds;

            return PartialView("_CreateRolePartial", allMenus);
        }

        [HttpPost]
        [AuthorizePermission("Delete")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _identityRole.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            var rights = _context.RoleRights.Where(r => r.RoleId == roleId);
            _context.RoleRights.RemoveRange(rights);
            await _context.SaveChangesAsync();

            var result = await _identityRole.DeleteAsync(role);
            return Ok(new { message = result.Succeeded ? "Deleted" : "Failed to delete" });
        }

    }
}
