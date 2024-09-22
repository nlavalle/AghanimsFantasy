START TRANSACTION;

ALTER TABLE nadcl.dota_accounts ADD steam_profile_picture text NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240120030819_AccountProfilePictures', '6.0.4');

COMMIT;


