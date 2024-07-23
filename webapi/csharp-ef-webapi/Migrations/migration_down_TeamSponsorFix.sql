START TRANSACTION;

ALTER TABLE nadcl.dota_teams ALTER COLUMN logo_sponsor TYPE bigint;
ALTER TABLE nadcl.dota_teams ALTER COLUMN logo_sponsor SET NOT NULL;
ALTER TABLE nadcl.dota_teams ALTER COLUMN logo_sponsor SET DEFAULT 0;

ALTER TABLE nadcl.dota_teams ALTER COLUMN logo TYPE bigint;
ALTER TABLE nadcl.dota_teams ALTER COLUMN logo SET NOT NULL;
ALTER TABLE nadcl.dota_teams ALTER COLUMN logo SET DEFAULT 0;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240120052456_TeamSponsorFix';

COMMIT;


