START TRANSACTION;

DROP TABLE "Kali".dota_teams;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240101091726_Teams';

COMMIT;


