drop view if exists nadcl.match_highlights;
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
	round(fpp.total_match_fantasy_points - avg(fpp.total_match_fantasy_points) over (partition by fpp.fantasy_league_id),2) as total_match_fantasy_points_diff,
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