using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class FantasyMatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_dota_match_details_match_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata");

            migrationBuilder.DropTable(
                name: "balance_ledger",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "bets_streaks",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "bromance_last_60",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "discord_ids",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "match_status",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "matches_streaks",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "player_match_details",
                schema: "nadcl");

            migrationBuilder.DropIndex(
                name: "IX_dota_gc_match_metadata_match_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gc_dota_matches",
                schema: "nadcl",
                table: "gc_dota_matches");

            migrationBuilder.RenameTable(
                name: "gc_dota_matches",
                schema: "nadcl",
                newName: "dota_gc_match_details",
                newSchema: "nadcl");

            migrationBuilder.AddColumn<int>(
                name: "LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_dota_gc_match_details",
                schema: "nadcl",
                table: "dota_gc_match_details",
                column: "match_id");

            migrationBuilder.CreateTable(
                name: "discord_users",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discord_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dota_gc_match_detail_players",
                schema: "nadcl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    player_slot = table.Column<long>(type: "bigint", nullable: false),
                    hero_id = table.Column<long>(type: "bigint", nullable: false),
                    item_0 = table.Column<int>(type: "integer", nullable: false),
                    item_1 = table.Column<int>(type: "integer", nullable: false),
                    item_2 = table.Column<int>(type: "integer", nullable: false),
                    item_3 = table.Column<int>(type: "integer", nullable: false),
                    item_4 = table.Column<int>(type: "integer", nullable: false),
                    item_5 = table.Column<int>(type: "integer", nullable: false),
                    item_6 = table.Column<int>(type: "integer", nullable: false),
                    item_7 = table.Column<int>(type: "integer", nullable: false),
                    item_8 = table.Column<int>(type: "integer", nullable: false),
                    item_9 = table.Column<int>(type: "integer", nullable: false),
                    expected_team_contribution = table.Column<float>(type: "real", nullable: false),
                    scaled_metric = table.Column<float>(type: "real", nullable: false),
                    previous_rank = table.Column<long>(type: "bigint", nullable: false),
                    rank_change = table.Column<int>(type: "integer", nullable: false),
                    mmr_type = table.Column<long>(type: "bigint", nullable: false),
                    kills = table.Column<long>(type: "bigint", nullable: false),
                    deaths = table.Column<long>(type: "bigint", nullable: false),
                    assists = table.Column<long>(type: "bigint", nullable: false),
                    leaver_status = table.Column<long>(type: "bigint", nullable: false),
                    gold = table.Column<long>(type: "bigint", nullable: false),
                    last_hits = table.Column<long>(type: "bigint", nullable: false),
                    denies = table.Column<long>(type: "bigint", nullable: false),
                    gold_per_min = table.Column<long>(type: "bigint", nullable: false),
                    xp_per_min = table.Column<long>(type: "bigint", nullable: false),
                    gold_spent = table.Column<long>(type: "bigint", nullable: false),
                    hero_damage = table.Column<long>(type: "bigint", nullable: false),
                    tower_damage = table.Column<long>(type: "bigint", nullable: false),
                    hero_healing = table.Column<long>(type: "bigint", nullable: false),
                    level = table.Column<long>(type: "bigint", nullable: false),
                    time_last_seen = table.Column<long>(type: "bigint", nullable: false),
                    player_name = table.Column<string>(type: "text", nullable: true),
                    support_ability_value = table.Column<long>(type: "bigint", nullable: false),
                    feeding_detected = table.Column<bool>(type: "boolean", nullable: false),
                    search_rank = table.Column<long>(type: "bigint", nullable: false),
                    search_rank_uncertainty = table.Column<long>(type: "bigint", nullable: false),
                    rank_uncertainty_change = table.Column<int>(type: "integer", nullable: false),
                    hero_play_count = table.Column<long>(type: "bigint", nullable: false),
                    party_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    scaled_hero_damage = table.Column<long>(type: "bigint", nullable: false),
                    scaled_tower_damage = table.Column<long>(type: "bigint", nullable: false),
                    scaled_hero_healing = table.Column<long>(type: "bigint", nullable: false),
                    scaled_kills = table.Column<float>(type: "real", nullable: false),
                    scaled_deaths = table.Column<float>(type: "real", nullable: false),
                    scaled_assists = table.Column<float>(type: "real", nullable: false),
                    claimed_farm_gold = table.Column<long>(type: "bigint", nullable: false),
                    support_gold = table.Column<long>(type: "bigint", nullable: false),
                    claimed_denies = table.Column<long>(type: "bigint", nullable: false),
                    claimed_misses = table.Column<long>(type: "bigint", nullable: false),
                    misses = table.Column<long>(type: "bigint", nullable: false),
                    pro_name = table.Column<string>(type: "text", nullable: true),
                    real_name = table.Column<string>(type: "text", nullable: true),
                    active_plus_subscription = table.Column<bool>(type: "boolean", nullable: false),
                    net_worth = table.Column<long>(type: "bigint", nullable: false),
                    bot_difficulty = table.Column<long>(type: "bigint", nullable: false),
                    hero_pick_order = table.Column<long>(type: "bigint", nullable: false),
                    hero_was_randomed = table.Column<bool>(type: "boolean", nullable: false),
                    hero_was_dota_plus_suggestion = table.Column<bool>(type: "boolean", nullable: false),
                    seconds_dead = table.Column<long>(type: "bigint", nullable: false),
                    gold_lost_to_death = table.Column<long>(type: "bigint", nullable: false),
                    lane_selection_flags = table.Column<long>(type: "bigint", nullable: false),
                    bounty_runes = table.Column<long>(type: "bigint", nullable: false),
                    outposts_captured = table.Column<long>(type: "bigint", nullable: false),
                    team_number = table.Column<int>(type: "integer", nullable: false),
                    team_slot = table.Column<long>(type: "bigint", nullable: false),
                    MatchId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_gc_match_detail_players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dota_gc_match_detail_players_dota_gc_match_details_MatchId",
                        column: x => x.MatchId,
                        principalSchema: "nadcl",
                        principalTable: "dota_gc_match_details",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fantasy_match",
                schema: "nadcl",
                columns: table => new
                {
                    match_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    league_id = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<long>(type: "bigint", nullable: false),
                    match_history_parsed = table.Column<bool>(type: "boolean", nullable: false),
                    match_detail_parsed = table.Column<bool>(type: "boolean", nullable: false),
                    gc_metadata_parsed = table.Column<bool>(type: "boolean", nullable: false),
                    lobby_type = table.Column<int>(type: "integer", nullable: false),
                    RadiantTeamId = table.Column<long>(type: "bigint", nullable: true),
                    DireTeamId = table.Column<long>(type: "bigint", nullable: true),
                    radiant_win = table.Column<bool>(type: "boolean", nullable: true),
                    duration = table.Column<int>(type: "integer", nullable: true),
                    pre_game_duration = table.Column<int>(type: "integer", nullable: true),
                    first_blood_time = table.Column<int>(type: "integer", nullable: true),
                    game_mode = table.Column<int>(type: "integer", nullable: true),
                    radiant_score = table.Column<int>(type: "integer", nullable: true),
                    dire_score = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fantasy_match", x => x.match_id);
                    table.ForeignKey(
                        name: "FK_fantasy_match_dota_teams_DireTeamId",
                        column: x => x.DireTeamId,
                        principalSchema: "nadcl",
                        principalTable: "dota_teams",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_fantasy_match_dota_teams_RadiantTeamId",
                        column: x => x.RadiantTeamId,
                        principalSchema: "nadcl",
                        principalTable: "dota_teams",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "dota_gc_match_detail_player_damage_dealt",
                schema: "nadcl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pre_reduction = table.Column<long>(type: "bigint", nullable: false),
                    post_reduction = table.Column<long>(type: "bigint", nullable: false),
                    damage_type = table.Column<int>(type: "integer", nullable: false),
                    MatchPlayerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_gc_match_detail_player_damage_dealt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dota_gc_match_detail_player_damage_dealt_dota_gc_match_deta~",
                        column: x => x.MatchPlayerId,
                        principalSchema: "nadcl",
                        principalTable: "dota_gc_match_detail_players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dota_gc_match_detail_player_damage_received",
                schema: "nadcl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    pre_reduction = table.Column<long>(type: "bigint", nullable: false),
                    post_reduction = table.Column<long>(type: "bigint", nullable: false),
                    damage_type = table.Column<int>(type: "integer", nullable: false),
                    MatchPlayerId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_gc_match_detail_player_damage_received", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dota_gc_match_detail_player_damage_received_dota_gc_match_d~",
                        column: x => x.MatchPlayerId,
                        principalSchema: "nadcl",
                        principalTable: "dota_gc_match_detail_players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fantasy_match_player",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    match_detail_player_parsed = table.Column<bool>(type: "boolean", nullable: false),
                    gc_metadata_player_parsed = table.Column<bool>(type: "boolean", nullable: false),
                    dota_team_side = table.Column<bool>(type: "boolean", nullable: false),
                    player_slot = table.Column<int>(type: "integer", nullable: false),
                    HeroId = table.Column<long>(type: "bigint", nullable: true),
                    kills = table.Column<int>(type: "integer", nullable: true),
                    deaths = table.Column<int>(type: "integer", nullable: true),
                    assists = table.Column<int>(type: "integer", nullable: true),
                    last_hits = table.Column<int>(type: "integer", nullable: true),
                    denies = table.Column<int>(type: "integer", nullable: true),
                    gold_per_min = table.Column<int>(type: "integer", nullable: true),
                    xp_per_min = table.Column<int>(type: "integer", nullable: true),
                    support_gold_spent = table.Column<int>(type: "integer", nullable: true),
                    observer_wards_placed = table.Column<int>(type: "integer", nullable: true),
                    sentry_wards_placed = table.Column<int>(type: "integer", nullable: true),
                    dewards = table.Column<int>(type: "integer", nullable: true),
                    camps_stacked = table.Column<int>(type: "integer", nullable: true),
                    stun_duration = table.Column<float>(type: "real", nullable: true),
                    level = table.Column<int>(type: "integer", nullable: true),
                    net_worth = table.Column<long>(type: "bigint", nullable: true),
                    hero_damage = table.Column<int>(type: "integer", nullable: true),
                    tower_damage = table.Column<int>(type: "integer", nullable: true),
                    hero_healing = table.Column<int>(type: "integer", nullable: true),
                    gold = table.Column<int>(type: "integer", nullable: true),
                    fight_score = table.Column<float>(type: "real", nullable: true),
                    farm_score = table.Column<float>(type: "real", nullable: true),
                    support_score = table.Column<float>(type: "real", nullable: true),
                    push_score = table.Column<float>(type: "real", nullable: true),
                    hero_xp = table.Column<long>(type: "bigint", nullable: true),
                    rampages = table.Column<long>(type: "bigint", nullable: true),
                    triple_kills = table.Column<long>(type: "bigint", nullable: true),
                    aegis_snatched = table.Column<long>(type: "bigint", nullable: true),
                    rapiers_purchased = table.Column<long>(type: "bigint", nullable: true),
                    couriers_killed = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fantasy_match_player", x => x.id);
                    table.ForeignKey(
                        name: "FK_fantasy_match_player_dota_accounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "nadcl",
                        principalTable: "dota_accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_fantasy_match_player_dota_heroes_HeroId",
                        column: x => x.HeroId,
                        principalSchema: "nadcl",
                        principalTable: "dota_heroes",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_fantasy_match_player_dota_teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "nadcl",
                        principalTable: "dota_teams",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_fantasy_match_player_fantasy_match_match_id",
                        column: x => x.match_id,
                        principalSchema: "nadcl",
                        principalTable: "fantasy_match",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_metadata_LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                column: "LeagueId");

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_detail_player_damage_dealt_MatchPlayerId",
                schema: "nadcl",
                table: "dota_gc_match_detail_player_damage_dealt",
                column: "MatchPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_detail_player_damage_received_MatchPlayerId",
                schema: "nadcl",
                table: "dota_gc_match_detail_player_damage_received",
                column: "MatchPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_detail_players_MatchId",
                schema: "nadcl",
                table: "dota_gc_match_detail_players",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_match_DireTeamId",
                schema: "nadcl",
                table: "fantasy_match",
                column: "DireTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_match_RadiantTeamId",
                schema: "nadcl",
                table: "fantasy_match",
                column: "RadiantTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_match_player_AccountId",
                schema: "nadcl",
                table: "fantasy_match_player",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_match_player_HeroId",
                schema: "nadcl",
                table: "fantasy_match_player",
                column: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_match_player_match_id",
                schema: "nadcl",
                table: "fantasy_match_player",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_match_player_TeamId",
                schema: "nadcl",
                table: "fantasy_match_player",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_dota_leagues_LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                column: "LeagueId",
                principalSchema: "nadcl",
                principalTable: "dota_leagues",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_dota_leagues_LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata");

            migrationBuilder.DropTable(
                name: "discord_users",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_gc_match_detail_player_damage_dealt",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_gc_match_detail_player_damage_received",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "fantasy_match_player",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_gc_match_detail_players",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "fantasy_match",
                schema: "nadcl");

            migrationBuilder.DropIndex(
                name: "IX_dota_gc_match_metadata_LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dota_gc_match_details",
                schema: "nadcl",
                table: "dota_gc_match_details");

            migrationBuilder.DropColumn(
                name: "LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata");

            migrationBuilder.RenameTable(
                name: "dota_gc_match_details",
                schema: "nadcl",
                newName: "gc_dota_matches",
                newSchema: "nadcl");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gc_dota_matches",
                schema: "nadcl",
                table: "gc_dota_matches",
                column: "match_id");

            migrationBuilder.CreateTable(
                name: "balance_ledger",
                schema: "nadcl",
                columns: table => new
                {
                    discord_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tokens = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_balance_ledger", x => x.discord_id);
                });

            migrationBuilder.CreateTable(
                name: "bets_streaks",
                schema: "nadcl",
                columns: table => new
                {
                    discord_name = table.Column<string>(type: "text", nullable: false),
                    streak = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bets_streaks", x => x.discord_name);
                });

            migrationBuilder.CreateTable(
                name: "bromance_last_60",
                schema: "nadcl",
                columns: table => new
                {
                    bro_1_name = table.Column<string>(type: "text", nullable: false),
                    bro_2_name = table.Column<string>(type: "text", nullable: false),
                    total_matches = table.Column<int>(type: "integer", nullable: false),
                    total_wins = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bromance_last_60", x => new { x.bro_1_name, x.bro_2_name });
                });

            migrationBuilder.CreateTable(
                name: "discord_ids",
                schema: "nadcl",
                columns: table => new
                {
                    discord_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    discord_name = table.Column<string>(type: "text", nullable: false),
                    steam_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_discord_ids", x => x.discord_id);
                });

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
                name: "matches_streaks",
                schema: "nadcl",
                columns: table => new
                {
                    discord_name = table.Column<string>(type: "text", nullable: false),
                    streak = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matches_streaks", x => x.discord_name);
                });

            migrationBuilder.CreateTable(
                name: "player_match_details",
                schema: "nadcl",
                columns: table => new
                {
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    player_slot = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    aghanims_scepter = table.Column<int>(type: "integer", nullable: true),
                    aghanims_shard = table.Column<int>(type: "integer", nullable: true),
                    assists = table.Column<int>(type: "integer", nullable: true),
                    backpack_0 = table.Column<int>(type: "integer", nullable: true),
                    backpack_1 = table.Column<int>(type: "integer", nullable: true),
                    backpack_2 = table.Column<int>(type: "integer", nullable: true),
                    deaths = table.Column<int>(type: "integer", nullable: true),
                    denies = table.Column<int>(type: "integer", nullable: true),
                    gold = table.Column<int>(type: "integer", nullable: true),
                    gold_per_min = table.Column<int>(type: "integer", nullable: true),
                    gold_spent = table.Column<int>(type: "integer", nullable: true),
                    hero_damage = table.Column<int>(type: "integer", nullable: true),
                    hero_healing = table.Column<int>(type: "integer", nullable: true),
                    hero_id = table.Column<int>(type: "integer", nullable: false),
                    item_0 = table.Column<int>(type: "integer", nullable: true),
                    item_1 = table.Column<int>(type: "integer", nullable: true),
                    item_2 = table.Column<int>(type: "integer", nullable: true),
                    item_3 = table.Column<int>(type: "integer", nullable: true),
                    item_4 = table.Column<int>(type: "integer", nullable: true),
                    item_5 = table.Column<int>(type: "integer", nullable: true),
                    item_neutral = table.Column<int>(type: "integer", nullable: true),
                    kills = table.Column<int>(type: "integer", nullable: true),
                    last_hits = table.Column<int>(type: "integer", nullable: true),
                    leaver_status = table.Column<int>(type: "integer", nullable: true),
                    level = table.Column<int>(type: "integer", nullable: true),
                    moonshard = table.Column<int>(type: "integer", nullable: true),
                    net_worth = table.Column<long>(type: "bigint", nullable: true),
                    scaled_hero_damage = table.Column<int>(type: "integer", nullable: true),
                    scaled_hero_healing = table.Column<int>(type: "integer", nullable: true),
                    scaled_tower_damage = table.Column<int>(type: "integer", nullable: true),
                    team_number = table.Column<int>(type: "integer", nullable: true),
                    team_slot = table.Column<int>(type: "integer", nullable: true),
                    tower_damage = table.Column<int>(type: "integer", nullable: true),
                    xp_per_min = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_match_details", x => new { x.match_id, x.player_slot });
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_metadata_match_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                column: "match_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_dota_match_details_match_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                column: "match_id",
                principalSchema: "nadcl",
                principalTable: "dota_match_details",
                principalColumn: "match_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
