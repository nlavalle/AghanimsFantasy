START TRANSACTION;

ALTER TABLE nadcl.dota_gc_match_detail_players ALTER COLUMN hero_id TYPE integer;

ALTER TABLE nadcl.dota_gc_match_detail_players ADD selected_facet bigint NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240925031912_Facets', '8.0.1');

COMMIT;


