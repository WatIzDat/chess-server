using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessServer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "matches",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserMatch",
                schema: "identity",
                columns: table => new
                {
                    ConnectedUsersId = table.Column<string>(type: "text", nullable: false),
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserMatch", x => new { x.ConnectedUsersId, x.MatchId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserMatch_AspNetUsers_ConnectedUsersId",
                        column: x => x.ConnectedUsersId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserMatch_matches_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "public",
                        principalTable: "matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserMatch_MatchId",
                schema: "identity",
                table: "ApplicationUserMatch",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserMatch",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "matches",
                schema: "public");
        }
    }
}
