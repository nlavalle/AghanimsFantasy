using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class FantasyCost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dota_fantasy_budget_probability",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fantasy_league_id = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    quintile = table.Column<int>(type: "integer", nullable: false),
                    probability = table.Column<decimal>(type: "numeric", nullable: false),
                    cumulative_probability = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_fantasy_budget_probability", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_fantasy_budget_probability_dota_accounts_account_id",
                        column: x => x.account_id,
                        principalSchema: "nadcl",
                        principalTable: "dota_accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dota_fantasy_budget_probability_dota_fantasy_leagues_fantas~",
                        column: x => x.fantasy_league_id,
                        principalSchema: "nadcl",
                        principalTable: "dota_fantasy_leagues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_budget_probability_account_id",
                schema: "nadcl",
                table: "dota_fantasy_budget_probability",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_budget_probability_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_budget_probability",
                column: "fantasy_league_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dota_fantasy_budget_probability",
                schema: "nadcl");
        }
    }
}
