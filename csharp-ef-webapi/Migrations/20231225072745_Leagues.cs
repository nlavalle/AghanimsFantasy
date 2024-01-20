using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class Leagues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dota_leagues",
                schema: "nadcl",
                columns: table => new
                {
                    league_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    league_name = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_leagues", x => x.league_id);
                });

            migrationBuilder.CreateTable(
                name: "dota_match_history",
                schema: "nadcl",
                columns: table => new
                {
                    match_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    series_id = table.Column<int>(type: "integer", nullable: false),
                    league_id = table.Column<int>(type: "integer", nullable: false),
                    series_type = table.Column<int>(type: "integer", nullable: false),
                    match_seq_num = table.Column<long>(type: "bigint", nullable: false),
                    start_time = table.Column<long>(type: "bigint", nullable: false),
                    lobby_type = table.Column<int>(type: "integer", nullable: false),
                    radiant_team_id = table.Column<long>(type: "bigint", nullable: false),
                    dire_team_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_match_history", x => x.match_id);
                    table.ForeignKey(
                        name: "FK_dota_match_history_dota_leagues_league_id",
                        column: x => x.league_id,
                        principalSchema: "nadcl",
                        principalTable: "dota_leagues",
                        principalColumn: "league_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_match_history_league_id",
                schema: "nadcl",
                table: "dota_match_history",
                column: "league_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dota_match_history",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "dota_leagues",
                schema: "nadcl");
        }
    }
}
