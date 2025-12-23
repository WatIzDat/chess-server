using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessServer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchConnectionJoinEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserMatch",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "MatchConnection",
                schema: "public",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConnectedUsersId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchConnection", x => new { x.ConnectedUsersId, x.MatchId });
                    table.ForeignKey(
                        name: "FK_MatchConnection_AspNetUsers_ConnectedUsersId",
                        column: x => x.ConnectedUsersId,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MatchConnection_matches_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "public",
                        principalTable: "matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchConnection_MatchId",
                schema: "public",
                table: "MatchConnection",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchConnection",
                schema: "public");

            migrationBuilder.CreateTable(
                name: "ApplicationUserMatch",
                schema: "public",
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
                        principalSchema: "public",
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
                schema: "public",
                table: "ApplicationUserMatch",
                column: "MatchId");
        }
    }
}
