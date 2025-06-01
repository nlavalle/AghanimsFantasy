using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class GcDotaPicksBans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "nadcl",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "07aea6b5-5780-4779-875e-e7d457e90030");

            migrationBuilder.DeleteData(
                schema: "nadcl",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d113700f-8542-43f1-90ec-b9dc05239b7e");

            migrationBuilder.CreateTable(
                name: "dota_gc_match_detail_picks_bans",
                schema: "nadcl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    is_pick = table.Column<bool>(type: "boolean", nullable: false),
                    team = table.Column<long>(type: "bigint", nullable: false),
                    hero_id = table.Column<int>(type: "integer", nullable: false),
                    MatchId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_gc_match_detail_picks_bans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_dota_gc_match_detail_picks_bans_dota_gc_match_details_Match~",
                        column: x => x.MatchId,
                        principalSchema: "nadcl",
                        principalTable: "dota_gc_match_details",
                        principalColumn: "match_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "nadcl",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "700e0119-2040-46f5-8700-e54dce42d3c9", null, "Admin", null },
                    { "d34d6f89-8a8b-434b-8e7a-1e0e3d685ad8", null, "PrivateFantasyLeagueAdmin", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_dota_gc_match_detail_picks_bans_MatchId",
                schema: "nadcl",
                table: "dota_gc_match_detail_picks_bans",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dota_gc_match_detail_picks_bans",
                schema: "nadcl");

            migrationBuilder.DeleteData(
                schema: "nadcl",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "700e0119-2040-46f5-8700-e54dce42d3c9");

            migrationBuilder.DeleteData(
                schema: "nadcl",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d34d6f89-8a8b-434b-8e7a-1e0e3d685ad8");

            migrationBuilder.InsertData(
                schema: "nadcl",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "07aea6b5-5780-4779-875e-e7d457e90030", null, "Admin", null },
                    { "d113700f-8542-43f1-90ec-b9dc05239b7e", null, "PrivateFantasyLeagueAdmin", null }
                });
        }
    }
}
