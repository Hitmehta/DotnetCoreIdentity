using DotnetCoreIdentity.Data;
using DotnetCoreIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DotnetCoreIdentity.Components
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MenuViewComponent(
            ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                // Not logged in, return empty list or some default menu
                return View(new List<MenuLinkTitles>());
            }

            // Get roles of the logged-in user
            var roles = await _userManager.GetRolesAsync(user);

            // Get role IDs from role names
            var roleIds = await _context.Roles
                .Where(r => roles.Contains(r.Name))
                .Select(r => r.Id)
                .ToListAsync();

            // Get MenuLinkIds allowed for the user's roles
            var allowedMenuLinkIds = await _context.RoleRights
                .Where(rr => roleIds.Contains(rr.RoleId))
                .Select(rr => rr.MenuLinkId)
                .Distinct()
                .ToListAsync();

            // Get MenuLinkTitles that are allowed
            var menuItems = await _context.MenuLinkTitles
                .Where(m => allowedMenuLinkIds.Contains(m.Id))
                .ToListAsync();

            return View(menuItems);
        }
    }
}
