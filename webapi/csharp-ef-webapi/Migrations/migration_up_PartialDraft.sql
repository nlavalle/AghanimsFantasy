START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_two DROP NOT NULL;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_three DROP NOT NULL;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_one DROP NOT NULL;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_four DROP NOT NULL;

ALTER TABLE nadcl.dota_fantasy_drafts ALTER COLUMN draft_pick_five DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240121045803_PartialDraft', '6.0.4');

COMMIT;


