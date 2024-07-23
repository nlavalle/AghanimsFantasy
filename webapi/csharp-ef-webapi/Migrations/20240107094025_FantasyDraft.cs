using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class FantasyDraft : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dota_fantasy_drafts",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    league_id = table.Column<long>(type: "bigint", nullable: false),
                    discord_account_id = table.Column<long>(type: "bigint", nullable: true),
                    draft_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    draft_last_updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    draft_pick_one = table.Column<long>(type: "bigint", nullable: false),
                    draft_pick_two = table.Column<long>(type: "bigint", nullable: false),
                    draft_pick_three = table.Column<long>(type: "bigint", nullable: false),
                    draft_pick_four = table.Column<long>(type: "bigint", nullable: false),
                    draft_pick_five = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_fantasy_drafts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dota_fantasy_players",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    league_id = table.Column<long>(type: "bigint", nullable: false),
                    team_id = table.Column<long>(type: "bigint", nullable: false),
                    dota_account_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_fantasy_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_fantasy_players_dota_accounts_dota_account_id",
                        column: x => x.dota_account_id,
                        principalSchema: "nadcl",
                        principalTable: "dota_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dota_fantasy_players_dota_teams_team_id",
                        column: x => x.team_id,
                        principalSchema: "nadcl",
                        principalTable: "dota_teams",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "dota_fantasy_draft_players",
                schema: "nadcl",
                columns: table => new
                {
                    FantasyDraftId = table.Column<long>(type: "bigint", nullable: false),
                    FantasyPlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_fantasy_draft_players", x => new { x.FantasyPlayerId, x.FantasyDraftId });
                    table.ForeignKey(
                        name: "FK_dota_fantasy_draft_players_dota_fantasy_drafts_FantasyDraft~",
                        column: x => x.FantasyDraftId,
                        principalSchema: "nadcl",
                        principalTable: "dota_fantasy_drafts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dota_fantasy_draft_players_dota_fantasy_players_FantasyPlay~",
                        column: x => x.FantasyPlayerId,
                        principalSchema: "nadcl",
                        principalTable: "dota_fantasy_players",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_draft_players_FantasyDraftId",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                column: "FantasyDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_players_dota_account_id",
                schema: "nadcl",
                table: "dota_fantasy_players",
                column: "dota_account_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_players_team_id",
                schema: "nadcl",
                table: "dota_fantasy_players",
                column: "team_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dota_fantasy_draft_players",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_fantasy_drafts",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_fantasy_players",
                schema: "nadcl");
        }
    }
}
