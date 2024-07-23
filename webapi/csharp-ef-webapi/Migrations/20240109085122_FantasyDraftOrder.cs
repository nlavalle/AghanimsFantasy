using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class FantasyDraftOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DraftOrder",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DraftOrder",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");
        }
    }
}
