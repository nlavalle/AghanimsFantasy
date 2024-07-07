using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class FantasyNormalizedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dota_fantasy_normalized_averages",
                schema: "nadcl",
                columns: table => new
                {
                    fantasy_normalized_averages_table_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fantasy_player_id = table.Column<long>(type: "bigint", nullable: false),
                    matches_played = table.Column<decimal>(type: "numeric", nullable: true),
                    kills_points = table.Column<decimal>(type: "numeric", nullable: true),
                    deaths_points = table.Column<decimal>(type: "numeric", nullable: true),
                    assists_points = table.Column<decimal>(type: "numeric", nullable: true),
                    last_hits_points = table.Column<decimal>(type: "numeric", nullable: true),
                    gold_per_min_points = table.Column<decimal>(type: "numeric", nullable: true),
                    xp_per_min_points = table.Column<decimal>(type: "numeric", nullable: true),
                    networth_points = table.Column<decimal>(type: "numeric", nullable: true),
                    hero_damage_points = table.Column<decimal>(type: "numeric", nullable: true),
                    tower_damage_points = table.Column<decimal>(type: "numeric", nullable: true),
                    hero_healing_points = table.Column<decimal>(type: "numeric", nullable: true),
                    gold_points = table.Column<decimal>(type: "numeric", nullable: true),
                    fight_score = table.Column<float>(type: "real", nullable: true),
                    farm_score = table.Column<float>(type: "real", nullable: true),
                    support_score = table.Column<float>(type: "real", nullable: true),
                    push_score = table.Column<float>(type: "real", nullable: true),
                    hero_xp_points = table.Column<decimal>(type: "numeric", nullable: true),
                    camps_stacked_points = table.Column<decimal>(type: "numeric", nullable: true),
                    rampages_points = table.Column<decimal>(type: "numeric", nullable: true),
                    triple_kills_points = table.Column<decimal>(type: "numeric", nullable: true),
                    aegis_snatched_points = table.Column<decimal>(type: "numeric", nullable: true),
                    rapiers_purchased_points = table.Column<decimal>(type: "numeric", nullable: true),
                    couriers_killed_points = table.Column<decimal>(type: "numeric", nullable: true),
                    support_gold_spent_points = table.Column<decimal>(type: "numeric", nullable: true),
                    observer_wards_placed_points = table.Column<decimal>(type: "numeric", nullable: true),
                    sentry_wards_placed_points = table.Column<decimal>(type: "numeric", nullable: true),
                    wards_dewarded_points = table.Column<decimal>(type: "numeric", nullable: true),
                    stun_duration_points = table.Column<float>(type: "real", nullable: true),
                    total_match_fantasy_points = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_fantasy_normalized_averages", x => x.fantasy_normalized_averages_table_id);
                    table.ForeignKey(
                        name: "FK_dota_fantasy_normalized_averages_dota_fantasy_players_fanta~",
                        column: x => x.fantasy_player_id,
                        principalSchema: "nadcl",
                        principalTable: "dota_fantasy_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_normalized_averages_fantasy_player_id",
                schema: "nadcl",
                table: "dota_fantasy_normalized_averages",
                column: "fantasy_player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dota_fantasy_normalized_averages",
                schema: "nadcl");
        }
    }
}
