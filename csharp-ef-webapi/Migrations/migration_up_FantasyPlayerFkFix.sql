START TRANSACTION;

DROP INDEX nadcl."IX_dota_fantasy_players_dota_account_id";

CREATE INDEX "IX_dota_fantasy_players_dota_account_id" ON nadcl.dota_fantasy_players (dota_account_id);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240120040555_FantasyPlayerFkFix', '6.0.4');

COMMIT;
