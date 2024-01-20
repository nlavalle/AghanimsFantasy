START TRANSACTION;

ALTER TABLE nadcl.dota_leagues ADD fantasy_draft_locked_date timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN league_id TYPE integer;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240117062055_LeagueDraftLock', '6.0.4');

COMMIT;


