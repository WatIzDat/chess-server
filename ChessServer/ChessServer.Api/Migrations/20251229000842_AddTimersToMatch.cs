using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessServer.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTimersToMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BlackTimeRemaining",
                schema: "public",
                table: "Matches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LastTurnStartTimestamp",
                schema: "public",
                table: "Matches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WhiteTimeRemaining",
                schema: "public",
                table: "Matches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlackTimeRemaining",
                schema: "public",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "LastTurnStartTimestamp",
                schema: "public",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "WhiteTimeRemaining",
                schema: "public",
                table: "Matches");
        }
    }
}
