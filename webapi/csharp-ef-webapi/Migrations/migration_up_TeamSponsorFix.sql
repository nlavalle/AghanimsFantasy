START TRANSACTION;

ALTER TABLE nadcl.dota_teams ALTER COLUMN logo_sponsor TYPE text;
ALTER TABLE nadcl.dota_teams ALTER COLUMN logo_sponsor DROP NOT NULL;

ALTER TABLE nadcl.dota_teams ALTER COLUMN logo TYPE text;
ALTER TABLE nadcl.dota_teams ALTER COLUMN logo DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240120052456_TeamSponsorFix', '6.0.4');

COMMIT;


