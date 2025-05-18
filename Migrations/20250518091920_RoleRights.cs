using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetCoreIdentity.Migrations
{
    /// <inheritdoc />
    public partial class RoleRights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleRights",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MenuLinkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleRights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleRights_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleRights_MenuLinkTitles_MenuLinkId",
                        column: x => x.MenuLinkId,
                        principalTable: "MenuLinkTitles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleRights_MenuLinkId",
                table: "RoleRights",
                column: "MenuLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleRights_RoleId",
                table: "RoleRights",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleRights");
        }
    }
}
