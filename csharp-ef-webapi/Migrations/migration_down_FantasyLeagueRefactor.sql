START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_drafts DROP CONSTRAINT "FK_dota_fantasy_drafts_dota_fantasy_leagues_fantasy_league_id";

ALTER TABLE nadcl.dota_fantasy_players DROP CONSTRAINT "FK_dota_fantasy_players_dota_fantasy_leagues_fantasy_league_id";

ALTER TABLE nadcl.dota_gc_match_metadata DROP CONSTRAINT "FK_dota_gc_match_metadata_dota_match_details_match_id";

ALTER TABLE nadcl.dota_match_details DROP CONSTRAINT "FK_dota_match_details_dota_leagues_league_id";

ALTER TABLE nadcl.dota_match_history DROP CONSTRAINT "FK_dota_match_history_dota_leagues_league_id";

DROP TABLE nadcl.dota_fantasy_leagues;

DROP INDEX nadcl."IX_dota_match_history_league_id";

DROP INDEX nadcl."IX_dota_match_details_league_id";

DROP INDEX nadcl."IX_dota_gc_match_metadata_match_id";

DROP INDEX nadcl."IX_dota_fantasy_players_fantasy_league_id";

DROP INDEX nadcl."IX_dota_fantasy_drafts_fantasy_league_id";

ALTER TABLE nadcl.dota_fantasy_players DROP COLUMN fantasy_league_id;

ALTER TABLE nadcl.dota_fantasy_drafts RENAME COLUMN fantasy_league_id TO league_id;

ALTER TABLE nadcl.dota_leagues ADD fantasy_draft_locked_date bigint NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD league_end_time bigint NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD league_id integer NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD league_start_time bigint NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_fantasy_players ADD league_id bigint NOT NULL DEFAULT 0;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240216062618_FantasyLeagueRefactor';

COMMIT;


