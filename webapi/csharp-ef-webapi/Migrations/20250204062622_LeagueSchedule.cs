using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class LeagueSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "end_timestamp",
                schema: "nadcl",
                table: "dota_leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "is_scheduled",
                schema: "nadcl",
                table: "dota_leagues",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "most_recent_activity",
                schema: "nadcl",
                table: "dota_leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "region",
                schema: "nadcl",
                table: "dota_leagues",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "start_timestamp",
                schema: "nadcl",
                table: "dota_leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "status",
                schema: "nadcl",
                table: "dota_leagues",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "tier",
                schema: "nadcl",
                table: "dota_leagues",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "total_prize_pool",
                schema: "nadcl",
                table: "dota_leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "end_timestamp",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "is_scheduled",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "most_recent_activity",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "region",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "start_timestamp",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "tier",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "total_prize_pool",
                schema: "nadcl",
                table: "dota_leagues");
        }
    }
}
