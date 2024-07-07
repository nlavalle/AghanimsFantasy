START TRANSACTION;

DROP INDEX nadcl."IX_dota_fantasy_players_dota_account_id";

CREATE UNIQUE INDEX "IX_dota_fantasy_players_dota_account_id" ON nadcl.dota_fantasy_players (dota_account_id);

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240120040555_FantasyPlayerFkFix';

COMMIT;


