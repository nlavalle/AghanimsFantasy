START TRANSACTION;

DROP TABLE nadcl.dota_fantasy_draft_players;

DROP TABLE nadcl.dota_fantasy_drafts;

DROP TABLE nadcl.dota_fantasy_players;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240107094025_FantasyDraft';

COMMIT;


