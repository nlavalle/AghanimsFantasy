START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_draft_players DROP COLUMN "DraftOrder";

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240109085122_FantasyDraftOrder';

COMMIT;


