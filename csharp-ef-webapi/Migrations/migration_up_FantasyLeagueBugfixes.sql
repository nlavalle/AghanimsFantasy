START TRANSACTION;

ALTER TABLE nadcl.dota_gc_match_metadata_itempurchase DROP CONSTRAINT "FK_dota_gc_match_metadata_itempurchase_dota_gc_match_metadata_~";

ALTER TABLE nadcl.dota_gc_match_metadata_player DROP CONSTRAINT "FK_dota_gc_match_metadata_player_dota_gc_match_metadata_team_G~";

ALTER TABLE nadcl.dota_gc_match_metadata_playerkill DROP CONSTRAINT "FK_dota_gc_match_metadata_playerkill_dota_gc_match_metadata_pl~";

ALTER TABLE nadcl.dota_gc_match_metadata_team DROP CONSTRAINT "FK_dota_gc_match_metadata_team_dota_gc_match_metadata_GcMatchM~";

ALTER TABLE nadcl.dota_gc_match_metadata_tip DROP CONSTRAINT "FK_dota_gc_match_metadata_tip_dota_gc_match_metadata_GcMatchMe~";

ALTER TABLE nadcl.dota_gc_match_metadata_itempurchase ADD CONSTRAINT "FK_dota_gc_match_metadata_itempurchase_dota_gc_match_metadata_~" FOREIGN KEY ("GcMatchMetadataPlayerId") REFERENCES nadcl.dota_gc_match_metadata_player (id) ON DELETE CASCADE;

ALTER TABLE nadcl.dota_gc_match_metadata_player ADD CONSTRAINT "FK_dota_gc_match_metadata_player_dota_gc_match_metadata_team_G~" FOREIGN KEY ("GcMatchMetadataTeamId") REFERENCES nadcl.dota_gc_match_metadata_team (id) ON DELETE CASCADE;

ALTER TABLE nadcl.dota_gc_match_metadata_playerkill ADD CONSTRAINT "FK_dota_gc_match_metadata_playerkill_dota_gc_match_metadata_pl~" FOREIGN KEY ("GcMatchMetadataPlayerId") REFERENCES nadcl.dota_gc_match_metadata_player (id) ON DELETE CASCADE;

ALTER TABLE nadcl.dota_gc_match_metadata_team ADD CONSTRAINT "FK_dota_gc_match_metadata_team_dota_gc_match_metadata_GcMatchM~" FOREIGN KEY ("GcMatchMetadataId") REFERENCES nadcl.dota_gc_match_metadata (id) ON DELETE CASCADE;

ALTER TABLE nadcl.dota_gc_match_metadata_tip ADD CONSTRAINT "FK_dota_gc_match_metadata_tip_dota_gc_match_metadata_GcMatchMe~" FOREIGN KEY ("GcMatchMetadataId") REFERENCES nadcl.dota_gc_match_metadata (id) ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240217040324_FantasyLeagueBugfixes', '8.0.1');

COMMIT;


