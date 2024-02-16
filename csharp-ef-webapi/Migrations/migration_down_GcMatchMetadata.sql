START TRANSACTION;

DROP TABLE nadcl.dota_gc_match_metadata_itempurchase;

DROP TABLE nadcl.dota_gc_match_metadata_playerkill;

DROP TABLE nadcl.dota_gc_match_metadata_tip;

DROP TABLE nadcl.dota_gc_match_metadata_player;

DROP TABLE nadcl.dota_gc_match_metadata_team;

DROP TABLE nadcl.dota_gc_match_metadata;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240211111101_GcMatchMetadata';

COMMIT;


