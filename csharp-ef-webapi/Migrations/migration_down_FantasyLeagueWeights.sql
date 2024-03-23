Build started...
Build succeeded.
START TRANSACTION;

DROP TABLE nadcl.dota_fantasy_league_weights;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240225050151_FantasyLeagueWeights';

COMMIT;


