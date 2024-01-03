 

from app_db import PGDB
import pandas as pd

from sqlalchemy import create_engine

from tokens import CONNECTION_STRING

engine = create_engine('sqlite:///dotabet_export.db', echo=True)
sqlite_connection = engine.connect()

db = PGDB(CONNECTION_STRING,'DotaWebAPI')

def convert_steam_to_account(steam_id):
	id_str = str(steam_id)
	id64_base = 76561197960265728
	offset_id = int(id_str) - id64_base
	account_type = offset_id % 2
	account_id = ((offset_id - account_type) // 2) + account_type
	return int(str((account_id * 2) - account_type))

sqlite_tables = [
				 "bet_ledger",
				 "charity",
				 "balance_ledger",
				 #"live_lobbies",
				 "match_status",
				 #"lobby_message",
				 #"announce_message",
				 "live_matches",
				 #"friends",
				 "discord_ids",
				 "live_players",
				 #"rp_heroes",
				 "match_details",
				 "player_match_details",
				 "ability_details",
				 "pick_details",
				 "bear_details"
				 ]

for t in sqlite_tables:
	columns,rows = db.select_table(t)
	table_df = pd.DataFrame(rows,columns=columns)
	if t == 'discord_ids':
		table_df['account_id'] = table_df['steam_id'].apply(convert_steam_to_account)
	table_df.to_sql(t,sqlite_connection,if_exists='fail',index=False)
	
sqlite_connection.close()
	
