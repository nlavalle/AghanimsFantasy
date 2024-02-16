using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class GcMatchMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dota_gc_match_metadata",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    lobby_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    report_until_time = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    primary_event_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_gc_match_metadata", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dota_gc_match_metadata_team",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dota_team = table.Column<long>(type: "bigint", nullable: false),
                    cm_first_pick = table.Column<bool>(type: "boolean", nullable: false),
                    cm_captain_player_id = table.Column<int>(type: "integer", nullable: false),
                    cm_penalty = table.Column<long>(type: "bigint", nullable: false),
                    GraphExperience = table.Column<List<float>>(type: "real[]", nullable: false),
                    GraphGoldEarned = table.Column<List<float>>(type: "real[]", nullable: false),
                    GraphNetworth = table.Column<List<float>>(type: "real[]", nullable: false),
                    GcMatchMetadataId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_gc_match_metadata_team", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_gc_match_metadata_team_dota_gc_match_metadata_GcMatchM~",
                        column: x => x.GcMatchMetadataId,
                        principalSchema: "nadcl",
                        principalTable: "dota_gc_match_metadata",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "dota_gc_match_metadata_tip",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    source_player_slot = table.Column<long>(type: "bigint", nullable: false),
                    target_player_slot = table.Column<long>(type: "bigint", nullable: false),
                    tip_amount = table.Column<long>(type: "bigint", nullable: false),
                    GcMatchMetadataId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_gc_match_metadata_tip", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_gc_match_metadata_tip_dota_gc_match_metadata_GcMatchMe~",
                        column: x => x.GcMatchMetadataId,
                        principalSchema: "nadcl",
                        principalTable: "dota_gc_match_metadata",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "dota_gc_match_metadata_player",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_slot = table.Column<long>(type: "bigint", nullable: false),
                    avg_kills_x16 = table.Column<long>(type: "bigint", nullable: false),
                    avg_deaths_x16 = table.Column<long>(type: "bigint", nullable: false),
                    avg_assists_x16 = table.Column<long>(type: "bigint", nullable: false),
                    avg_gpm_x16 = table.Column<long>(type: "bigint", nullable: false),
                    avg_xpm_x16 = table.Column<long>(type: "bigint", nullable: false),
                    best_kills_x16 = table.Column<long>(type: "bigint", nullable: false),
                    best_assists_x16 = table.Column<long>(type: "bigint", nullable: false),
                    best_gpm_x16 = table.Column<long>(type: "bigint", nullable: false),
                    best_xpm_x16 = table.Column<long>(type: "bigint", nullable: false),
                    win_streak = table.Column<long>(type: "bigint", nullable: false),
                    best_win_streak = table.Column<long>(type: "bigint", nullable: false),
                    fight_score = table.Column<float>(type: "real", nullable: false),
                    farm_score = table.Column<float>(type: "real", nullable: false),
                    support_score = table.Column<float>(type: "real", nullable: false),
                    push_score = table.Column<float>(type: "real", nullable: false),
                    avg_stats_calibrated = table.Column<bool>(type: "boolean", nullable: false),
                    hero_xp = table.Column<long>(type: "bigint", nullable: false),
                    camps_stacked = table.Column<long>(type: "bigint", nullable: false),
                    lane_selection_flags = table.Column<long>(type: "bigint", nullable: false),
                    rampages = table.Column<long>(type: "bigint", nullable: false),
                    triple_kills = table.Column<long>(type: "bigint", nullable: false),
                    aegis_snatched = table.Column<long>(type: "bigint", nullable: false),
                    rapiers_purchased = table.Column<long>(type: "bigint", nullable: false),
                    couriers_killed = table.Column<long>(type: "bigint", nullable: false),
                    net_worth_rank = table.Column<long>(type: "bigint", nullable: false),
                    support_gold_spent = table.Column<long>(type: "bigint", nullable: false),
                    observer_wards_placed = table.Column<long>(type: "bigint", nullable: false),
                    sentry_wards_placed = table.Column<long>(type: "bigint", nullable: false),
                    wards_dewarded = table.Column<long>(type: "bigint", nullable: false),
                    stun_duration = table.Column<float>(type: "real", nullable: false),
                    team_slot = table.Column<long>(type: "bigint", nullable: false),
                    featured_hero_sticker_index = table.Column<long>(type: "bigint", nullable: false),
                    featured_hero_sticker_quality = table.Column<long>(type: "bigint", nullable: false),
                    game_player_id = table.Column<int>(type: "integer", nullable: false),
                    AbilityUpgrades = table.Column<List<int>>(type: "integer[]", nullable: false),
                    GcMatchMetadataTeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_gc_match_metadata_player", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_gc_match_metadata_player_dota_gc_match_metadata_team_G~",
                        column: x => x.GcMatchMetadataTeamId,
                        principalSchema: "nadcl",
                        principalTable: "dota_gc_match_metadata_team",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "dota_gc_match_metadata_itempurchase",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    item_id = table.Column<long>(type: "bigint", nullable: false),
                    purchase_time = table.Column<long>(type: "bigint", nullable: false),
                    GcMatchMetadataPlayerId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_gc_match_metadata_itempurchase", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_gc_match_metadata_itempurchase_dota_gc_match_metadata_~",
                        column: x => x.GcMatchMetadataPlayerId,
                        principalSchema: "nadcl",
                        principalTable: "dota_gc_match_metadata_player",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "dota_gc_match_metadata_playerkill",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    victim_slot = table.Column<long>(type: "bigint", nullable: false),
                    count = table.Column<long>(type: "bigint", nullable: false),
                    GcMatchMetadataPlayerId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_gc_match_metadata_playerkill", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_gc_match_metadata_playerkill_dota_gc_match_metadata_pl~",
                        column: x => x.GcMatchMetadataPlayerId,
                        principalSchema: "nadcl",
                        principalTable: "dota_gc_match_metadata_player",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_metadata_itempurchase_GcMatchMetadataPlayerId",
                schema: "nadcl",
                table: "dota_gc_match_metadata_itempurchase",
                column: "GcMatchMetadataPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_metadata_player_GcMatchMetadataTeamId",
                schema: "nadcl",
                table: "dota_gc_match_metadata_player",
                column: "GcMatchMetadataTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_metadata_playerkill_GcMatchMetadataPlayerId",
                schema: "nadcl",
                table: "dota_gc_match_metadata_playerkill",
                column: "GcMatchMetadataPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_metadata_team_GcMatchMetadataId",
                schema: "nadcl",
                table: "dota_gc_match_metadata_team",
                column: "GcMatchMetadataId");

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_metadata_tip_GcMatchMetadataId",
                schema: "nadcl",
                table: "dota_gc_match_metadata_tip",
                column: "GcMatchMetadataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dota_gc_match_metadata_itempurchase",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_gc_match_metadata_playerkill",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_gc_match_metadata_tip",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_gc_match_metadata_player",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_gc_match_metadata_team",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_gc_match_metadata",
                schema: "nadcl");
        }
    }
}
