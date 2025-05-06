using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class Identity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_private_league_players_discord_users_discord_u~",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players");

            migrationBuilder.DropForeignKey(
                name: "FK_fantasy_ledger_discord_users_discord_id",
                schema: "nadcl",
                table: "fantasy_ledger");

            migrationBuilder.DropIndex(
                name: "IX_fantasy_ledger_discord_id",
                schema: "nadcl",
                table: "fantasy_ledger");

            migrationBuilder.DropIndex(
                name: "IX_dota_fantasy_private_league_players_discord_user_id",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players");

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                schema: "nadcl",
                table: "fantasy_ledger",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                schema: "nadcl",
                table: "dota_fantasy_drafts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "nadcl",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "nadcl",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    DiscordId = table.Column<long>(type: "bigint", nullable: false),
                    DiscordHandle = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "nadcl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "nadcl",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "nadcl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "nadcl",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "nadcl",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "nadcl",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "nadcl",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "nadcl",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "nadcl",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "nadcl",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
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
                    { "07aea6b5-5780-4779-875e-e7d457e90030", null, "Admin", null },
                    { "d113700f-8542-43f1-90ec-b9dc05239b7e", null, "PrivateFantasyLeagueAdmin", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_ledger_user_id",
                schema: "nadcl",
                table: "fantasy_ledger",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_private_league_players_user_id",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "nadcl",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "nadcl",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "nadcl",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "nadcl",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "nadcl",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "nadcl",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "nadcl",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_private_league_players_AspNetUsers_user_id",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players",
                column: "user_id",
                principalSchema: "nadcl",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_fantasy_ledger_AspNetUsers_user_id",
                schema: "nadcl",
                table: "fantasy_ledger",
                column: "user_id",
                principalSchema: "nadcl",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_private_league_players_AspNetUsers_user_id",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players");

            migrationBuilder.DropForeignKey(
                name: "FK_fantasy_ledger_AspNetUsers_user_id",
                schema: "nadcl",
                table: "fantasy_ledger");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "nadcl");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "nadcl");

            migrationBuilder.DropIndex(
                name: "IX_fantasy_ledger_user_id",
                schema: "nadcl",
                table: "fantasy_ledger");

            migrationBuilder.DropIndex(
                name: "IX_dota_fantasy_private_league_players_user_id",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "nadcl",
                table: "fantasy_ledger");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "nadcl",
                table: "dota_fantasy_drafts");

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_ledger_discord_id",
                schema: "nadcl",
                table: "fantasy_ledger",
                column: "discord_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_private_league_players_discord_user_id",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players",
                column: "discord_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_private_league_players_discord_users_discord_u~",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players",
                column: "discord_user_id",
                principalSchema: "nadcl",
                principalTable: "discord_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_fantasy_ledger_discord_users_discord_id",
                schema: "nadcl",
                table: "fantasy_ledger",
                column: "discord_id",
                principalSchema: "nadcl",
                principalTable: "discord_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
