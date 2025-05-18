using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetCoreIdentity.Models
{
    public class RoleMenuPermission
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public Guid MenuLinkId { get; set; }

        public bool CanView { get; set; }
        public bool CanInsert { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }

        public MenuLinkTitles MenuLink { get; set; }

        public IdentityRole Role { get; set; }
    }

    public class RoleRights
    {
        public Guid Id { get; set; }
        
        public string RoleId { get; set; }

        [ForeignKey("RoleId")]
        public IdentityRole Role { get; set; }

        public Guid MenuLinkId { get; set; }

        [ForeignKey("MenuLinkId")]
        public MenuLinkTitles MenuLink { get; set; }
    }
}
