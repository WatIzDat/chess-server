using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessServer.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixMatchConnectionForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchConnections_AspNetUsers_ConnectedUsersId",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchConnections",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropIndex(
                name: "IX_MatchConnections_MatchId",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropColumn(
                name: "ConnectedUsersId",
                schema: "public",
                table: "MatchConnections");

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
                name: "FK_MatchConnections_AspNetUsers_UserId",
                schema: "public",
                table: "MatchConnections",
                column: "UserId",
                principalSchema: "public",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchConnections_AspNetUsers_UserId",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchConnections",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.DropIndex(
                name: "IX_MatchConnections_UserId",
                schema: "public",
                table: "MatchConnections");

            migrationBuilder.AddColumn<string>(
                name: "ConnectedUsersId",
                schema: "public",
                table: "MatchConnections",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchConnections",
                schema: "public",
                table: "MatchConnections",
                columns: new[] { "ConnectedUsersId", "MatchId" });

            migrationBuilder.CreateIndex(
                name: "IX_MatchConnections_MatchId",
                schema: "public",
                table: "MatchConnections",
                column: "MatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchConnections_AspNetUsers_ConnectedUsersId",
                schema: "public",
                table: "MatchConnections",
                column: "ConnectedUsersId",
                principalSchema: "public",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
