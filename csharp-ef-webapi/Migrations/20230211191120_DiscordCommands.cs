using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class DiscordCommands : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "match_status",
                schema: "nadcl",
                columns: table => new
                {
                    match_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_match_status", x => x.match_id);
                });

            migrationBuilder.CreateTable(
                name: "player_match_details",
                schema: "nadcl",
                columns: table => new
                {
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    player_slot = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    hero_id = table.Column<int>(type: "integer", nullable: false),
                    item_0 = table.Column<int>(type: "integer", nullable: false),
                    item_1 = table.Column<int>(type: "integer", nullable: false),
                    item_2 = table.Column<int>(type: "integer", nullable: false),
                    item_3 = table.Column<int>(type: "integer", nullable: false),
                    item_4 = table.Column<int>(type: "integer", nullable: false),
                    item_5 = table.Column<int>(type: "integer", nullable: false),
                    backpack_0 = table.Column<int>(type: "integer", nullable: false),
                    backpack_1 = table.Column<int>(type: "integer", nullable: false),
                    backpack_2 = table.Column<int>(type: "integer", nullable: false),
                    item_neutral = table.Column<int>(type: "integer", nullable: false),
                    kills = table.Column<int>(type: "integer", nullable: false),
                    deaths = table.Column<int>(type: "integer", nullable: false),
                    assists = table.Column<int>(type: "integer", nullable: false),
                    leaver_status = table.Column<int>(type: "integer", nullable: false),
                    last_hits = table.Column<int>(type: "integer", nullable: false),
                    denies = table.Column<int>(type: "integer", nullable: false),
                    gold_per_min = table.Column<int>(type: "integer", nullable: false),
                    xp_per_min = table.Column<int>(type: "integer", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    hero_damage = table.Column<int>(type: "integer", nullable: false),
                    tower_damage = table.Column<int>(type: "integer", nullable: false),
                    hero_healing = table.Column<int>(type: "integer", nullable: false),
                    gold = table.Column<int>(type: "integer", nullable: false),
                    gold_spent = table.Column<int>(type: "integer", nullable: false),
                    scaled_hero_damage = table.Column<int>(type: "integer", nullable: false),
                    scaled_tower_damage = table.Column<int>(type: "integer", nullable: false),
                    scaled_hero_healing = table.Column<int>(type: "integer", nullable: false),
                    aghanims_scepter = table.Column<int>(type: "integer", nullable: false),
                    aghanims_shard = table.Column<int>(type: "integer", nullable: false),
                    moonshard = table.Column<int>(type: "integer", nullable: false),
                    net_worth = table.Column<long>(type: "bigint", nullable: false),
                    team_number = table.Column<int>(type: "integer", nullable: false),
                    team_slot = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_match_details", x => new { x.match_id, x.player_slot });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "match_status",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "player_match_details",
                schema: "nadcl");
        }
    }
}
