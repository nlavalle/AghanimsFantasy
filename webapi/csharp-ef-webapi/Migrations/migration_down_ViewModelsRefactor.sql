START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_drafts ADD draft_pick_five bigint NULL;

ALTER TABLE nadcl.dota_fantasy_drafts ADD draft_pick_four bigint NULL;

ALTER TABLE nadcl.dota_fantasy_drafts ADD draft_pick_one bigint NULL;

ALTER TABLE nadcl.dota_fantasy_drafts ADD draft_pick_three bigint NULL;

ALTER TABLE nadcl.dota_fantasy_drafts ADD draft_pick_two bigint NULL;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240121080307_ViewModelsRefactor';

COMMIT;


