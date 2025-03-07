START TRANSACTION;

ALTER TABLE nadcl.dota_leagues ADD end_timestamp bigint NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD is_scheduled boolean NOT NULL DEFAULT FALSE;

ALTER TABLE nadcl.dota_leagues ADD most_recent_activity bigint NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD region integer NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD start_timestamp bigint NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD status integer NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD tier integer NOT NULL DEFAULT 0;

ALTER TABLE nadcl.dota_leagues ADD total_prize_pool bigint NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250204062622_LeagueSchedule', '8.0.1');

COMMIT;


