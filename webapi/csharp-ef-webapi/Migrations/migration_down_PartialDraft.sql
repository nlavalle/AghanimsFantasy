START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_two SET NOT NULL;
ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_two SET DEFAULT 0;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_three SET NOT NULL;
ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_three SET DEFAULT 0;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_one SET NOT NULL;
ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_one SET DEFAULT 0;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_four SET NOT NULL;
ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_four SET DEFAULT 0;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_five SET NOT NULL;
ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_five SET DEFAULT 0;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240121045803_PartialDraft';

COMMIT;


