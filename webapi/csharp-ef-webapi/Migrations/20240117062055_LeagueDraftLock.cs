using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class LeagueDraftLock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fantasy_draft_locked_date",
                schema: "nadcl",
                table: "dota_leagues",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "league_id",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fantasy_draft_locked_date",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.AlterColumn<long>(
                name: "league_id",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
