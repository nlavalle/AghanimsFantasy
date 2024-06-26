START TRANSACTION;

DROP TABLE nadcl.dota_accounts;

CREATE INDEX "IX_dota_match_history_league_id" ON nadcl.dota_match_history (league_id);

ALTER TABLE nadcl.dota_match_history ADD CONSTRAINT "FK_dota_match_history_dota_leagues_league_id" FOREIGN KEY (league_id) REFERENCES nadcl.dota_leagues (league_id) ON DELETE CASCADE;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240105033835_Accounts';

COMMIT;


