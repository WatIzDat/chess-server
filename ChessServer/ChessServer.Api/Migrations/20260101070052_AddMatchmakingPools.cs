using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessServer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchmakingPools : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeControls",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InitialTime = table.Column<long>(type: "bigint", nullable: false),
                    IncrementTime = table.Column<long>(type: "bigint", nullable: false),
                    UseDelay = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeControls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatchmakingPools",
                schema: "public",
                columns: table => new
                {
                    TimeControlId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchmakingPools", x => x.TimeControlId);
                    table.ForeignKey(
                        name: "FK_MatchmakingPools_TimeControls_TimeControlId",
                        column: x => x.TimeControlId,
                        principalSchema: "public",
                        principalTable: "TimeControls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserMatchmakingPool",
                schema: "public",
                columns: table => new
                {
                    MatchmakingPoolTimeControlId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserMatchmakingPool", x => new { x.MatchmakingPoolTimeControlId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserMatchmakingPool_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalSchema: "public",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserMatchmakingPool_MatchmakingPools_Matchmaking~",
                        column: x => x.MatchmakingPoolTimeControlId,
                        principalSchema: "public",
                        principalTable: "MatchmakingPools",
                        principalColumn: "TimeControlId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserMatchmakingPool_UsersId",
                schema: "public",
                table: "ApplicationUserMatchmakingPool",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserMatchmakingPool",
                schema: "public");

            migrationBuilder.DropTable(
                name: "MatchmakingPools",
                schema: "public");

            migrationBuilder.DropTable(
                name: "TimeControls",
                schema: "public");
        }
    }
}
