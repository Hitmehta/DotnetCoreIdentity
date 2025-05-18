using DotnetCoreIdentity.Data;
using DotnetCoreIdentity.Models;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetCoreIdentity.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUserRoleController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _identityUser;
        private readonly RoleManager<IdentityRole> _identityRole;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public ApiUserRoleController(UserManager<IdentityUser> identityUser,RoleManager<IdentityRole> identityRole, ApplicationDbContext dbContext,SignInManager<IdentityUser> signInManager)
        {
            _identityRole = identityRole;
            _identityUser = identityUser;
            _context = dbContext;
            _signInManager = signInManager;
        }

        [HttpGet("GetuserRole")]
        public async Task<ActionResult> GetuserRole() {
            var users = _identityUser.Users.ToList();
            var roleList = _identityRole.Roles.ToList();

            var model = new List<UserWithRolesViewModel>();
            foreach (var user in users)
            {
                var roles =await _identityUser.GetRolesAsync(user);
                model.Add(new UserWithRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    CurrentRole = roles.FirstOrDefault() ?? "-"
                });
            }
            return Ok(new {User = model, Role = roleList });
        }

        [HttpPost("ChangeUserRole")]
        public async Task<ActionResult> ChangeUserRole()
        {
            var form  = Request.Form;
            var userid = form["userID"];
            var roleName = form["roleName"];
            var user = await _identityUser.FindByIdAsync(userid);
            var userRoles = await _identityUser.GetRolesAsync(user);
            await _identityUser.RemoveFromRolesAsync(user, userRoles);

            var removeResult = await _identityUser.RemoveFromRolesAsync(user, userRoles);
            var addResult = await _identityUser.AddToRoleAsync(user, roleName);
            // Add user to the new role
            await _identityUser.AddToRoleAsync(user, roleName);
            if (User.Identity?.Name == user.UserName)
            {
                await _signInManager.RefreshSignInAsync(user);
            }

            return Ok("User role updated successfully.");
        }
    }
}
