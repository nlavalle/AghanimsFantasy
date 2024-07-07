using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class GcDotaMsg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "gc_dota_matches",
                schema: "nadcl",
                columns: table => new
                {
                    match_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    duration = table.Column<long>(type: "bigint", nullable: false),
                    starttime = table.Column<long>(type: "bigint", nullable: false),
                    cluster = table.Column<long>(type: "bigint", nullable: false),
                    first_blood_time = table.Column<long>(type: "bigint", nullable: false),
                    replay_salt = table.Column<long>(type: "bigint", nullable: false),
                    server_ip = table.Column<long>(type: "bigint", nullable: false),
                    server_port = table.Column<long>(type: "bigint", nullable: false),
                    lobby_type = table.Column<long>(type: "bigint", nullable: false),
                    human_players = table.Column<long>(type: "bigint", nullable: false),
                    average_skill = table.Column<long>(type: "bigint", nullable: false),
                    game_balance = table.Column<float>(type: "real", nullable: false),
                    radiant_team_id = table.Column<long>(type: "bigint", nullable: false),
                    dire_team_id = table.Column<long>(type: "bigint", nullable: false),
                    leagueid = table.Column<long>(type: "bigint", nullable: false),
                    radiant_team_name = table.Column<string>(type: "text", nullable: true),
                    dire_team_name = table.Column<string>(type: "text", nullable: true),
                    radiant_team_logo = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    dire_team_logo = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    radiant_team_logo_url = table.Column<string>(type: "text", nullable: true),
                    dire_team_logo_url = table.Column<string>(type: "text", nullable: true),
                    radiant_team_complete = table.Column<long>(type: "bigint", nullable: false),
                    dire_team_complete = table.Column<long>(type: "bigint", nullable: false),
                    game_mode = table.Column<int>(type: "integer", nullable: false),
                    match_seq_num = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    replay_state = table.Column<int>(type: "integer", nullable: false),
                    radiant_guild_id = table.Column<long>(type: "bigint", nullable: false),
                    dire_guild_id = table.Column<long>(type: "bigint", nullable: false),
                    radiant_team_tag = table.Column<string>(type: "text", nullable: true),
                    dire_team_tag = table.Column<string>(type: "text", nullable: true),
                    series_id = table.Column<long>(type: "bigint", nullable: false),
                    series_type = table.Column<long>(type: "bigint", nullable: false),
                    engine = table.Column<long>(type: "bigint", nullable: false),
                    match_flags = table.Column<long>(type: "bigint", nullable: false),
                    private_metadata_key = table.Column<long>(type: "bigint", nullable: false),
                    radiant_team_score = table.Column<long>(type: "bigint", nullable: false),
                    dire_team_score = table.Column<long>(type: "bigint", nullable: false),
                    match_outcome = table.Column<int>(type: "integer", nullable: false),
                    tournament_id = table.Column<long>(type: "bigint", nullable: false),
                    tournament_round = table.Column<long>(type: "bigint", nullable: false),
                    pre_game_duration = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gc_dota_matches", x => x.match_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "gc_dota_matches",
                schema: "nadcl");
        }
    }
}
