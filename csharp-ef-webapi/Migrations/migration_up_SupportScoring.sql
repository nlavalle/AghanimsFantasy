START TRANSACTION;

ALTER TABLE nadcl.dota_fantasy_players ADD team_position integer NOT NULL DEFAULT 0;

drop view if exists nadcl.match_highlights;
drop view if exists nadcl.fantasy_player_point_totals;
drop view if exists nadcl.fantasy_player_points;
create view nadcl.fantasy_player_points as
select
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
	dmdp.net_worth as networth,
	dmdp.net_worth * dflw.networth_weight as networth_points,
	dmdp.hero_damage as hero_damage,
	dmdp.hero_damage * dflw.hero_damage_weight as hero_damage_points,
	dmdp.tower_damage as tower_damage,
	dmdp.tower_damage * dflw.tower_damage_weight as tower_damage_points,
	dmdp.hero_healing as hero_healing,
	dmdp.hero_healing * dflw.hero_healing_weight as hero_healing_points,
	dmdp.gold as gold,
	dmdp.gold * dflw.gold_weight as gold_points,
	dgmmp.fight_score as fight_score,
	dgmmp.fight_score * dflw.fight_score_weight as fight_score_points,
	dgmmp.farm_score as farm_score,
	dgmmp.farm_score * dflw.farm_score_weight as farm_score_points,
	dgmmp.support_score as support_score,
	dgmmp.support_score * dflw.support_score_weight as support_score_points,
	dgmmp.push_score as push_score,
	dgmmp.push_score * dflw.push_score_weight as push_score_points,
	dgmmp.hero_xp as hero_xp,
	dgmmp.hero_xp * dflw.hero_xp_weight as hero_xp_points,
	dgmmp.camps_stacked as camps_stacked,
	dgmmp.camps_stacked * dflw.camps_stacked_weight as camps_stacked_points,
	dgmmp.rampages as rampages,
	dgmmp.rampages * dflw.rampages_weight as rampages_points,
	dgmmp.triple_kills as triple_kills,
	dgmmp.triple_kills * dflw.triple_kills_weight as triple_kills_points,
	dgmmp.aegis_snatched as aegis_snatched,
	dgmmp.aegis_snatched * dflw.aegis_snatched_weight as aegis_snatched_points,	
	dgmmp.rapiers_purchased as rapiers_purchased,
	dgmmp.rapiers_purchased * dflw.rapiers_purchased_weight as rapiers_purchased_points,
	dgmmp.couriers_killed as couriers_killed,
	dgmmp.couriers_killed * dflw.couriers_killed_weight as couriers_killed_points,
	dgmmp.support_gold_spent as support_gold_spent,
	dgmmp.support_gold_spent * dflw.support_gold_spent_weight as support_gold_spent_points,
	dgmmp.observer_wards_placed as observer_wards_placed,
	dgmmp.observer_wards_placed * dflw.observer_wards_placed_weight as observer_wards_placed_points,
	dgmmp.sentry_wards_placed as sentry_wards_placed,
	dgmmp.sentry_wards_placed * dflw.sentry_wards_placed_weight as sentry_wards_placed_points,
	dgmmp.wards_dewarded as wards_dewarded,
	dgmmp.wards_dewarded * dflw.wards_dewarded_weight as wards_dewarded_points,
	dgmmp.stun_duration as stun_duration,
	dgmmp.stun_duration * dflw.stun_duration_weight as stun_duration_points,	
	(
		dmdp.kills * dflw.kills_weight +
		dmdp.deaths * dflw.deaths_weight + 
		dmdp.assists * dflw.assists_weight + 
		dmdp.last_hits * dflw.last_hits_weight +
		dmdp.gold_per_min * dflw.gold_per_min_weight + 
		dmdp.xp_per_min * dflw.xp_per_min_weight + 
		dmdp.net_worth * dflw.networth_weight +
		dmdp.hero_damage * dflw.hero_damage_weight +
		dmdp.tower_damage * dflw.tower_damage_weight +
		dmdp.hero_healing * dflw.hero_healing_weight +
		dmdp.gold * dflw.gold_weight +
		dgmmp.fight_score * dflw.fight_score_weight +
		dgmmp.farm_score * dflw.farm_score_weight + 
		dgmmp.support_score * dflw.support_score_weight +
		dgmmp.push_score * dflw.push_score_weight +
		dgmmp.hero_xp * dflw.hero_xp_weight +
		dgmmp.camps_stacked * dflw.camps_stacked_weight +
		dgmmp.rampages * dflw.rampages_weight +
		dgmmp.triple_kills * dflw.triple_kills_weight +
		dgmmp.aegis_snatched * dflw.aegis_snatched_weight +	
		dgmmp.rapiers_purchased * dflw.rapiers_purchased_weight +
		dgmmp.couriers_killed * dflw.couriers_killed_weight +
		dgmmp.support_gold_spent * dflw.support_gold_spent_weight +
		dgmmp.observer_wards_placed * dflw.observer_wards_placed_weight +
		dgmmp.sentry_wards_placed * dflw.sentry_wards_placed_weight +
		dgmmp.wards_dewarded * dflw.wards_dewarded_weight +
		dgmmp.stun_duration * dflw.stun_duration_weight	
	)::numeric as total_match_fantasy_points	
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
where dgmmt.id is null or 
	(dgmmt.id is not null and dgmmp.id is not null)
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

