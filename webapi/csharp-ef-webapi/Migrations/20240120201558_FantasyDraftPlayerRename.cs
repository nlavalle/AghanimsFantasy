using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class FantasyDraftPlayerRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_drafts_FantasyDraft~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_players_FantasyPlay~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");

            migrationBuilder.RenameColumn(
                name: "DraftOrder",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                newName: "draft_order");

            migrationBuilder.RenameColumn(
                name: "FantasyDraftId",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                newName: "fantasy_draft_id");

            migrationBuilder.RenameColumn(
                name: "FantasyPlayerId",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                newName: "fantasy_player_id");

            migrationBuilder.RenameIndex(
                name: "IX_dota_fantasy_draft_players_FantasyDraftId",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                newName: "IX_dota_fantasy_draft_players_fantasy_draft_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_drafts_fantasy_draf~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                column: "fantasy_draft_id",
                principalSchema: "nadcl",
                principalTable: "dota_fantasy_drafts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_players_fantasy_pla~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                column: "fantasy_player_id",
                principalSchema: "nadcl",
                principalTable: "dota_fantasy_players",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_drafts_fantasy_draf~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_players_fantasy_pla~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");

            migrationBuilder.RenameColumn(
                name: "draft_order",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                newName: "DraftOrder");

            migrationBuilder.RenameColumn(
                name: "fantasy_draft_id",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                newName: "FantasyDraftId");

            migrationBuilder.RenameColumn(
                name: "fantasy_player_id",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                newName: "FantasyPlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_dota_fantasy_draft_players_fantasy_draft_id",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                newName: "IX_dota_fantasy_draft_players_FantasyDraftId");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_drafts_FantasyDraft~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                column: "FantasyDraftId",
                principalSchema: "nadcl",
                principalTable: "dota_fantasy_drafts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_players_FantasyPlay~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                column: "FantasyPlayerId",
                principalSchema: "nadcl",
                principalTable: "dota_fantasy_players",
                principalColumn: "id");
        }
    }
}
