START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_draft_players DROP CONSTRAINT "FK_dota_fantasy_draft_players_dota_fantasy_drafts_FantasyDraft~";

ALTER TABLE nadcl.dota_fantasy_draft_players DROP CONSTRAINT "FK_dota_fantasy_draft_players_dota_fantasy_players_FantasyPlay~";

ALTER TABLE nadcl.dota_fantasy_draft_players RENAME COLUMN "DraftOrder" TO draft_order;

ALTER TABLE nadcl.dota_fantasy_draft_players RENAME COLUMN "FantasyDraftId" TO fantasy_draft_id;

ALTER TABLE nadcl.dota_fantasy_draft_players RENAME COLUMN "FantasyPlayerId" TO fantasy_player_id;

ALTER INDEX nadcl."IX_dota_fantasy_draft_players_FantasyDraftId" RENAME TO "IX_dota_fantasy_draft_players_fantasy_draft_id";

ALTER TABLE nadcl.dota_fantasy_draft_players ADD CONSTRAINT "FK_dota_fantasy_draft_players_dota_fantasy_drafts_fantasy_draf~" FOREIGN KEY (fantasy_draft_id) REFERENCES nadcl.dota_fantasy_drafts (id) ON DELETE CASCADE;

ALTER TABLE nadcl.dota_fantasy_draft_players ADD CONSTRAINT "FK_dota_fantasy_draft_players_dota_fantasy_players_fantasy_pla~" FOREIGN KEY (fantasy_player_id) REFERENCES nadcl.dota_fantasy_players (id);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240120201558_FantasyDraftPlayerRename', '6.0.4');

COMMIT;


