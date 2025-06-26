using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class Cost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DropColumn(
                name: "cumulative_probability",
                schema: "nadcl",
                table: "dota_fantasy_budget_probability");

            migrationBuilder.DropColumn(
                name: "quintile",
                schema: "nadcl",
                table: "dota_fantasy_budget_probability");

            migrationBuilder.RenameColumn(
                name: "probability",
                schema: "nadcl",
                table: "dota_fantasy_budget_probability",
                newName: "Cost");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "Cost",
                schema: "nadcl",
                table: "dota_fantasy_budget_probability",
                newName: "probability");

            migrationBuilder.AddColumn<decimal>(
                name: "cumulative_probability",
                schema: "nadcl",
                table: "dota_fantasy_budget_probability",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "quintile",
                schema: "nadcl",
                table: "dota_fantasy_budget_probability",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                schema: "nadcl",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "700e0119-2040-46f5-8700-e54dce42d3c9", null, "Admin", null },
                    { "d34d6f89-8a8b-434b-8e7a-1e0e3d685ad8", null, "PrivateFantasyLeagueAdmin", null }
                });
        }
    }
}
