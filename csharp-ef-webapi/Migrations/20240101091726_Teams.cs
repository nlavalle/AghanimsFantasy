using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class Teams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dota_teams",
                schema: "Kali",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    tag = table.Column<string>(type: "text", nullable: true),
                    abbreviation = table.Column<string>(type: "text", nullable: true),
                    time_created = table.Column<long>(type: "bigint", nullable: false),
                    logo = table.Column<long>(type: "bigint", nullable: false),
                    logo_sponsor = table.Column<long>(type: "bigint", nullable: false),
                    country_code = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    games_played = table.Column<int>(type: "integer", nullable: false),
                    player_0_account_id = table.Column<long>(type: "bigint", nullable: false),
                    player_1_account_id = table.Column<long>(type: "bigint", nullable: false),
                    player_2_account_id = table.Column<long>(type: "bigint", nullable: false),
                    player_3_account_id = table.Column<long>(type: "bigint", nullable: false),
                    player_4_account_id = table.Column<long>(type: "bigint", nullable: false),
                    player_5_account_id = table.Column<long>(type: "bigint", nullable: false),
                    admin_account_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_teams", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dota_teams",
                schema: "Kali");
        }
    }
}
