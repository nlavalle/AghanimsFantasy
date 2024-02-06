using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class LeagueStartEndTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_dota_leagues",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.AlterColumn<long>(
                name: "fantasy_draft_locked_date",
                schema: "nadcl",
                table: "dota_leagues",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<int>(
                name: "league_id",
                schema: "nadcl",
                table: "dota_leagues",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "id",
                schema: "nadcl",
                table: "dota_leagues",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "league_end_time",
                schema: "nadcl",
                table: "dota_leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "league_start_time",
                schema: "nadcl",
                table: "dota_leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_dota_leagues",
                schema: "nadcl",
                table: "dota_leagues",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_dota_leagues",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "league_end_time",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "league_start_time",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.AlterColumn<int>(
                name: "league_id",
                schema: "nadcl",
                table: "dota_leagues",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "fantasy_draft_locked_date",
                schema: "nadcl",
                table: "dota_leagues",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dota_leagues",
                schema: "nadcl",
                table: "dota_leagues",
                column: "league_id");
        }
    }
}
