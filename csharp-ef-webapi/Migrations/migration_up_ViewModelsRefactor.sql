START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_drafts DROP COLUMN draft_pick_five;

ALTER TABLE nadcl.dota_fantasy_drafts DROP COLUMN draft_pick_four;

ALTER TABLE nadcl.dota_fantasy_drafts DROP COLUMN draft_pick_one;

ALTER TABLE nadcl.dota_fantasy_drafts DROP COLUMN draft_pick_three;

ALTER TABLE nadcl.dota_fantasy_drafts DROP COLUMN draft_pick_two;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240121080307_ViewModelsRefactor', '6.0.4');

COMMIT;


