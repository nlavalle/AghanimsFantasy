using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class Accounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_match_history_dota_leagues_league_id",
                schema: "nadcl",
                table: "dota_match_history");

            migrationBuilder.DropIndex(
                name: "IX_dota_match_history_league_id",
                schema: "nadcl",
                table: "dota_match_history");

            migrationBuilder.CreateTable(
                name: "dota_accounts",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_accounts", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dota_accounts",
                schema: "nadcl");

            migrationBuilder.CreateIndex(
                name: "IX_dota_match_history_league_id",
                schema: "nadcl",
                table: "dota_match_history",
                column: "league_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_match_history_dota_leagues_league_id",
                schema: "nadcl",
                table: "dota_match_history",
                column: "league_id",
                principalSchema: "nadcl",
                principalTable: "dota_leagues",
                principalColumn: "league_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
