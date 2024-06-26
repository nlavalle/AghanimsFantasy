START TRANSACTION;

ALTER TABLE nadcl.dota_leagues DROP CONSTRAINT "PK_dota_leagues";

ALTER TABLE nadcl.dota_leagues ADD fantasy_draft_locked_date_new bigint NOT NULL DEFAULT 0;
UPDATE nadcl.dota_leagues
SET fantasy_draft_locked_date_new = extract(epoch from fantasy_draft_locked_date)::bigint;
ALTER TABLE nadcl.dota_leagues RENAME COLUMN fantasy_draft_locked_date to fantasy_draft_locked_date_old;
ALTER TABLE nadcl.dota_leagues RENAME COLUMN fantasy_draft_locked_date_new to fantasy_draft_locked_date;
ALTER TABLE nadcl.dota_leagues DROP COLUMN fantasy_draft_locked_date_old;

ALTER TABLE nadcl.dota_leagues ALTER COLUMN league_id 
DROP IDENTITY;

ALTER TABLE nadcl.dota_leagues ADD id integer GENERATED BY DEFAULT AS IDENTITY;

ALTER TABLE nadcl.dota_leagues ADD league_end_time bigint NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD league_start_time bigint NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD CONSTRAINT "PK_dota_leagues" PRIMARY KEY (id);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240124045536_LeagueStartEndTime', '6.0.4');

COMMIT;


