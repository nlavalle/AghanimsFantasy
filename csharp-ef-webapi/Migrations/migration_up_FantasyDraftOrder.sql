START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_draft_players ADD "DraftOrder" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240109085122_FantasyDraftOrder', '6.0.4');

COMMIT;


