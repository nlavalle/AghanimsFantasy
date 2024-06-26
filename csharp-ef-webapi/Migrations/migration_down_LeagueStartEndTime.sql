START TRANSACTION;

ALTER TABLE nadcl.dota_leagues DROP CONSTRAINT "PK_dota_leagues";

ALTER TABLE nadcl.dota_leagues DROP COLUMN id;

ALTER TABLE nadcl.dota_leagues DROP COLUMN league_end_time;

ALTER TABLE nadcl.dota_leagues DROP COLUMN league_start_time;

ALTER TABLE nadcl.dota_leagues ALTER COLUMN league_id DROP DEFAULT;
ALTER TABLE nadcl.dota_leagues ALTER COLUMN league_id ADD GENERATED BY DEFAULT AS IDENTITY;

ALTER TABLE nadcl.dota_leagues ADD fantasy_draft_locked_date_new timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';
UPDATE nadcl.dota_leagues
SET fantasy_draft_locked_date_new = to_timestamp(fantasy_draft_locked_date)::date;
ALTER TABLE nadcl.dota_leagues RENAME COLUMN fantasy_draft_locked_date to fantasy_draft_locked_date_old;
ALTER TABLE nadcl.dota_leagues RENAME COLUMN fantasy_draft_locked_date_new to fantasy_draft_locked_date;
ALTER TABLE nadcl.dota_leagues DROP COLUMN fantasy_draft_locked_date_old;

ALTER TABLE nadcl.dota_leagues ADD CONSTRAINT "PK_dota_leagues" PRIMARY KEY (league_id);

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240124045536_LeagueStartEndTime';

COMMIT;


