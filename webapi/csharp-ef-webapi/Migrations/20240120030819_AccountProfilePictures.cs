using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    public partial class AccountProfilePictures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "steam_profile_picture",
                schema: "nadcl",
                table: "dota_accounts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "steam_profile_picture",
                schema: "nadcl",
                table: "dota_accounts");
        }
    }
}
