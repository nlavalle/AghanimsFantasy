START TRANSACTION;

DROP TABLE "Kali".dota_match_details_picks_bans;

DROP TABLE "Kali".dota_match_details_players_ability_upgrades;

DROP TABLE "Kali".dota_match_history_players;

DROP TABLE "Kali".dota_match_details_players;

DROP TABLE "Kali".dota_match_details;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20231231054941_MatchDetails';

COMMIT;


