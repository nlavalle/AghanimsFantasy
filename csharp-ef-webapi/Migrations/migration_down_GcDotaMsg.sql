START TRANSACTION;

DROP TABLE nadcl.gc_dota_matches;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240210091155_GcDotaMsg';

COMMIT;


