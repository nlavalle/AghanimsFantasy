using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class FantasyPlayerFkFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dota_fantasy_players_dota_account_id",
                schema: "nadcl",
                table: "dota_fantasy_players");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_players_dota_account_id",
                schema: "nadcl",
                table: "dota_fantasy_players",
                column: "dota_account_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_dota_fantasy_players_dota_account_id",
                schema: "nadcl",
                table: "dota_fantasy_players");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_players_dota_account_id",
                schema: "nadcl",
                table: "dota_fantasy_players",
                column: "dota_account_id",
                unique: true);
        }
    }
}
