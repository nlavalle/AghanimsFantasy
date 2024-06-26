START TRANSACTION;

ALTER TABLE nadcl.dota_gc_match_metadata DROP CONSTRAINT "FK_dota_gc_match_metadata_dota_leagues_LeagueId";

DROP TABLE nadcl.discord_users;

DROP TABLE nadcl.dota_gc_match_detail_player_damage_dealt;

DROP TABLE nadcl.dota_gc_match_detail_player_damage_received;

DROP TABLE nadcl.fantasy_match_player;

DROP TABLE nadcl.dota_gc_match_detail_players;

DROP TABLE nadcl.fantasy_match;

DROP INDEX nadcl."IX_dota_gc_match_metadata_LeagueId";

ALTER TABLE nadcl.dota_gc_match_details DROP CONSTRAINT "PK_dota_gc_match_details";

ALTER TABLE nadcl.dota_gc_match_metadata DROP COLUMN "LeagueId";

ALTER TABLE nadcl.dota_gc_match_details RENAME TO gc_dota_matches;

ALTER TABLE nadcl.gc_dota_matches ADD CONSTRAINT "PK_gc_dota_matches" PRIMARY KEY (match_id);

CREATE TABLE nadcl.balance_ledger (
    discord_id bigint GENERATED BY DEFAULT AS IDENTITY,
    tokens bigint NOT NULL,
    CONSTRAINT "PK_balance_ledger" PRIMARY KEY (discord_id)
);

CREATE TABLE nadcl.bets_streaks (
    discord_name text NOT NULL,
    streak bigint NOT NULL,
    CONSTRAINT "PK_bets_streaks" PRIMARY KEY (discord_name)
);

CREATE TABLE nadcl.bromance_last_60 (
    bro_1_name text NOT NULL,
    bro_2_name text NOT NULL,
    total_matches integer NOT NULL,
    total_wins integer NOT NULL,
    CONSTRAINT "PK_bromance_last_60" PRIMARY KEY (bro_1_name, bro_2_name)
);

CREATE TABLE nadcl.discord_ids (
    discord_id bigint GENERATED BY DEFAULT AS IDENTITY,
    account_id bigint NOT NULL,
    discord_name text NOT NULL,
    steam_id bigint NOT NULL,
    CONSTRAINT "PK_discord_ids" PRIMARY KEY (discord_id)
);

CREATE TABLE nadcl.match_status (
    match_id bigint GENERATED BY DEFAULT AS IDENTITY,
    status integer NOT NULL,
    CONSTRAINT "PK_match_status" PRIMARY KEY (match_id)
);

CREATE TABLE nadcl.matches_streaks (
    discord_name text NOT NULL,
    streak bigint NOT NULL,
    CONSTRAINT "PK_matches_streaks" PRIMARY KEY (discord_name)
);

CREATE TABLE nadcl.player_match_details (
    match_id bigint NOT NULL,
    player_slot integer NOT NULL,
    account_id bigint NOT NULL,
    aghanims_scepter integer,
    aghanims_shard integer,
    assists integer,
    backpack_0 integer,
    backpack_1 integer,
    backpack_2 integer,
    deaths integer,
    denies integer,
    gold integer,
    gold_per_min integer,
    gold_spent integer,
    hero_damage integer,
    hero_healing integer,
    hero_id integer NOT NULL,
    item_0 integer,
    item_1 integer,
    item_2 integer,
    item_3 integer,
    item_4 integer,
    item_5 integer,
    item_neutral integer,
    kills integer,
    last_hits integer,
    leaver_status integer,
    level integer,
    moonshard integer,
    net_worth bigint,
    scaled_hero_damage integer,
    scaled_hero_healing integer,
    scaled_tower_damage integer,
    team_number integer,
    team_slot integer,
    tower_damage integer,
    xp_per_min integer,
    CONSTRAINT "PK_player_match_details" PRIMARY KEY (match_id, player_slot)
);

CREATE UNIQUE INDEX "IX_dota_gc_match_metadata_match_id" ON nadcl.dota_gc_match_metadata (match_id);

ALTER TABLE nadcl.dota_gc_match_metadata ADD CONSTRAINT "FK_dota_gc_match_metadata_dota_match_details_match_id" FOREIGN KEY (match_id) REFERENCES nadcl.dota_match_details (match_id) ON DELETE CASCADE;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240611225940_FantasyMatch';

COMMIT;


