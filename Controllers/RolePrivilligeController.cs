using DotnetCoreIdentity.Data;
using DotnetCoreIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace DotnetCoreIdentity.Controllers
{
    public class RolePrivilligeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolePrivilligeController(ApplicationDbContext dbContext, RoleManager<IdentityRole>roleManager)
        {
            _context = dbContext;
            _roleManager = roleManager;
        }

        public ActionResult Index()
        {
            var menuLinks = _context.MenuLinkTitles.ToList();
            var roles = _roleManager.Roles.ToList();
            var model = new RoleSelectionViewModel
            {
                MenuLinks = menuLinks
            };
            ViewBag.RoleList = new SelectList(roles, "Id", "Name"); ;
            return View(model);
        }

        [HttpGet]
        public IActionResult GetPermissionsByRole(string roleId)
        {
            var allMenuLinks = _context.MenuLinkTitles.ToList();

            var rolePermissions = _context.RoleMenuPermissions
                .Where(rp => rp.RoleId == roleId)
                .ToList();
            var model = allMenuLinks.Select(menuLink =>
            {
                var permission = rolePermissions.FirstOrDefault(rp => rp.MenuLinkId == menuLink.Id);

                return new RoleMenuPermission
                {
                    MenuLink = menuLink,
                    RoleId = roleId,
                    MenuLinkId = menuLink.Id,
                    CanView = permission?.CanView ?? false,
                    CanInsert = permission?.CanInsert ?? false,
                    CanUpdate = permission?.CanUpdate ?? false,
                    CanDelete = permission?.CanDelete ?? false
                };
            }).ToList();

            return PartialView("_RolePermissionsPartial", model);
        }

        [HttpPost]
        public IActionResult UpdatePermissionsForRole([FromBody] List<RoleMenuPermission> updatedPermissions)
        {
            if (updatedPermissions == null || !updatedPermissions.Any())
                return BadRequest("No permissions provided");

            var roleId = updatedPermissions.First().RoleId;

            // Get existing permissions for the role
            var existingPermissions = _context.RoleMenuPermissions
                .Where(rp => rp.RoleId == roleId)
                .ToList();

            foreach (var updatedPerm in updatedPermissions)
            {
                var existingPerm = existingPermissions
                    .FirstOrDefault(rp => rp.MenuLinkId == updatedPerm.MenuLinkId);

                if (existingPerm != null)
                {
                    // Update existing record
                    existingPerm.CanView = updatedPerm.CanView;
                    existingPerm.CanInsert = updatedPerm.CanInsert;
                    existingPerm.CanUpdate = updatedPerm.CanUpdate;
                    existingPerm.CanDelete = updatedPerm.CanDelete;
                }
                else
                {
                    // Insert new record
                    _context.RoleMenuPermissions.Add(new RoleMenuPermission
                    {
                        RoleId = roleId,
                        MenuLinkId = updatedPerm.MenuLinkId,
                        CanView = updatedPerm.CanView,
                        CanInsert = updatedPerm.CanInsert,
                        CanUpdate = updatedPerm.CanUpdate,
                        CanDelete = updatedPerm.CanDelete
                    });
                }
            }

            _context.SaveChanges();

            return Ok("Permissions updated successfully");
        }


    }
}
