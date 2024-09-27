using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace csharp_ef_webapi.Migrations
{
    /// <inheritdoc />
    public partial class PrivateFantasyLeague : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_players_fantasy_pla~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_players_dota_teams_team_id",
                schema: "nadcl",
                table: "dota_fantasy_players");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_dota_leagues_LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata");

            migrationBuilder.DropForeignKey(
                name: "FK_fantasy_match_player_dota_accounts_AccountId",
                schema: "nadcl",
                table: "fantasy_match_player");

            migrationBuilder.DropForeignKey(
                name: "FK_fantasy_match_player_dota_teams_TeamId",
                schema: "nadcl",
                table: "fantasy_match_player");

            migrationBuilder.DropIndex(
                name: "IX_dota_fantasy_normalized_averages_fantasy_player_id",
                schema: "nadcl",
                table: "dota_fantasy_normalized_averages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dota_fantasy_draft_players",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");

            migrationBuilder.DropIndex(
                name: "IX_dota_fantasy_draft_players_fantasy_draft_id",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                schema: "nadcl",
                table: "fantasy_match_player",
                newName: "team_id");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                schema: "nadcl",
                table: "fantasy_match_player",
                newName: "account_id");

            migrationBuilder.RenameIndex(
                name: "IX_fantasy_match_player_TeamId",
                schema: "nadcl",
                table: "fantasy_match_player",
                newName: "IX_fantasy_match_player_team_id");

            migrationBuilder.RenameIndex(
                name: "IX_fantasy_match_player_AccountId",
                schema: "nadcl",
                table: "fantasy_match_player",
                newName: "IX_fantasy_match_player_account_id");

            migrationBuilder.RenameColumn(
                name: "LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                newName: "league_id");

            migrationBuilder.RenameIndex(
                name: "IX_dota_gc_match_metadata_LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                newName: "IX_dota_gc_match_metadata_league_id");

            migrationBuilder.AlterColumn<long>(
                name: "team_id",
                schema: "nadcl",
                table: "fantasy_match_player",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "league_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_private",
                schema: "nadcl",
                table: "dota_fantasy_leagues",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_dota_fantasy_draft_players",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                columns: new[] { "fantasy_draft_id", "draft_order" });

            migrationBuilder.CreateTable(
                name: "dota_fantasy_private_league_players",
                schema: "nadcl",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fantasy_league_id = table.Column<int>(type: "integer", nullable: false),
                    discord_user_id = table.Column<long>(type: "bigint", nullable: false),
                    fantasy_league_join_date = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dota_fantasy_private_league_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_dota_fantasy_private_league_players_discord_users_discord_u~",
                        column: x => x.discord_user_id,
                        principalSchema: "nadcl",
                        principalTable: "discord_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dota_fantasy_private_league_players_dota_fantasy_leagues_fa~",
                        column: x => x.fantasy_league_id,
                        principalSchema: "nadcl",
                        principalTable: "dota_fantasy_leagues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_match_league_id",
                schema: "nadcl",
                table: "fantasy_match",
                column: "league_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_normalized_averages_fantasy_player_id",
                schema: "nadcl",
                table: "dota_fantasy_normalized_averages",
                column: "fantasy_player_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_draft_players_fantasy_player_id",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                column: "fantasy_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_private_league_players_discord_user_id",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players",
                column: "discord_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_private_league_players_fantasy_league_id",
                schema: "nadcl",
                table: "dota_fantasy_private_league_players",
                column: "fantasy_league_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_players_fantasy_pla~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                column: "fantasy_player_id",
                principalSchema: "nadcl",
                principalTable: "dota_fantasy_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_players_dota_teams_team_id",
                schema: "nadcl",
                table: "dota_fantasy_players",
                column: "team_id",
                principalSchema: "nadcl",
                principalTable: "dota_teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_dota_leagues_league_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                column: "league_id",
                principalSchema: "nadcl",
                principalTable: "dota_leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_fantasy_match_dota_leagues_league_id",
                schema: "nadcl",
                table: "fantasy_match",
                column: "league_id",
                principalSchema: "nadcl",
                principalTable: "dota_leagues",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_fantasy_match_player_dota_accounts_account_id",
                schema: "nadcl",
                table: "fantasy_match_player",
                column: "account_id",
                principalSchema: "nadcl",
                principalTable: "dota_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_fantasy_match_player_dota_teams_team_id",
                schema: "nadcl",
                table: "fantasy_match_player",
                column: "team_id",
                principalSchema: "nadcl",
                principalTable: "dota_teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_players_fantasy_pla~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_fantasy_players_dota_teams_team_id",
                schema: "nadcl",
                table: "dota_fantasy_players");

            migrationBuilder.DropForeignKey(
                name: "FK_dota_gc_match_metadata_dota_leagues_league_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata");

            migrationBuilder.DropForeignKey(
                name: "FK_fantasy_match_dota_leagues_league_id",
                schema: "nadcl",
                table: "fantasy_match");

            migrationBuilder.DropForeignKey(
                name: "FK_fantasy_match_player_dota_accounts_account_id",
                schema: "nadcl",
                table: "fantasy_match_player");

            migrationBuilder.DropForeignKey(
                name: "FK_fantasy_match_player_dota_teams_team_id",
                schema: "nadcl",
                table: "fantasy_match_player");

            migrationBuilder.DropTable(
                name: "dota_fantasy_private_league_players",
                schema: "nadcl");

            migrationBuilder.DropIndex(
                name: "IX_fantasy_match_league_id",
                schema: "nadcl",
                table: "fantasy_match");

            migrationBuilder.DropIndex(
                name: "IX_dota_fantasy_normalized_averages_fantasy_player_id",
                schema: "nadcl",
                table: "dota_fantasy_normalized_averages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dota_fantasy_draft_players",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");

            migrationBuilder.DropIndex(
                name: "IX_dota_fantasy_draft_players_fantasy_player_id",
                schema: "nadcl",
                table: "dota_fantasy_draft_players");

            migrationBuilder.DropColumn(
                name: "is_private",
                schema: "nadcl",
                table: "dota_fantasy_leagues");

            migrationBuilder.RenameColumn(
                name: "team_id",
                schema: "nadcl",
                table: "fantasy_match_player",
                newName: "TeamId");

            migrationBuilder.RenameColumn(
                name: "account_id",
                schema: "nadcl",
                table: "fantasy_match_player",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_fantasy_match_player_team_id",
                schema: "nadcl",
                table: "fantasy_match_player",
                newName: "IX_fantasy_match_player_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_fantasy_match_player_account_id",
                schema: "nadcl",
                table: "fantasy_match_player",
                newName: "IX_fantasy_match_player_AccountId");

            migrationBuilder.RenameColumn(
                name: "league_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                newName: "LeagueId");

            migrationBuilder.RenameIndex(
                name: "IX_dota_gc_match_metadata_league_id",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                newName: "IX_dota_gc_match_metadata_LeagueId");

            migrationBuilder.AlterColumn<long>(
                name: "TeamId",
                schema: "nadcl",
                table: "fantasy_match_player",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddPrimaryKey(
                name: "PK_dota_fantasy_draft_players",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                columns: new[] { "fantasy_player_id", "fantasy_draft_id" });

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_normalized_averages_fantasy_player_id",
                schema: "nadcl",
                table: "dota_fantasy_normalized_averages",
                column: "fantasy_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_dota_fantasy_draft_players_fantasy_draft_id",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                column: "fantasy_draft_id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_draft_players_dota_fantasy_players_fantasy_pla~",
                schema: "nadcl",
                table: "dota_fantasy_draft_players",
                column: "fantasy_player_id",
                principalSchema: "nadcl",
                principalTable: "dota_fantasy_players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_fantasy_players_dota_teams_team_id",
                schema: "nadcl",
                table: "dota_fantasy_players",
                column: "team_id",
                principalSchema: "nadcl",
                principalTable: "dota_teams",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_dota_gc_match_metadata_dota_leagues_LeagueId",
                schema: "nadcl",
                table: "dota_gc_match_metadata",
                column: "LeagueId",
                principalSchema: "nadcl",
                principalTable: "dota_leagues",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_fantasy_match_player_dota_accounts_AccountId",
                schema: "nadcl",
                table: "fantasy_match_player",
                column: "AccountId",
                principalSchema: "nadcl",
                principalTable: "dota_accounts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_fantasy_match_player_dota_teams_TeamId",
                schema: "nadcl",
                table: "fantasy_match_player",
                column: "TeamId",
                principalSchema: "nadcl",
                principalTable: "dota_teams",
                principalColumn: "id");
        }
    }
}
