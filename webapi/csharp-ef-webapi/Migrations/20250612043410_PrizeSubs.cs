using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class PrizeSubs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "nadcl",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "45d3b385-e1d9-4bae-b8d7-a3640034ace7");

            migrationBuilder.DeleteData(
                schema: "nadcl",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b037dd1d-9b5a-4e0a-81a4-b89e0fbb0aae");

            migrationBuilder.AddColumn<bool>(
                name: "is_substitution",
                schema: "nadcl",
                table: "dota_fantasy_players",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "fantasy_prizes",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    prize_type = table.Column<int>(type: "integer", nullable: false),
                    prize_timestamp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fantasy_prizes", x => x.id);
                    table.ForeignKey(
                        name: "FK_fantasy_prizes_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalSchema: "nadcl",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "nadcl",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7ae66c2d-e22f-4bd4-9e67-7af779d28fd6", null, "Admin", null },
                    { "b792d7a7-84b2-4995-a163-497521944993", null, "PrivateFantasyLeagueAdmin", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_prizes_user_id",
                schema: "nadcl",
                table: "fantasy_prizes",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fantasy_prizes",
                schema: "nadcl");

            migrationBuilder.DeleteData(
                schema: "nadcl",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7ae66c2d-e22f-4bd4-9e67-7af779d28fd6");

            migrationBuilder.DeleteData(
                schema: "nadcl",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b792d7a7-84b2-4995-a163-497521944993");

            migrationBuilder.DropColumn(
                name: "is_substitution",
                schema: "nadcl",
                table: "dota_fantasy_players");

            migrationBuilder.InsertData(
                schema: "nadcl",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "45d3b385-e1d9-4bae-b8d7-a3640034ace7", null, "Admin", null },
                    { "b037dd1d-9b5a-4e0a-81a4-b89e0fbb0aae", null, "PrivateFantasyLeagueAdmin", null }
                });
        }
    }
}
