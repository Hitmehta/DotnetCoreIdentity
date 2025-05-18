using DotnetCoreIdentity.Data;
using DotnetCoreIdentity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoreIdentity.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiRoleController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _identityUser;
        private readonly RoleManager<IdentityRole> _identityRole;
        private readonly ApplicationDbContext _context;
        public ApiRoleController(UserManager<IdentityUser> identityUser, RoleManager<IdentityRole> identityRole, ApplicationDbContext dbContext)
        {
            _identityRole = identityRole;
            _identityUser = identityUser;
            _context = dbContext;
        }
       

        [HttpPost("CreateRoleWithRights")]
        [AuthorizePermission("Insert")]
        public async Task<ActionResult> CreateRoleWithRights()
        {
            var form = Request.Form;
            var rolename = form["RoleName"];
            var menuLinkIds = form["menuLinkIds"]; // Multiple values from checkboxes

            var result = await _identityRole.CreateAsync(new IdentityRole(rolename));

            if (!result.Succeeded)
            {
                var errorMsg = result.Errors.FirstOrDefault()?.Description ?? "Something went wrong.";
                return BadRequest(new { message = errorMsg });
            }

            // Get created role
            var role = await _identityRole.FindByNameAsync(rolename);

            if (menuLinkIds.Count > 0)
            {
                foreach (var id in menuLinkIds)
                {   
                    var roleRight = new RoleRights
                    {
                        Id = Guid.NewGuid(),
                        RoleId = role.Id,
                        MenuLinkId = Guid.Parse(id)
                    };
                    _context.RoleRights.Add(roleRight);
                }

                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Role and rights saved successfully." });
        }

        [HttpPost("UpdateRoleWithRights")]
        [AuthorizePermission("Update")]
        public async Task<IActionResult> UpdateRoleWithRights()
        {
            var form = Request.Form;

            string roleId = form["RoleId"];
            string newRoleName = form["RoleName"];

            if (string.IsNullOrEmpty(roleId) || string.IsNullOrEmpty(newRoleName))
                return BadRequest(new { message = "Role ID or name is missing." });

            // 1. Update Role Name
            var role = await _identityRole.FindByIdAsync(roleId);
            if (role == null)
                return NotFound(new { message = "Role not found." });

            role.Name = newRoleName;
            var updateResult = await _identityRole.UpdateAsync(role);
            if (!updateResult.Succeeded)
                return BadRequest(new { message = "Failed to update role name." });

            // 2. Update RoleRights
            var existingRights = _context.RoleRights.Where(r => r.RoleId == roleId);
            _context.RoleRights.RemoveRange(existingRights);

            var selectedMenuLinkIds = form["menuLinkIds"].ToList();

            foreach (var menuLinkIdStr in selectedMenuLinkIds)
            {
                if (Guid.TryParse(menuLinkIdStr, out Guid menuLinkId))
                {
                    var newRight = new RoleRights
                    {
                        Id = Guid.NewGuid(),
                        RoleId = roleId,
                        MenuLinkId = menuLinkId
                    };
                    _context.RoleRights.Add(newRight);
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Role and rights updated successfully." });
        }

    }
}