create view nadcl.fantasy_player_point_totals as
SELECT 
	fantasy_league_id, 
	fantasy_player_id, 
	count(distinct match_details_player_id) as matches,
	sum(kills) as kills,
	sum(kills_points) as kills_points,
	sum(deaths) as deaths,
	sum(deaths_points) as deaths_points,
	sum(assists) as assists,
	sum(assists_points) as assists_points,
	sum(last_hits) as last_hits,
	sum(last_hits_points) as last_hits_points,
	avg(gold_per_min) as gold_per_min,
	sum(gold_per_min_points) as gold_per_min_points,
	avg(xp_per_min) as xp_per_min,
	sum(xp_per_min_points) as xp_per_min_points,
	sum(networth) as networth,
	sum(networth_points) as networth_points,
	sum(hero_damage) as hero_damage,
	sum(hero_damage_points) as hero_damage_points,
	sum(tower_damage) as tower_damage,
	sum(tower_damage_points) as tower_damage_points,
	sum(hero_healing) as hero_healing,
	sum(hero_healing_points) as hero_healing_points,
	sum(gold) as gold,
	sum(gold_points) as gold_points,
	sum(fight_score) as fight_score,
	sum(fight_score_points) as fight_score_points,
	sum(farm_score) as farm_score,
	sum(farm_score_points) as farm_score_points,
	sum(support_score) as support_score,
	sum(support_score_points) as support_score_points,
	sum(push_score) as push_score,
	sum(push_score_points) as push_score_points,
	sum(hero_xp) as hero_xp,
	sum(hero_xp_points) as hero_xp_points,
	sum(camps_stacked) as camps_stacked,
	sum(camps_stacked_points) as camps_stacked_points,
	sum(rampages) as rampages,
	sum(rampages_points) as rampages_points,
	sum(triple_kills) as triple_kills,
	sum(triple_kills_points) as triple_kills_points,
	sum(aegis_snatched) as aegis_snatched,
	sum(aegis_snatched_points) as aegis_snatched_points,
	sum(rapiers_purchased) as rapiers_purchased,
	sum(rapiers_purchased_points) as rapiers_purchased_points,
	sum(couriers_killed) as couriers_killed,
	sum(couriers_killed_points) as couriers_killed_points,
	sum(support_gold_spent) as support_gold_spent,
	sum(support_gold_spent_points) as support_gold_spent_points,
	sum(observer_wards_placed) as observer_wards_placed,
	sum(observer_wards_placed_points) as observer_wards_placed_points,
	sum(sentry_wards_placed) as sentry_wards_placed,
	sum(sentry_wards_placed_points) as sentry_wards_placed_points,
	sum(wards_dewarded) as wards_dewarded,
	sum(wards_dewarded_points) as wards_dewarded_points,
	sum(stun_duration) as stun_duration,
	sum(stun_duration_points) as stun_duration_points,
	sum(total_match_fantasy_points) as total_match_fantasy_points
FROM nadcl.fantasy_player_points
group by fantasy_league_id, fantasy_player_id
;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240429065948_SupportScoring', '8.0.1');

COMMIT;


