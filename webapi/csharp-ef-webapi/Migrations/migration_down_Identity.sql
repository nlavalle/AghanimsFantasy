START TRANSACTION;

-- ALTER TABLE nadcl.dota_fantasy_private_league_players DROP CONSTRAINT "FK_dota_fantasy_private_league_players_AspNetUsers_user_id";

-- ALTER TABLE nadcl.fantasy_ledger DROP CONSTRAINT "FK_fantasy_ledger_AspNetUsers_user_id";

DROP TABLE nadcl."AspNetRoleClaims";

DROP TABLE nadcl."AspNetUserClaims";

DROP TABLE nadcl."AspNetUserLogins";

DROP TABLE nadcl."AspNetUserRoles";

DROP TABLE nadcl."AspNetUserTokens";

DROP TABLE nadcl."AspNetRoles";

DROP TABLE nadcl."AspNetUsers";

DROP INDEX nadcl."IX_fantasy_ledger_user_id";

DROP INDEX nadcl."IX_dota_fantasy_private_league_players_user_id";

ALTER TABLE nadcl.fantasy_ledger DROP COLUMN user_id;

ALTER TABLE nadcl.dota_fantasy_private_league_players DROP COLUMN user_id;

ALTER TABLE nadcl.dota_fantasy_drafts DROP COLUMN user_id;

CREATE INDEX "IX_fantasy_ledger_discord_id" ON nadcl.fantasy_ledger (discord_id);

CREATE INDEX "IX_dota_fantasy_private_league_players_discord_user_id" ON nadcl.dota_fantasy_private_league_players (discord_user_id);

ALTER TABLE nadcl.dota_fantasy_private_league_players ADD CONSTRAINT "FK_dota_fantasy_private_league_players_discord_users_discord_u~" FOREIGN KEY (discord_user_id) REFERENCES nadcl.discord_users (id) ON DELETE CASCADE;

ALTER TABLE nadcl.fantasy_ledger ADD CONSTRAINT "FK_fantasy_ledger_discord_users_discord_id" FOREIGN KEY (discord_id) REFERENCES nadcl.discord_users (id) ON DELETE CASCADE;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20250501011503_Identity';

COMMIT;


