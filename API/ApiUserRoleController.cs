using DotnetCoreIdentity.Data;
using DotnetCoreIdentity.Models;
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
        private readonly ApplicationDbContext _context;
        public ApiUserRoleController(UserManager<IdentityUser> identityUser,RoleManager<IdentityRole> identityRole, ApplicationDbContext dbContext)
        {
            _identityRole = identityRole;
            _identityUser = identityUser;
            _context = dbContext;
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
    }
}
