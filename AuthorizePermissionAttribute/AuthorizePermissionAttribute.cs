using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using DotnetCoreIdentity.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class AuthorizePermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _permission; // "View", "Insert", "Update", "Delete"
    private ApplicationDbContext _dbContext;
    private UserManager<IdentityUser> _userManager;

    public AuthorizePermissionAttribute(string permission)
    {
        _permission = permission;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var httpContext = context.HttpContext;
        var user = httpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new ChallengeResult();
            return;
        }

        // Resolve services
        _dbContext = httpContext.RequestServices.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
        _userManager = httpContext.RequestServices.GetService(typeof(UserManager<IdentityUser>)) as UserManager<IdentityUser>;

        var identityUser = await _userManager.GetUserAsync(user);
        if (identityUser == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var roles = await _userManager.GetRolesAsync(identityUser);
        var roleIds = _dbContext.Roles.Where(r => roles.Contains(r.Name)).Select(r => r.Id).ToList();

        // Get current controller and action from route data
        var routeData = context.RouteData.Values;
        var controllerName = routeData["controller"]?.ToString();
        var actionName = routeData["action"]?.ToString();
        controllerName = controllerName.ToLower().Contains("api") ? controllerName.ToLower().Replace("api", "") : controllerName;
        if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
        {
            context.Result = new ForbidResult();
            return;
        }

        // Find the corresponding MenuLink record by controller and action
        var menuLink = await _dbContext.MenuLinkTitles
            .FirstOrDefaultAsync(m =>
                m.ControllerName.ToLower() == controllerName.ToLower());

        if (menuLink == null)
        {
            // No menu link found for this route, deny access or allow based on your logic
            context.Result = new ForbidResult();
            return;
        }

        bool hasPermission = false;

        // Check permission for each role
        foreach (var roleId in roleIds)
        {
            var permissionRecord = await _dbContext.RoleMenuPermissions
                .Where(rp => rp.RoleId == roleId && rp.MenuLinkId == menuLink.Id)
                .FirstOrDefaultAsync();

            if (permissionRecord != null)
            {
                switch (_permission.ToLower())
                {
                    case "view":
                        hasPermission = permissionRecord.CanView;
                        break;
                    case "insert":
                        hasPermission = permissionRecord.CanInsert;
                        break;
                    case "update":
                        hasPermission = permissionRecord.CanUpdate;
                        break;
                    case "delete":
                        hasPermission = permissionRecord.CanDelete;
                        break;
                }
                if (hasPermission) break;
            }
        }

        if (!hasPermission)
        {
            context.Result = new JsonResult(new { message = $"You do not have permission to {_permission.ToLower()} this item." })
            { StatusCode = 403 };
            return;
        }
    }
}
