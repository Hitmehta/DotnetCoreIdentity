using DotnetCoreIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoreIdentity.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<MenuLinkTitles> MenuLinkTitles { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermissions { get; set; }
        public DbSet<RoleRights> RoleRights { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RoleMenuPermission>()
                .HasOne(p => p.Role)
                .WithMany()  // No collection in IdentityRole
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        
    }
}
