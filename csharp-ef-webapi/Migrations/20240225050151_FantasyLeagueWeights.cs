using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class FantasyLeagueWeights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dota_fantasy_league_weights",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fantasy_league_id = table.Column<int>(type: "integer", nullable: false),
                    kills_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    deaths_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    assists_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    last_hits_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    gold_per_min_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    xp_per_min_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    networth_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    hero_damage_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    tower_damage_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    hero_healing_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    gold_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    scaled_hero_damage_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    scaled_tower_damage_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    scaled_hero_healing_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    fight_score_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    farm_score_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    support_score_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    push_score_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    hero_xp_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    camps_stacked_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    rampages_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    triple_kills_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    aegis_snatched_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    rapiers_purchased_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    couriers_killed_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    networth_rank_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    support_gold_spent_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    observer_wards_placed_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    sentry_wards_placed_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    wards_dewarded_weight = table.Column<decimal>(type: "numeric", nullable: false),
                    stun_duration_weight = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_fantasy_league_weights", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_fantasy_league_weights_dota_fantasy_leagues_fantasy_le~",
                        column: x => x.fantasy_league_id,
                        principalSchema: "nadcl",
                        principalTable: "dota_fantasy_leagues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_league_weights_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_league_weights",
                column: "fantasy_league_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dota_fantasy_league_weights",
                schema: "nadcl");
        }
    }
}
