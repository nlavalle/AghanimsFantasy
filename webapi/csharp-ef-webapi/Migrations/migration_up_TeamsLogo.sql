START TRANSACTION;

ALTER TABLE nadcl.dota_teams ALTER COLUMN logo_sponsor TYPE numeric USING coalesce(nullif(logo_sponsor,''),'0')::numeric;

ALTER TABLE nadcl.dota_teams ALTER COLUMN logo TYPE numeric USING coalesce(nullif(logo,''),'0')::numeric;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250126210142_TeamsLogo', '8.0.1');

COMMIT;
