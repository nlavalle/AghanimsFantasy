using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class MatchDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dota_match_details",
                schema: "Kali",
                columns: table => new
                {
                    match_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    radiant_win = table.Column<bool>(type: "boolean", nullable: false),
                    duration = table.Column<int>(type: "integer", nullable: false),
                    pre_game_duration = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<long>(type: "bigint", nullable: false),
                    match_seq_num = table.Column<long>(type: "bigint", nullable: false),
                    tower_status_radiant = table.Column<int>(type: "integer", nullable: false),
                    tower_status_dire = table.Column<int>(type: "integer", nullable: false),
                    barracks_status_radiant = table.Column<int>(type: "integer", nullable: false),
                    barracks_status_dire = table.Column<int>(type: "integer", nullable: false),
                    cluster = table.Column<int>(type: "integer", nullable: false),
                    first_blood_time = table.Column<int>(type: "integer", nullable: false),
                    lobby_type = table.Column<int>(type: "integer", nullable: false),
                    human_players = table.Column<int>(type: "integer", nullable: false),
                    league_id = table.Column<int>(type: "integer", nullable: false),
                    game_mode = table.Column<int>(type: "integer", nullable: false),
                    flags = table.Column<int>(type: "integer", nullable: false),
                    engine = table.Column<int>(type: "integer", nullable: false),
                    radiant_score = table.Column<int>(type: "integer", nullable: false),
                    dire_score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_match_details", x => x.match_id);
                });

            migrationBuilder.CreateTable(
                name: "dota_match_history_players",
                schema: "Kali",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    player_slot = table.Column<int>(type: "integer", nullable: false),
                    team_number = table.Column<int>(type: "integer", nullable: false),
                    team_slot = table.Column<int>(type: "integer", nullable: false),
                    hero_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_match_history_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_match_history_players_dota_match_history_match_id",
                        column: x => x.match_id,
                        principalSchema: "Kali",
                        principalTable: "dota_match_history",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dota_match_details_picks_bans",
                schema: "Kali",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    is_pick = table.Column<bool>(type: "boolean", nullable: false),
                    hero_id = table.Column<int>(type: "integer", nullable: false),
                    team = table.Column<int>(type: "integer", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_match_details_picks_bans", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_match_details_picks_bans_dota_match_details_match_id",
                        column: x => x.match_id,
                        principalSchema: "Kali",
                        principalTable: "dota_match_details",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dota_match_details_players",
                schema: "Kali",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    player_slot = table.Column<int>(type: "integer", nullable: false),
                    team_number = table.Column<int>(type: "integer", nullable: true),
                    team_slot = table.Column<int>(type: "integer", nullable: true),
                    hero_id = table.Column<int>(type: "integer", nullable: false),
                    item_0 = table.Column<int>(type: "integer", nullable: true),
                    item_1 = table.Column<int>(type: "integer", nullable: true),
                    item_2 = table.Column<int>(type: "integer", nullable: true),
                    item_3 = table.Column<int>(type: "integer", nullable: true),
                    item_4 = table.Column<int>(type: "integer", nullable: true),
                    item_5 = table.Column<int>(type: "integer", nullable: true),
                    backpack_0 = table.Column<int>(type: "integer", nullable: true),
                    backpack_1 = table.Column<int>(type: "integer", nullable: true),
                    backpack_2 = table.Column<int>(type: "integer", nullable: true),
                    item_neutral = table.Column<int>(type: "integer", nullable: true),
                    kills = table.Column<int>(type: "integer", nullable: true),
                    deaths = table.Column<int>(type: "integer", nullable: true),
                    assists = table.Column<int>(type: "integer", nullable: true),
                    leaver_status = table.Column<int>(type: "integer", nullable: true),
                    last_hits = table.Column<int>(type: "integer", nullable: true),
                    denies = table.Column<int>(type: "integer", nullable: true),
                    gold_per_min = table.Column<int>(type: "integer", nullable: true),
                    xp_per_min = table.Column<int>(type: "integer", nullable: true),
                    level = table.Column<int>(type: "integer", nullable: true),
                    net_worth = table.Column<long>(type: "bigint", nullable: true),
                    aghanims_scepter = table.Column<int>(type: "integer", nullable: true),
                    aghanims_shard = table.Column<int>(type: "integer", nullable: true),
                    moonshard = table.Column<int>(type: "integer", nullable: true),
                    hero_damage = table.Column<int>(type: "integer", nullable: true),
                    tower_damage = table.Column<int>(type: "integer", nullable: true),
                    hero_healing = table.Column<int>(type: "integer", nullable: true),
                    gold = table.Column<int>(type: "integer", nullable: true),
                    gold_spent = table.Column<int>(type: "integer", nullable: true),
                    scaled_hero_damage = table.Column<int>(type: "integer", nullable: true),
                    scaled_tower_damage = table.Column<int>(type: "integer", nullable: true),
                    scaled_hero_healing = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_match_details_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_match_details_players_dota_match_details_match_id",
                        column: x => x.match_id,
                        principalSchema: "Kali",
                        principalTable: "dota_match_details",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dota_match_details_players_ability_upgrades",
                schema: "Kali",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    ability = table.Column<int>(type: "integer", nullable: false),
                    time = table.Column<int>(type: "integer", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_match_details_players_ability_upgrades", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_match_details_players_ability_upgrades_dota_match_deta~",
                        column: x => x.player_id,
                        principalSchema: "Kali",
                        principalTable: "dota_match_details_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_match_details_picks_bans_match_id",
                schema: "Kali",
                table: "dota_match_details_picks_bans",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_match_details_players_match_id",
                schema: "Kali",
                table: "dota_match_details_players",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_match_details_players_ability_upgrades_player_id",
                schema: "Kali",
                table: "dota_match_details_players_ability_upgrades",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_match_history_players_match_id",
                schema: "Kali",
                table: "dota_match_history_players",
                column: "match_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dota_match_details_picks_bans",
                schema: "Kali");

            migrationBuilder.DropTable(
                name: "dota_match_details_players_ability_upgrades",
                schema: "Kali");

            migrationBuilder.DropTable(
                name: "dota_match_history_players",
                schema: "Kali");

            migrationBuilder.DropTable(
                name: "dota_match_details_players",
                schema: "Kali");

            migrationBuilder.DropTable(
                name: "dota_match_details",
                schema: "Kali");
        }
    }
}
