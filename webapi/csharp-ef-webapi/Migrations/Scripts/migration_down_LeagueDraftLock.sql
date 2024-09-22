START TRANSACTION;

ALTER TABLE nadcl.dota_leagues DROP COLUMN fantasy_draft_locked_date;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN league_id TYPE bigint;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240117062055_LeagueDraftLock';

COMMIT;


