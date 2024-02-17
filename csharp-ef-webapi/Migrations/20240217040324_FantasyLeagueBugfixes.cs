using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class FantasyLeagueBugfixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_itempurchase_dota_gc_match_metadata_~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_itempurchase");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_player_dota_gc_match_metadata_team_G~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_player");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_playerkill_dota_gc_match_metadata_pl~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_playerkill");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_team_dota_gc_match_metadata_GcMatchM~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_team");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_tip_dota_gc_match_metadata_GcMatchMe~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_tip");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_itempurchase_dota_gc_match_metadata_~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_itempurchase",
                column: "GcMatchMetadataPlayerId",
                principalSchema: "nadcl",
                principalTable: "dota_gc_match_metadata_player",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_player_dota_gc_match_metadata_team_G~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_player",
                column: "GcMatchMetadataTeamId",
                principalSchema: "nadcl",
                principalTable: "dota_gc_match_metadata_team",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_playerkill_dota_gc_match_metadata_pl~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_playerkill",
                column: "GcMatchMetadataPlayerId",
                principalSchema: "nadcl",
                principalTable: "dota_gc_match_metadata_player",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_team_dota_gc_match_metadata_GcMatchM~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_team",
                column: "GcMatchMetadataId",
                principalSchema: "nadcl",
                principalTable: "dota_gc_match_metadata",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_tip_dota_gc_match_metadata_GcMatchMe~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_tip",
                column: "GcMatchMetadataId",
                principalSchema: "nadcl",
                principalTable: "dota_gc_match_metadata",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_itempurchase_dota_gc_match_metadata_~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_itempurchase");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_player_dota_gc_match_metadata_team_G~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_player");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_playerkill_dota_gc_match_metadata_pl~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_playerkill");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_team_dota_gc_match_metadata_GcMatchM~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_team");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_tip_dota_gc_match_metadata_GcMatchMe~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_tip");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_itempurchase_dota_gc_match_metadata_~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_itempurchase",
                column: "GcMatchMetadataPlayerId",
                principalSchema: "nadcl",
                principalTable: "dota_gc_match_metadata_player",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_player_dota_gc_match_metadata_team_G~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_player",
                column: "GcMatchMetadataTeamId",
                principalSchema: "nadcl",
                principalTable: "dota_gc_match_metadata_team",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_playerkill_dota_gc_match_metadata_pl~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_playerkill",
                column: "GcMatchMetadataPlayerId",
                principalSchema: "nadcl",
                principalTable: "dota_gc_match_metadata_player",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_team_dota_gc_match_metadata_GcMatchM~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_team",
                column: "GcMatchMetadataId",
                principalSchema: "nadcl",
                principalTable: "dota_gc_match_metadata",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_tip_dota_gc_match_metadata_GcMatchMe~",
                schema: "nadcl",
                table: "dota_gc_match_metadata_tip",
                column: "GcMatchMetadataId",
                principalSchema: "nadcl",
                principalTable: "dota_gc_match_metadata",
                principalColumn: "id");
        }
    }
}
