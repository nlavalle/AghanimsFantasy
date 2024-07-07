using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class Streaks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    total_wins = table.Column<int>(type: "integer", nullable: false),
                    total_matches = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bromance_last_60", x => new { x.bro_1_name, x.bro_2_name });
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bets_streaks",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "bromance_last_60",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "matches_streaks",
                schema: "nadcl");
        }
    }
}
