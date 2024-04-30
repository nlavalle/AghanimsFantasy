START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_players DROP COLUMN team_position;

drop view if exists nadcl.match_highlights;
drop view if exists nadcl.fantasy_player_point_totals;
drop view if exists nadcl.fantasy_player_points;
create view nadcl.fantasy_player_points as
select distinct
	dfl.id as fantasy_league_id,
	dfp.id as fantasy_player_id,
	dmdp.id as match_details_player_id,
	dmdp.kills as kills,
	dmdp.kills * dflw.kills_weight as kills_points,
	dmdp.deaths as deaths,
	dmdp.deaths * dflw.deaths_weight as deaths_points,
	dmdp.assists as assists,
	dmdp.assists * dflw.assists_weight as assists_points,
	dmdp.last_hits as last_hits,
	dmdp.last_hits * dflw.last_hits_weight as last_hits_points,
	dmdp.gold_per_min as gold_per_min,
	dmdp.gold_per_min * dflw.gold_per_min_weight as gold_per_min_points,
	dmdp.xp_per_min as xp_per_min,
	dmdp.xp_per_min * dflw.xp_per_min_weight as xp_per_min_points,
	(
		dmdp.kills * dflw.kills_weight +
		dmdp.deaths * dflw.deaths_weight + 
		dmdp.assists * dflw.assists_weight + 
		dmdp.last_hits * dflw.last_hits_weight +
		dmdp.gold_per_min * dflw.gold_per_min_weight + 
		dmdp.xp_per_min * dflw.xp_per_min_weight
	) as total_match_fantasy_points	
from nadcl.dota_fantasy_leagues dfl
	join nadcl.dota_fantasy_league_weights dflw 
		on dfl.id = dflw.fantasy_league_id 
	join nadcl.dota_fantasy_players dfp
		on dfl.id = dfp.fantasy_league_id
	left join nadcl.dota_match_details dmd 
		on dfl.league_id = dmd.league_id
			and dfl.league_start_time <= dmd.start_time 
			and dfl.league_end_time >= dmd.start_time
	left join nadcl.dota_match_details_players dmdp 
		on dmd.match_id = dmdp.match_id and dmdp.account_id = dfp.dota_account_id
	left join nadcl.dota_gc_match_metadata dgmm 
		on dmd.match_id = dgmm.match_id
	left join nadcl.dota_gc_match_metadata_team dgmmt 
		on dgmmt."GcMatchMetadataId" = dgmm.id
	left join nadcl.dota_gc_match_metadata_player dgmmp 
		on dgmmp."GcMatchMetadataTeamId" = dgmmt.id 
			and dgmmp.player_slot = dmdp.player_slot
;

create view nadcl.match_highlights as
with stats as (
select
	fpp.fantasy_league_id,
	case
		when dmdp.team_number = 0 then dmh.radiant_team_id
		when dmdp.team_number = 1 then dmh.dire_team_id
		else null
	end as team_id,
	dmd.match_id,
	dmd.start_time,
	fpp.fantasy_player_id,
	fpp.kills,
	fpp.kills_points,
	round(fpp.kills_points - avg(fpp.kills_points) over (partition by fpp.fantasy_league_id),2) as kills_diff,
	abs(fpp.kills_points - avg(fpp.kills_points) over (partition by fpp.fantasy_league_id)) > stddev(fpp.kills_points) over (partition by fpp.fantasy_league_id) as kills_points_deviation,
	fpp.deaths,
	fpp.deaths_points,
	round(fpp.deaths_points - avg(fpp.deaths_points) over (partition by fpp.fantasy_league_id),2) as deaths_diff,
	abs(fpp.deaths_points - avg(fpp.deaths_points) over (partition by fpp.fantasy_league_id)) > stddev(fpp.deaths_points) over (partition by fpp.fantasy_league_id) as deaths_points_deviation,
	fpp.assists,
	fpp.assists_points,
	round(fpp.assists_points - avg(fpp.assists_points) over (partition by fpp.fantasy_league_id),2) as assists_diff,
	abs(fpp.assists_points - avg(fpp.assists_points) over (partition by fpp.fantasy_league_id)) > stddev(fpp.assists_points) over (partition by fpp.fantasy_league_id) as assists_points_deviation,
	fpp.last_hits,
	fpp.last_hits_points,
	round(fpp.last_hits_points - avg(fpp.last_hits_points) over (partition by fpp.fantasy_league_id),2) as last_hits_diff,
	abs(fpp.last_hits_points - avg(fpp.last_hits_points) over (partition by fpp.fantasy_league_id)) > stddev(fpp.last_hits_points) over (partition by fpp.fantasy_league_id) as last_hits_points_deviation,
	fpp.gold_per_min,
	fpp.gold_per_min_points,
	round(fpp.gold_per_min_points - avg(fpp.gold_per_min_points) over (partition by fpp.fantasy_league_id),2) as gold_per_min_diff,
	abs(fpp.gold_per_min_points - avg(fpp.gold_per_min_points) over (partition by fpp.fantasy_league_id)) > stddev(fpp.gold_per_min_points) over (partition by fpp.fantasy_league_id) as gold_per_min_deviation,
	fpp.xp_per_min,
	fpp.xp_per_min_points,
	round(fpp.xp_per_min_points - avg(fpp.xp_per_min_points) over (partition by fpp.fantasy_league_id),2) as xp_per_min_diff,
	abs(fpp.xp_per_min_points - avg(fpp.xp_per_min_points) over (partition by fpp.fantasy_league_id)) > stddev(fpp.xp_per_min_points) over (partition by fpp.fantasy_league_id) as xp_per_min_deviation,
	fpp.total_match_fantasy_points,
	round((fpp.total_match_fantasy_points - avg(fpp.total_match_fantasy_points) over (partition by fpp.fantasy_league_id))::numeric,2) as total_match_fantasy_points_diff,
	abs(fpp.total_match_fantasy_points - avg(fpp.total_match_fantasy_points) over (partition by fpp.fantasy_league_id)) > stddev(fpp.total_match_fantasy_points) over (partition by fpp.fantasy_league_id) as deviation,
	avg(fpp.total_match_fantasy_points) over (partition by fpp.fantasy_league_id) as avg_total,
	stddev(fpp.total_match_fantasy_points) over (partition by fpp.fantasy_league_id) as stdddev_total
from nadcl.dota_match_details dmd
	join nadcl.dota_match_history dmh
		on dmd.match_id = dmh.match_id
	join nadcl.dota_match_details_players dmdp 
		on dmd.match_id = dmdp.match_id 
	join nadcl.fantasy_player_points fpp 
		on dmdp.id = fpp.match_details_player_id 
)
select stats.*, da.id
from stats
	join nadcl.dota_fantasy_players dfp 
		on stats.fantasy_player_id = dfp.id
	join nadcl.dota_accounts da
		on dfp.dota_account_id = da.id
where deviation = true
order by stats.start_time desc
;

DELETE FROM "__EFMigrationsHistory"
WHERE "MigrationId" = '20240429065948_SupportScoring';

COMMIT;


