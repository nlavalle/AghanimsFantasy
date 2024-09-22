START TRANSACTION;

ALTER TABLE nadcl.discord_users ADD is_admin boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240616225253_AdminRole', '8.0.1');

COMMIT;


