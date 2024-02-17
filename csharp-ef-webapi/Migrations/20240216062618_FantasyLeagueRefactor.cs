using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class FantasyLeagueRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fantasy_draft_locked_date",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "league_end_time",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "league_id",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "league_start_time",
                schema: "nadcl",
                table: "dota_leagues");

            migrationBuilder.DropColumn(
                name: "league_id",
                schema: "nadcl",
                table: "dota_fantasy_players");

            migrationBuilder.RenameColumn(
                name: "league_id",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                newName: "fantasy_league_id");

            migrationBuilder.AddColumn<int>(
                name: "fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "dota_fantasy_leagues",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    league_id = table.Column<int>(type: "integer", nullable: false),
                    league_name = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    fantasy_draft_locked_date = table.Column<long>(type: "bigint", nullable: false),
                    league_start_time = table.Column<long>(type: "bigint", nullable: false),
                    league_end_time = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_fantasy_leagues", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_fantasy_leagues_dota_leagues_league_id",
                        column: x => x.league_id,
                        principalSchema: "nadcl",
                        principalTable: "dota_leagues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_match_history_league_id",
                schema: "nadcl",
                table: "dota_match_history",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_match_details_league_id",
                schema: "nadcl",
                table: "dota_match_details",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_metadata_match_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                column: "match_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_players_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_players",
                column: "fantasy_league_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_drafts_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                column: "fantasy_league_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_leagues_league_id",
                schema: "nadcl",
                table: "dota_fantasy_leagues",
                column: "league_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_drafts_dota_fantasy_leagues_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                column: "fantasy_league_id",
                principalSchema: "nadcl",
                principalTable: "dota_fantasy_leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_players_dota_fantasy_leagues_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_players",
                column: "fantasy_league_id",
                principalSchema: "nadcl",
                principalTable: "dota_fantasy_leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_dota_match_details_match_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                column: "match_id",
                principalSchema: "nadcl",
                principalTable: "dota_match_details",
                principalColumn: "match_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_match_details_dota_leagues_league_id",
                schema: "nadcl",
                table: "dota_match_details",
                column: "league_id",
                principalSchema: "nadcl",
                principalTable: "dota_leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_match_history_dota_leagues_league_id",
                schema: "nadcl",
                table: "dota_match_history",
                column: "league_id",
                principalSchema: "nadcl",
                principalTable: "dota_leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_drafts_dota_fantasy_leagues_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_drafts");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_players_dota_fantasy_leagues_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_players");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_dota_match_details_match_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_match_details_dota_leagues_league_id",
                schema: "nadcl",
                table: "dota_match_details");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_match_history_dota_leagues_league_id",
                schema: "nadcl",
                table: "dota_match_history");

            migrationBuilder.DropTable(
                name: "dota_fantasy_leagues",
                schema: "nadcl");

            migrationBuilder.DropIndex(
                name: "IX_dota_match_history_league_id",
                schema: "nadcl",
                table: "dota_match_history");

            migrationBuilder.DropIndex(
                name: "IX_dota_match_details_league_id",
                schema: "nadcl",
                table: "dota_match_details");

            migrationBuilder.DropIndex(
                name: "IX_dota_gc_match_metadata_match_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata");

            migrationBuilder.DropIndex(
                name: "IX_dota_fantasy_players_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_players");

            migrationBuilder.DropIndex(
                name: "IX_dota_fantasy_drafts_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_drafts");

            migrationBuilder.DropColumn(
                name: "fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_players");

            migrationBuilder.RenameColumn(
                name: "fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                newName: "league_id");

            migrationBuilder.AddColumn<long>(
                name: "fantasy_draft_locked_date",
                schema: "nadcl",
                table: "dota_leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "league_end_time",
                schema: "nadcl",
                table: "dota_leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "league_id",
                schema: "nadcl",
                table: "dota_leagues",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "league_start_time",
                schema: "nadcl",
                table: "dota_leagues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "league_id",
                schema: "nadcl",
                table: "dota_fantasy_players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
