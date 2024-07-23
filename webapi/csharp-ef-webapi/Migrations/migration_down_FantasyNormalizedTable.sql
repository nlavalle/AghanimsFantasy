START TRANSACTION;

DROP TABLE nadcl.dota_fantasy_normalized_averages;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240523061620_FantasyNormalizedTable';

COMMIT;


