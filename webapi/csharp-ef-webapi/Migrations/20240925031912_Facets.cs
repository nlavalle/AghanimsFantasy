using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class Facets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "hero_id",
                schema: "nadcl",
                table: "dota_gc_match_detail_players",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "selected_facet",
                schema: "nadcl",
                table: "dota_gc_match_detail_players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "selected_facet",
                schema: "nadcl",
                table: "dota_gc_match_detail_players");

            migrationBuilder.AlterColumn<long>(
                name: "hero_id",
                schema: "nadcl",
                table: "dota_gc_match_detail_players",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
