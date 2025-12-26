using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessServer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerTypeToMatchConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchConnections_matches_MatchId",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_matches",
                schema: "public",
                table: "matches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchConnections",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropIndex(
                name: "IX_MatchConnections_UserId",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.RenameTable(
                name: "matches",
                schema: "public",
                newName: "Matches",
                newSchema: "public");

            migrationBuilder.AddColumn<int>(
                name: "PlayerType",
                schema: "public",
                table: "MatchConnections",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Matches",
                schema: "public",
                table: "Matches",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchConnections",
                schema: "public",
                table: "MatchConnections",
                columns: new[] { "UserId", "MatchId" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchConnections_MatchId",
                schema: "public",
                table: "MatchConnections",
                column: "MatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchConnections_Matches_MatchId",
                schema: "public",
                table: "MatchConnections",
                column: "MatchId",
                principalSchema: "public",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchConnections_Matches_MatchId",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Matches",
                schema: "public",
                table: "Matches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchConnections",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropIndex(
                name: "IX_MatchConnections_MatchId",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropColumn(
                name: "PlayerType",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.RenameTable(
                name: "Matches",
                schema: "public",
                newName: "matches",
                newSchema: "public");

            migrationBuilder.AddPrimaryKey(
                name: "PK_matches",
                schema: "public",
                table: "matches",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchConnections",
                schema: "public",
                table: "MatchConnections",
                columns: new[] { "MatchId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchConnections_UserId",
                schema: "public",
                table: "MatchConnections",
                column: "UserId");

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
    }
}
