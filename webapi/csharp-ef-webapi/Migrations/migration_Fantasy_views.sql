create or replace view nadcl.fantasy_player_points as
select
	dfl.id as fantasy_league_id,
	dfp.id as fantasy_player_id,
	fmp.id as fantasy_match_player_id,
	fmp.kills as kills,
	fmp.kills * dflw.kills_weight as kills_points,
	fmp.deaths as deaths,
	fmp.deaths * dflw.deaths_weight as deaths_points,
	fmp.assists as assists,
	fmp.assists * dflw.assists_weight as assists_points,
	fmp.last_hits as last_hits,
	fmp.last_hits * dflw.last_hits_weight as last_hits_points,
	fmp.gold_per_min as gold_per_min,
	fmp.gold_per_min * dflw.gold_per_min_weight as gold_per_min_points,
	fmp.xp_per_min as xp_per_min,
	fmp.xp_per_min * dflw.xp_per_min_weight as xp_per_min_points,
	fmp.net_worth as networth,
	fmp.net_worth * dflw.networth_weight as networth_points,
	fmp.hero_damage as hero_damage,
	fmp.hero_damage * dflw.hero_damage_weight as hero_damage_points,
	fmp.tower_damage as tower_damage,
	fmp.tower_damage * dflw.tower_damage_weight as tower_damage_points,
	fmp.hero_healing as hero_healing,
	fmp.hero_healing * dflw.hero_healing_weight as hero_healing_points,
	fmp.gold as gold,
	fmp.gold * dflw.gold_weight as gold_points,
	fmp.fight_score as fight_score,
	fmp.fight_score * dflw.fight_score_weight as fight_score_points,
	fmp.farm_score as farm_score,
	fmp.farm_score * dflw.farm_score_weight as farm_score_points,
	fmp.support_score as support_score,
	fmp.support_score * dflw.support_score_weight as support_score_points,
	fmp.push_score as push_score,
	fmp.push_score * dflw.push_score_weight as push_score_points,
	fmp.hero_xp as hero_xp,
	fmp.hero_xp * dflw.hero_xp_weight as hero_xp_points,
	fmp.camps_stacked as camps_stacked,
	fmp.camps_stacked * dflw.camps_stacked_weight as camps_stacked_points,
	fmp.rampages as rampages,
	fmp.rampages * dflw.rampages_weight as rampages_points,
	fmp.triple_kills as triple_kills,
	fmp.triple_kills * dflw.triple_kills_weight as triple_kills_points,
	fmp.aegis_snatched as aegis_snatched,
	fmp.aegis_snatched * dflw.aegis_snatched_weight as aegis_snatched_points,	
	fmp.rapiers_purchased as rapiers_purchased,
	fmp.rapiers_purchased * dflw.rapiers_purchased_weight as rapiers_purchased_points,
	fmp.couriers_killed as couriers_killed,
	fmp.couriers_killed * dflw.couriers_killed_weight as couriers_killed_points,
	fmp.support_gold_spent as support_gold_spent,
	fmp.support_gold_spent * dflw.support_gold_spent_weight as support_gold_spent_points,
	fmp.observer_wards_placed as observer_wards_placed,
	fmp.observer_wards_placed * dflw.observer_wards_placed_weight as observer_wards_placed_points,
	fmp.sentry_wards_placed as sentry_wards_placed,
	fmp.sentry_wards_placed * dflw.sentry_wards_placed_weight as sentry_wards_placed_points,
	fmp.dewards as wards_dewarded,
	fmp.dewards * dflw.wards_dewarded_weight as wards_dewarded_points,
	fmp.stun_duration as stun_duration,
	fmp.stun_duration * dflw.stun_duration_weight as stun_duration_points,	
	(
		coalesce(fmp.kills * dflw.kills_weight, 0) +
		coalesce(fmp.deaths * dflw.deaths_weight, 0) + 
		coalesce(fmp.assists * dflw.assists_weight, 0) + 
		coalesce(fmp.last_hits * dflw.last_hits_weight, 0) +
		coalesce(fmp.gold_per_min * dflw.gold_per_min_weight, 0) + 
		coalesce(fmp.xp_per_min * dflw.xp_per_min_weight, 0) + 
		coalesce(fmp.net_worth * dflw.networth_weight, 0) +
		coalesce(fmp.hero_damage * dflw.hero_damage_weight, 0) +
		coalesce(fmp.tower_damage * dflw.tower_damage_weight, 0) +
		coalesce(fmp.hero_healing * dflw.hero_healing_weight, 0) +
		coalesce(fmp.gold * dflw.gold_weight, 0) +
		coalesce(fmp.fight_score * dflw.fight_score_weight, 0) +
		coalesce(fmp.farm_score * dflw.farm_score_weight, 0) + 
		coalesce(fmp.support_score * dflw.support_score_weight, 0) +
		coalesce(fmp.push_score * dflw.push_score_weight, 0) +
		coalesce(fmp.hero_xp * dflw.hero_xp_weight, 0) +
		coalesce(fmp.camps_stacked * dflw.camps_stacked_weight, 0) +
		coalesce(fmp.rampages * dflw.rampages_weight, 0) +
		coalesce(fmp.triple_kills * dflw.triple_kills_weight, 0) +
		coalesce(fmp.aegis_snatched * dflw.aegis_snatched_weight, 0) +	
		coalesce(fmp.rapiers_purchased * dflw.rapiers_purchased_weight, 0) +
		coalesce(fmp.couriers_killed * dflw.couriers_killed_weight, 0) +
		coalesce(fmp.support_gold_spent * dflw.support_gold_spent_weight, 0) +
		coalesce(fmp.observer_wards_placed * dflw.observer_wards_placed_weight, 0) +
		coalesce(fmp.sentry_wards_placed * dflw.sentry_wards_placed_weight, 0) +
		coalesce(fmp.dewards * dflw.wards_dewarded_weight, 0) +
		coalesce(fmp.stun_duration * dflw.stun_duration_weight, 0)	
	)::numeric as total_match_fantasy_points	
