START TRANSACTION;

ALTER TABLE nadcl.dota_accounts DROP COLUMN steam_profile_picture;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240120030819_AccountProfilePictures';

COMMIT;


