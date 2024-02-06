START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_draft_players DROP CONSTRAINT "FK_dota_fantasy_draft_players_dota_fantasy_drafts_fantasy_draf~";

ALTER TABLE nadcl.dota_fantasy_draft_players DROP CONSTRAINT "FK_dota_fantasy_draft_players_dota_fantasy_players_fantasy_pla~";

ALTER TABLE nadcl.dota_fantasy_draft_players RENAME COLUMN draft_order TO "DraftOrder";

ALTER TABLE nadcl.dota_fantasy_draft_players RENAME COLUMN fantasy_draft_id TO "FantasyDraftId";

ALTER TABLE nadcl.dota_fantasy_draft_players RENAME COLUMN fantasy_player_id TO "FantasyPlayerId";

ALTER INDEX nadcl."IX_dota_fantasy_draft_players_fantasy_draft_id" RENAME TO "IX_dota_fantasy_draft_players_FantasyDraftId";

ALTER TABLE nadcl.dota_fantasy_draft_players ADD CONSTRAINT "FK_dota_fantasy_draft_players_dota_fantasy_drafts_FantasyDraft~" FOREIGN KEY ("FantasyDraftId") REFERENCES nadcl.dota_fantasy_drafts (id) ON DELETE CASCADE;

ALTER TABLE nadcl.dota_fantasy_draft_players ADD CONSTRAINT "FK_dota_fantasy_draft_players_dota_fantasy_players_FantasyPlay~" FOREIGN KEY ("FantasyPlayerId") REFERENCES nadcl.dota_fantasy_players (id);

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240120201558_FantasyDraftPlayerRename';

COMMIT;


