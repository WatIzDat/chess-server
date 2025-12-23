using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessServer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchConnectionsToDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchConnection_AspNetUsers_ConnectedUsersId",
                schema: "public",
                table: "MatchConnection");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchConnection_matches_MatchId",
                schema: "public",
                table: "MatchConnection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchConnection",
                schema: "public",
                table: "MatchConnection");

            migrationBuilder.RenameTable(
                name: "MatchConnection",
                schema: "public",
                newName: "MatchConnections",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_MatchConnection_MatchId",
                schema: "public",
                table: "MatchConnections",
                newName: "IX_MatchConnections_MatchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchConnections",
                schema: "public",
                table: "MatchConnections",
                columns: new[] { "ConnectedUsersId", "MatchId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MatchConnections_AspNetUsers_ConnectedUsersId",
                schema: "public",
                table: "MatchConnections",
                column: "ConnectedUsersId",
                principalSchema: "public",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchConnections_matches_MatchId",
                schema: "public",
                table: "MatchConnections",
                column: "MatchId",
                principalSchema: "public",
                principalTable: "matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchConnections_AspNetUsers_ConnectedUsersId",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchConnections_matches_MatchId",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchConnections",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.RenameTable(
                name: "MatchConnections",
                schema: "public",
                newName: "MatchConnection",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_MatchConnections_MatchId",
                schema: "public",
                table: "MatchConnection",
                newName: "IX_MatchConnection_MatchId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchConnection",
                schema: "public",
                table: "MatchConnection",
                columns: new[] { "ConnectedUsersId", "MatchId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MatchConnection_AspNetUsers_ConnectedUsersId",
                schema: "public",
                table: "MatchConnection",
                column: "ConnectedUsersId",
                principalSchema: "public",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchConnection_matches_MatchId",
                schema: "public",
                table: "MatchConnection",
                column: "MatchId",
                principalSchema: "public",
                principalTable: "matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
