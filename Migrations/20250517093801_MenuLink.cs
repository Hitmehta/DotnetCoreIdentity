using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetCoreIdentity.Migrations
{
    /// <inheritdoc />
    public partial class MenuLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuLinkTitles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuLinkName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuLinkTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MenuLinkUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ControllerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuLinkTitles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuLinkTitles");
        }
    }
}
