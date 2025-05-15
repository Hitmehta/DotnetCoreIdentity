using DotnetCoreIdentity.Data;
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
        [HttpPost("CreateRole")]
        public async Task<ActionResult> CreateRole()
        {
            var form = Request.Form;
            var rolename= form["RoleName"];
            var  result  =await _identityRole.CreateAsync(new IdentityRole(rolename.ToString()));
            return Ok();
        }
    }
}
