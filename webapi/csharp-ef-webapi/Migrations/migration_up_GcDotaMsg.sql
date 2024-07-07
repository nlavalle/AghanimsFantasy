START TRANSACTION;

CREATE TABLE nadcl.gc_dota_matches (
    match_id numeric(20,0) NOT NULL,
    duration bigint NOT NULL,
    starttime bigint NOT NULL,
    cluster bigint NOT NULL,
    first_blood_time bigint NOT NULL,
    replay_salt bigint NOT NULL,
    server_ip bigint NOT NULL,
    server_port bigint NOT NULL,
    lobby_type bigint NOT NULL,
    human_players bigint NOT NULL,
    average_skill bigint NOT NULL,
    game_balance real NOT NULL,
    radiant_team_id bigint NOT NULL,
    dire_team_id bigint NOT NULL,
    leagueid bigint NOT NULL,
    radiant_team_name text,
    dire_team_name text,
    radiant_team_logo numeric(20,0) NOT NULL,
    dire_team_logo numeric(20,0) NOT NULL,
    radiant_team_logo_url text,
    dire_team_logo_url text,
    radiant_team_complete bigint NOT NULL,
    dire_team_complete bigint NOT NULL,
    game_mode integer NOT NULL,
    match_seq_num numeric(20,0) NOT NULL,
    replay_state integer NOT NULL,
    radiant_guild_id bigint NOT NULL,
    dire_guild_id bigint NOT NULL,
    radiant_team_tag text,
    dire_team_tag text,
    series_id bigint NOT NULL,
    series_type bigint NOT NULL,
    engine bigint NOT NULL,
    match_flags bigint NOT NULL,
    private_metadata_key bigint NOT NULL,
    radiant_team_score bigint NOT NULL,
    dire_team_score bigint NOT NULL,
    match_outcome integer NOT NULL,
    tournament_id bigint NOT NULL,
    tournament_round bigint NOT NULL,
    pre_game_duration bigint NOT NULL,
    CONSTRAINT "PK_gc_dota_matches" PRIMARY KEY (match_id)
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240210091155_GcDotaMsg', '8.0.1');

COMMIT;


