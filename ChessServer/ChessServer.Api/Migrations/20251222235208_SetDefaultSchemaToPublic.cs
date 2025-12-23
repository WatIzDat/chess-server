using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessServer.Api.Migrations
{
    /// <inheritdoc />
    public partial class SetDefaultSchemaToPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "matches",
                newName: "matches",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "identity",
                newName: "AspNetUserTokens",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "identity",
                newName: "AspNetUsers",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "identity",
                newName: "AspNetUserRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "identity",
                newName: "AspNetUserLogins",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "identity",
                newName: "AspNetUserClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "identity",
                newName: "AspNetRoles",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "identity",
                newName: "AspNetRoleClaims",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "ApplicationUserMatch",
                schema: "identity",
                newName: "ApplicationUserMatch",
                newSchema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "matches",
                schema: "public",
                newName: "matches");

            migrationBuilder.RenameTable(
                name: "AspNetUserTokens",
                schema: "public",
                newName: "AspNetUserTokens");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                schema: "public",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUserRoles",
                schema: "public",
                newName: "AspNetUserRoles");

            migrationBuilder.RenameTable(
                name: "AspNetUserLogins",
                schema: "public",
                newName: "AspNetUserLogins");

            migrationBuilder.RenameTable(
                name: "AspNetUserClaims",
                schema: "public",
                newName: "AspNetUserClaims");

            migrationBuilder.RenameTable(
                name: "AspNetRoles",
                schema: "public",
                newName: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "AspNetRoleClaims",
                schema: "public",
                newName: "AspNetRoleClaims");

            migrationBuilder.RenameTable(
                name: "ApplicationUserMatch",
                schema: "public",
                newName: "ApplicationUserMatch");
        }
    }
}
