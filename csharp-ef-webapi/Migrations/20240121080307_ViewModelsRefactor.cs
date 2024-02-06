using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class ViewModelsRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "draft_pick_five",
                schema: "nadcl",
                table: "dota_fantasy_drafts");

            migrationBuilder.DropColumn(
                name: "draft_pick_four",
                schema: "nadcl",
                table: "dota_fantasy_drafts");

            migrationBuilder.DropColumn(
                name: "draft_pick_one",
                schema: "nadcl",
                table: "dota_fantasy_drafts");

            migrationBuilder.DropColumn(
                name: "draft_pick_three",
                schema: "nadcl",
                table: "dota_fantasy_drafts");

            migrationBuilder.DropColumn(
                name: "draft_pick_two",
                schema: "nadcl",
                table: "dota_fantasy_drafts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "draft_pick_five",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "draft_pick_four",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "draft_pick_one",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "draft_pick_three",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "draft_pick_two",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                type: "bigint",
                nullable: true);
        }
    }
}