from nadcl.dota_fantasy_leagues dfl
	join nadcl.dota_fantasy_league_weights dflw 
		on dfl.id = dflw.fantasy_league_id
	join nadcl.dota_fantasy_players dfp
		on dfl.id = dfp.fantasy_league_id
	left join nadcl.fantasy_match fm
		on dfl.league_id = fm.league_id
			and dfl.league_start_time <= fm.start_time 
			and dfl.league_end_time >= fm.start_time
	left join nadcl.fantasy_match_player fmp 
		on fmp.match_id = fm.match_id and fmp.account_id = dfp.dota_account_id
;

create or replace view nadcl.match_highlights as
with stats as (
select
	fpp.fantasy_league_id,
	case
		when fmp.dota_team_side = false then fm."RadiantTeamId"
		when fmp.dota_team_side = true then fm."DireTeamId"
		else null
	end as team_id,
	fm.match_id,
	fm.start_time,
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
from nadcl.fantasy_match fm
	join nadcl.fantasy_match_player fmp
		on fm.match_id = fmp.match_id
	join nadcl.fantasy_player_points fpp 
		on fmp.id = fpp.fantasy_match_player_id 
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

create or replace view nadcl.fantasy_player_point_totals as
SELECT 
	fantasy_league_id, 
	fantasy_player_id, 
	count(distinct fantasy_match_player_id) as matches,
	coalesce(sum(kills),0) as kills,
	coalesce(sum(kills_points),0) as kills_points,
	coalesce(sum(deaths),0) as deaths,
	coalesce(sum(deaths_points),0) as deaths_points,
	coalesce(sum(assists),0) as assists,
	coalesce(sum(assists_points),0) as assists_points,
	coalesce(sum(last_hits),0) as last_hits,
	coalesce(sum(last_hits_points),0) as last_hits_points,
	coalesce(avg(gold_per_min),0) as gold_per_min,
	coalesce(sum(gold_per_min_points),0) as gold_per_min_points,
	coalesce(avg(xp_per_min),0) as xp_per_min,
	coalesce(sum(xp_per_min_points),0) as xp_per_min_points,
	coalesce(sum(networth),0) as networth,
	coalesce(sum(networth_points),0) as networth_points,
	coalesce(sum(hero_damage),0) as hero_damage,
	coalesce(sum(hero_damage_points),0) as hero_damage_points,
	coalesce(sum(tower_damage),0) as tower_damage,
	coalesce(sum(tower_damage_points),0) as tower_damage_points,
	coalesce(sum(hero_healing),0) as hero_healing,
	coalesce(sum(hero_healing_points),0) as hero_healing_points,
	coalesce(sum(gold),0) as gold,
	coalesce(sum(gold_points),0) as gold_points,
	coalesce(sum(fight_score),0) as fight_score,
	coalesce(sum(fight_score_points),0) as fight_score_points,
	coalesce(sum(farm_score),0) as farm_score,
	coalesce(sum(farm_score_points),0) as farm_score_points,
	coalesce(sum(support_score),0) as support_score,
	coalesce(sum(support_score_points),0) as support_score_points,
	coalesce(sum(push_score),0) as push_score,
	coalesce(sum(push_score_points),0) as push_score_points,
	coalesce(sum(hero_xp),0) as hero_xp,
	coalesce(sum(hero_xp_points),0) as hero_xp_points,
	coalesce(sum(camps_stacked),0) as camps_stacked,
	coalesce(sum(camps_stacked_points),0) as camps_stacked_points,
	coalesce(sum(rampages),0) as rampages,
	coalesce(sum(rampages_points),0) as rampages_points,
	coalesce(sum(triple_kills),0) as triple_kills,
	coalesce(sum(triple_kills_points),0) as triple_kills_points,
	coalesce(sum(aegis_snatched),0) as aegis_snatched,
	coalesce(sum(aegis_snatched_points),0) as aegis_snatched_points,
	coalesce(sum(rapiers_purchased),0) as rapiers_purchased,
	coalesce(sum(rapiers_purchased_points),0) as rapiers_purchased_points,
	coalesce(sum(couriers_killed),0) as couriers_killed,
	coalesce(sum(couriers_killed_points),0) as couriers_killed_points,
	coalesce(sum(support_gold_spent),0) as support_gold_spent,
	coalesce(sum(support_gold_spent_points),0) as support_gold_spent_points,
	coalesce(sum(observer_wards_placed),0) as observer_wards_placed,
	coalesce(sum(observer_wards_placed_points),0) as observer_wards_placed_points,
	coalesce(sum(sentry_wards_placed),0) as sentry_wards_placed,
	coalesce(sum(sentry_wards_placed_points),0) as sentry_wards_placed_points,
	coalesce(sum(wards_dewarded),0) as wards_dewarded,
	coalesce(sum(wards_dewarded_points),0) as wards_dewarded_points,
	coalesce(sum(stun_duration),0) as stun_duration,
	coalesce(sum(stun_duration_points),0) as stun_duration_points,
	coalesce(sum(total_match_fantasy_points),0) as total_match_fantasy_points
FROM nadcl.fantasy_player_points
group by fantasy_league_id, fantasy_player_id
order by total_match_fantasy_points desc
;

create or replace view nadcl.fantasy_normalized_averages as
with avg_scores as (
	select 
		fp.dota_account_id,
		count(DISTINCT match_details_player_id) as matches_played,
		avg(kills_points) as kills_points,
		avg(deaths_points) as deaths_points,
		avg(assists_points) as assists_points,
		avg(last_hits_points) as last_hits_points,
		avg(gold_per_min_points) as gold_per_min_points,
		avg(xp_per_min_points) as xp_per_min_points,
		avg(networth_points) as networth_points,
		avg(hero_damage_points) as hero_damage_points,
		avg(tower_damage_points) as tower_damage_points,
		avg(hero_healing_points) as hero_healing_points,
		avg(gold_points) as gold_points,
		avg(hero_xp_points) as hero_xp_points,
		avg(camps_stacked_points) as camps_stacked_points,
		avg(rampages_points) as rampages_points,
		avg(triple_kills_points) as triple_kills_points,
		avg(aegis_snatched_points) as aegis_snatched_points,
		avg(rapiers_purchased_points) as rapiers_purchased_points,
		avg(couriers_killed_points) as couriers_killed_points,
		avg(support_gold_spent_points) as support_gold_spent_points,
		avg(observer_wards_placed_points) as observer_wards_placed_points,
		avg(sentry_wards_placed_points) as sentry_wards_placed_points,
		avg(wards_dewarded_points) as wards_dewarded_points,
		avg(stun_duration_points) as stun_duration_points,
		avg(total_match_fantasy_points) as total_match_fantasy_points,
		avg(fight_score) as fight_score,
		avg(farm_score) as farm_score,
		avg(support_score) as support_score,
		avg(push_score) as push_score
	from nadcl.fantasy_player_points fpp
		join nadcl.dota_fantasy_players fp
			on fpp.fantasy_player_id = fp.id
		join nadcl.dota_accounts da
			on fp.dota_account_id = da.id
	where fight_score is not null
	group by fp.dota_account_id
), min_max as (
	select
		min(matches_played) as matches_played_min,
		max(matches_played) as matches_played_max,
		min(kills_points) as kills_points_min,
		max(kills_points) as kills_points_max,
		min(deaths_points) as deaths_points_min,
		max(deaths_points) as deaths_points_max,
		min(assists_points) as assists_points_min,
		max(assists_points) as assists_points_max,
		min(last_hits_points) as last_hits_points_min,
		max(last_hits_points) as last_hits_points_max,
		min(gold_per_min_points) as gold_per_min_points_min,
		max(gold_per_min_points) as gold_per_min_points_max,
		min(xp_per_min_points) as xp_per_min_points_min,
		max(xp_per_min_points) as xp_per_min_points_max,
		min(networth_points) as networth_points_min,
		max(networth_points) as networth_points_max,
		min(hero_damage_points) as hero_damage_points_min,
		max(hero_damage_points) as hero_damage_points_max,
		min(tower_damage_points) as tower_damage_points_min,
		max(tower_damage_points) as tower_damage_points_max,
		min(hero_healing_points) as hero_healing_points_min,
		max(hero_healing_points) as hero_healing_points_max,
		min(gold_points) as gold_points_min,
		max(gold_points) as gold_points_max,
		min(hero_xp_points) as hero_xp_points_min,
		max(hero_xp_points) as hero_xp_points_max,
		min(camps_stacked_points) as camps_stacked_points_min,
		max(camps_stacked_points) as camps_stacked_points_max,
		min(rampages_points) as rampages_points_min,
		max(rampages_points) as rampages_points_max,
		min(triple_kills_points) as triple_kills_points_min,
		max(triple_kills_points) as triple_kills_points_max,
		min(aegis_snatched_points) as aegis_snatched_points_min,
		max(aegis_snatched_points) as aegis_snatched_points_max,
		min(rapiers_purchased_points) as rapiers_purchased_points_min,
		max(rapiers_purchased_points) as rapiers_purchased_points_max,
		min(couriers_killed_points) as couriers_killed_points_min,
		max(couriers_killed_points) as couriers_killed_points_max,
		min(support_gold_spent_points) as support_gold_spent_points_min,
		max(support_gold_spent_points) as support_gold_spent_points_max,
		min(observer_wards_placed_points) as observer_wards_placed_points_min,
		max(observer_wards_placed_points) as observer_wards_placed_points_max,
		min(sentry_wards_placed_points) as sentry_wards_placed_points_min,
		max(sentry_wards_placed_points) as sentry_wards_placed_points_max,
		min(wards_dewarded_points) as wards_dewarded_points_min,
		max(wards_dewarded_points) as wards_dewarded_points_max,
		min(stun_duration_points) as stun_duration_points_min,
		max(stun_duration_points) as stun_duration_points_max,
		min(total_match_fantasy_points) as total_match_fantasy_points_min,
		max(total_match_fantasy_points) as total_match_fantasy_points_max,
		
		-- Valve scores
		min(fight_score) as fight_score_min,
		max(fight_score) as fight_score_max,
		min(farm_score) as farm_score_min,
		max(farm_score) as farm_score_max,
		min(support_score) as support_score_min,
		max(support_score) as support_score_max,
		min(push_score) as push_score_min,
		max(push_score) as push_score_max
	from avg_scores
)
select
	fp.id as fantasy_player_id,
	(matches_played - matches_played_min) / nullif(matches_played_max - matches_played_min, 0)::numeric as matches_played,
	(kills_points - kills_points_min) / nullif(kills_points_max - kills_points_min, 0) as kills_points,
	(deaths_points - deaths_points_min) / nullif(deaths_points_max - deaths_points_min, 0) as deaths_points,
	(assists_points - assists_points_min) / nullif(assists_points_max - assists_points_min, 0) as assists_points,
	(last_hits_points - last_hits_points_min) / nullif(last_hits_points_max - last_hits_points_min, 0) as last_hits_points,
	(gold_per_min_points - gold_per_min_points_min) / nullif(gold_per_min_points_max - gold_per_min_points_min, 0) as gold_per_min_points,
	(xp_per_min_points - xp_per_min_points_min) / nullif(xp_per_min_points_max - xp_per_min_points_min, 0) as xp_per_min_points,
	(networth_points - networth_points_min) / nullif(networth_points_max - networth_points_min, 0) as networth_points,
	(hero_damage_points - hero_damage_points_min) / nullif(hero_damage_points_max - hero_damage_points_min, 0) as hero_damage_points,
	(tower_damage_points - tower_damage_points_min) / nullif(tower_damage_points_max - tower_damage_points_min, 0) as tower_damage_points,
	(hero_healing_points - hero_healing_points_min) / nullif(hero_healing_points_max - hero_healing_points_min, 0) as hero_healing_points,
	(gold_points - gold_points_min) / nullif(gold_points_max - gold_points_min, 0) as gold_points,
	(hero_xp_points - hero_xp_points_min) / nullif(hero_xp_points_max - hero_xp_points_min, 0) as hero_xp_points,
	(camps_stacked_points - camps_stacked_points_min) / nullif(camps_stacked_points_max - camps_stacked_points_min, 0) as camps_stacked_points,
	(rampages_points - rampages_points_min) / nullif(rampages_points_max - rampages_points_min, 0) as rampages_points,
	(triple_kills_points - triple_kills_points_min) / nullif(triple_kills_points_max - triple_kills_points_min, 0) as triple_kills_points,
	(aegis_snatched_points - aegis_snatched_points_min) / nullif(aegis_snatched_points_max - aegis_snatched_points_min, 0) as aegis_snatched_points,
	(rapiers_purchased_points - rapiers_purchased_points_min) / nullif(rapiers_purchased_points_max - rapiers_purchased_points_min, 0) as rapiers_purchased_points,
	(couriers_killed_points - couriers_killed_points_min) / nullif(couriers_killed_points_max - couriers_killed_points_min, 0) as couriers_killed_points,
	(support_gold_spent_points - support_gold_spent_points_min) / nullif(support_gold_spent_points_max - support_gold_spent_points_min, 0) as support_gold_spent_points,
	(observer_wards_placed_points - observer_wards_placed_points_min) / nullif(observer_wards_placed_points_max - observer_wards_placed_points_min, 0) as observer_wards_placed_points,
	(sentry_wards_placed_points - sentry_wards_placed_points_min) / nullif(sentry_wards_placed_points_max - sentry_wards_placed_points_min, 0) as sentry_wards_placed_points,
	(wards_dewarded_points - wards_dewarded_points_min) / nullif(wards_dewarded_points_max - wards_dewarded_points_min, 0) as wards_dewarded_points,
	(stun_duration_points - stun_duration_points_min) / nullif(stun_duration_points_max - stun_duration_points_min, 0) as stun_duration_points,
	(total_match_fantasy_points - total_match_fantasy_points_min) / nullif(total_match_fantasy_points_max - total_match_fantasy_points_min, 0) as total_match_fantasy_points,
	-- Valve scores
	(fight_score - fight_score_min) / nullif(fight_score_max - fight_score_min, 0) as fight_score,
	(farm_score - farm_score_min) / nullif(farm_score_max - farm_score_min, 0) as farm_score,
	(support_score - support_score_min) / nullif(support_score_max - support_score_min, 0) as support_score,
	(push_score - push_score_min) / nullif(push_score_max - push_score_min, 0) as push_score
from avg_scores
	cross join min_max
	join nadcl.dota_fantasy_players fp
		on avg_scores.dota_account_id = fp.dota_account_id
;

create or replace view nadcl.fantasy_match_metadata as
select
	fl.id as fantasy_league_id,
	fp.id as fantasy_player_id,
	count(fmp.match_id) as matches_played,
	coalesce(sum(fmp.kills),0) as kills_sum,
	coalesce(avg(fmp.kills),0) as kills_avg,
	coalesce(sum(fmp.deaths),0) as deaths_sum,
	coalesce(avg(fmp.deaths),0) as deaths_avg,
	coalesce(sum(fmp.assists),0) as assists_sum,
	coalesce(avg(fmp.assists),0) as assists_avg,
	coalesce(sum(fmp.last_hits),0) as last_hits_sum,
	coalesce(avg(fmp.last_hits),0) as last_hits_avg,
	coalesce(sum(fmp.denies),0) as denies_sum,
	coalesce(avg(fmp.denies),0) as denies_avg,
	coalesce(sum(fmp.gold_per_min),0) as gold_per_min_sum,
	coalesce(avg(fmp.gold_per_min),0) as gold_per_min_avg,
	coalesce(sum(fmp.xp_per_min),0) as xp_per_min_sum,
	coalesce(avg(fmp.xp_per_min),0) as xp_per_min_avg,
	coalesce(sum(fmp.support_gold_spent),0) as support_gold_spent_sum,
	coalesce(avg(fmp.support_gold_spent),0) as support_gold_spent_avg,
	coalesce(sum(fmp.observer_wards_placed),0) as observer_wards_placed_sum,
	coalesce(avg(fmp.observer_wards_placed),0) as observer_wards_placed_avg,
	coalesce(sum(fmp.sentry_wards_placed),0) as sentry_wards_placed_sum,
	coalesce(avg(fmp.sentry_wards_placed),0) as sentry_wards_placed_avg,
	coalesce(sum(fmp.dewards),0) as dewards_sum,
	coalesce(avg(fmp.dewards),0) as dewards_avg,
	coalesce(sum(fmp.camps_stacked),0) as camps_stacked_sum,
	coalesce(avg(fmp.camps_stacked),0) as camps_stacked_avg,
	coalesce(sum(fmp.stun_duration),0) as stun_duration_sum,
	coalesce(avg(fmp.stun_duration),0) as stun_duration_avg,
	coalesce(sum(fmp.net_worth),0) as net_worth_sum,
	coalesce(avg(fmp.net_worth),0) as net_worth_avg,
	coalesce(sum(fmp.hero_damage),0) as hero_damage_sum,
	coalesce(avg(fmp.hero_damage),0) as hero_damage_avg,
	coalesce(sum(fmp.tower_damage),0) as tower_damage_sum,
	coalesce(avg(fmp.tower_damage),0) as tower_damage_avg,
	coalesce(sum(fmp.hero_healing),0) as hero_healing_sum,
	coalesce(avg(fmp.hero_healing),0) as hero_healing_avg,
	coalesce(sum(fmp.gold),0) as gold_sum,
	coalesce(avg(fmp.gold),0) as gold_avg
from nadcl.dota_fantasy_leagues fl
	join nadcl.dota_fantasy_players fp
		on fl.id = fp.fantasy_league_id
	left join nadcl.fantasy_match fm
		on fl.league_id = fm.league_id
			and fl.league_start_time <= fm.start_time
			and fl.league_end_time >= fm.start_time
	left join nadcl.fantasy_match_player fmp
		on fm.match_id = fmp.match_id and fmp."AccountId" = fp.dota_account_id
group by fl.id, fp.id
;

create or replace view nadcl.fantasy_player_probabilities as
with quintiles as (
    select
        allfl.id as fantasy_league_id,
        a.id as account_id,
        fl.id as past_fantasy_league_id,
        fl.league_end_time,
        fppt.matches,
        fppt.total_match_fantasy_points,
		fppt.total_match_fantasy_points / fppt.matches as avg,
        NTILE(5) over (partition by fl.id order by fppt.total_match_fantasy_points desc) as quintile,
        fl.league_start_time,
        row_number() over (partition by allfl.id, a.id order by fl.league_start_time desc) row_num
    from nadcl.dota_fantasy_leagues allfl
        left join nadcl.dota_fantasy_leagues fl
            on allfl.league_start_time > fl.league_end_time
        join nadcl.fantasy_player_point_totals fppt
            on fppt.fantasy_league_id = fl.id
        join nadcl.dota_fantasy_players fp
            on fppt.fantasy_player_id = fp.id
        join nadcl.dota_accounts a
            on fp.dota_account_id = a.id
        -- Filter this to only fantasy players
        join nadcl.dota_fantasy_players fpfilter
            on allfl.id = fpfilter.fantasy_league_id
                and fp.dota_account_id = fpfilter.dota_account_id
    where fl.is_private = false
        and fppt.matches > 0
), recent_fantasies as (
    select 
        fantasy_league_id,
        account_id,
        quintile
    from quintiles
    where row_num <= 20
), quintile_counts as (
    select
        fantasy_league_id,
        account_id,
        quintile,
        count(*) as count_per_quintile
    from recent_fantasies
    group by fantasy_league_id, account_id, quintile
), total_games as (
    select
        fantasy_league_id,
        account_id,
        cross_quintile as quintile,
        sum(count_per_quintile) as total_games
    from quintile_counts
    cross join (
        select column1 as cross_quintile
        from (values(1),(2),(3),(4),(5))
    )
    group by fantasy_league_id, account_id, cross_quintile
)
select distinct
    a.fantasy_league_id,
    a.account_id,
    a.quintile,
	-- Doing a dirichlet prior of [1, 1, 1, 1, 1] to help even out players with v few games
    round((coalesce(p.count_per_quintile, 0) + 1) / (a.total_games + 5), 4) as probability,
    round(sum((coalesce(p.count_per_quintile, 0) + 1) / (a.total_games + 5)) over (partition by a.fantasy_league_id, a.account_id order by a.quintile),4) as cumulative_probability
from total_games a
    left join quintile_counts p
        on a.account_id = p.account_id 
            and a.quintile = p.quintile
            and a.fantasy_league_id = p.fantasy_league_id
order by fantasy_league_id desc, account_id, quintile
;

create or replace view nadcl.fantasy_account_top_heroes as
with player_games as (
select
    fmp.account_id, 
    fmp."HeroId" as hero_id,
    fmp.id, 
    fmp.dota_team_side,
    fm.radiant_win,
    row_number() over (partition by fmp.account_id order by fm.start_time desc) row_num
from nadcl.fantasy_match fm
    join nadcl.fantasy_match_player fmp
        on fm.match_id = fmp.match_id
), win_loss as (
select
    account_id, 
    hero_id, 
    count(id) as count, 
    sum(case 
        when dota_team_side = false and radiant_win = true then 1
        when dota_team_side = true and radiant_win = false then 1
        else 0
    end) as wins,
    sum(case 
        when dota_team_side = true and radiant_win = true then 1
        when dota_team_side = false and radiant_win = false then 1
        else 0
    end) as losses,
	row_number() over (partition by account_id order by count(id) desc) as row_num
from player_games
where row_num <= 30
group by account_id, hero_id
)
select
	account_id,
	hero_id,
	count,
	wins,
	losses
from win_loss
where row_num <= 3
order by account_id, count desc
;
