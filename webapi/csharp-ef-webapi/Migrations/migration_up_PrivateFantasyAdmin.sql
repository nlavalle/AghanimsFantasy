START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_private_league_players ADD is_admin boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250120061417_PrivateFantasyAdmin', '8.0.1');

COMMIT;


