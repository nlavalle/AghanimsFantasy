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