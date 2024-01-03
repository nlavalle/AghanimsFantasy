import datetime, time

import sqlalchemy as db
from sqlalchemy import exc, text, desc
from sqlalchemy.orm import sessionmaker
import pandas as pd
import numpy as np

import logging
from enum import Enum, auto, IntEnum

NEW_PLAYER_STIPEND = 1000
AUTOBET = 250
HOUSE_BET = 500
WAIT_REPLAY = 1200

SALT_MINE = 50

class MATCH_STATUS(IntEnum):
	UNRESOLVED = auto()
	RADIANT = auto()
	DIRE = auto()
	ERROR = auto()

class LP_STATUS(Enum):
	NOT_FOUND = auto()	
	INIT = auto()	
	VALID = auto()	
	INVALID = auto()	

class PGDB:

	def __init__(self,conn_string,app_name):
		self.engine = db.create_engine(conn_string,isolation_level="AUTOCOMMIT",connect_args={"application_name":app_name})
		self.conn = self.engine.connect()
		self.metadata = db.MetaData(schema='Kali')

		self.extended_match_details_request = db.Table('extended_match_details_request', self.metadata,
			db.Column('match_id',db.BigInteger,nullable=False,primary_key=True),
			db.Column('request_time',db.Integer,nullable=False),
			db.Column('match_details_retrieved',db.Boolean,nullable=False))

		self.bet_ledger = db.Table('bet_ledger', self.metadata,
			db.Column('wager_id',db.Integer,primary_key=True),
			db.Column('match_id',db.BigInteger,nullable=False),
			db.Column('gambler_id',db.BigInteger,nullable=False),
			db.Column('side',db.Integer,nullable=False),
			db.Column('amount',db.BigInteger,nullable=False),
			db.Column('finalized',db.Boolean,nullable=False))

		self.charity = db.Table('charity', self.metadata,
			db.Column('charity_id',db.Integer,primary_key=True),
			db.Column('wager_id',db.Integer,nullable=True),
			db.Column('gambler_id',db.BigInteger,nullable=False),
			db.Column('amount',db.BigInteger,nullable=False),
			db.Column('reason',db.String,nullable=False),
			db.Column('query_time',db.BigInteger,nullable=False))

		self.balance_ledger = db.Table('balance_ledger',self.metadata,
			db.Column('discord_id',db.BigInteger,primary_key=True,nullable=False),
			db.Column('tokens',db.BigInteger,nullable=False))

		self.live_lobbies = db.Table('live_lobbies', self.metadata,
			db.Column('lobby_id',db.BigInteger,nullable=False))

		self.match_status = db.Table('match_status', self.metadata,
			db.Column('match_id',db.BigInteger,nullable=False,primary_key=True),
			db.Column('status',db.Integer,nullable=False))

		self.lobby_message = db.Table('lobby_message', self.metadata,
			db.Column('lobby_id',db.BigInteger,nullable=False,primary_key=True),
			db.Column('message_id',db.BigInteger,nullable=False))

		self.announce_message = db.Table('announce_message', self.metadata,
			db.Column('lobby_id',db.BigInteger,nullable=False,primary_key=True),
			db.Column('message_id',db.BigInteger,nullable=False))

		self.live_matches = db.Table('live_matches', self.metadata,
			db.Column('query_time',db.BigInteger,nullable=False),
			db.Column('activate_time',db.BigInteger,nullable=False),
			db.Column('deactivate_time',db.BigInteger,nullable=False),
			db.Column('server_steam_id',db.BigInteger,nullable=False),
			db.Column('lobby_id',db.BigInteger,nullable=False),
			db.Column('league_id',db.Integer,nullable=False),
			db.Column('lobby_type',db.Integer,nullable=False),
			db.Column('game_time',db.Integer,nullable=False),
			db.Column('delay',db.Integer,nullable=False),
			db.Column('spectators',db.Integer,nullable=False),
			db.Column('game_mode',db.Integer,nullable=False),
			db.Column('average_mmr',db.Integer,nullable=False),
			db.Column('match_id',db.BigInteger,nullable=False),
			db.Column('series_id',db.Integer,nullable=False),
			db.Column('sort_score',db.Integer,nullable=False),
			db.Column('last_update_time',db.Float,nullable=False),
			db.Column('radiant_lead',db.Integer,nullable=False),
			db.Column('radiant_score',db.Integer,nullable=False),
			db.Column('dire_score',db.Integer,nullable=False),
			db.Column('building_state',db.Integer,nullable=False),
			db.Column('custom_game_difficulty',db.Integer,nullable=True))

		self.friends = db.Table('friends', self.metadata,
			db.Column('steam_id',db.BigInteger,nullable=False))

		self.discord_ids = db.Table('discord_ids', self.metadata,
			db.Column('discord_id',db.BigInteger,nullable=False),
			db.Column('steam_id',db.BigInteger,nullable=False),
			db.Column('account_id',db.BigInteger,nullable=False),
			db.Column('discord_name',db.VARCHAR,nullable=False))

		self.live_players = db.Table('live_players', self.metadata,
			db.Column('match_id',db.BigInteger,nullable=False),
			db.Column('player_num',db.Integer,nullable=False),
			db.Column('account_id',db.BigInteger,nullable=False),
			db.Column('hero_id',db.Integer,nullable=False))

		self.rp_heroes = db.Table('rp_heroes', self.metadata,
			db.Column('lobby_id',db.BigInteger,nullable=False,primary_key=True),
			db.Column('steam_id',db.BigInteger,nullable=False,primary_key=True),
			db.Column('param0',db.String,nullable=True,primary_key=True),
			db.Column('param1',db.String,nullable=True,primary_key=True),
			db.Column('param2',db.String,nullable=True))

		self.match_details = db.Table('match_details', self.metadata,
			db.Column('match_id',db.BigInteger,primary_key=True,nullable=False),
			db.Column('duration',db.Integer,nullable=False),
			db.Column('pre_game_duration',db.Integer,nullable=False),
			db.Column('start_time',db.BigInteger,nullable=False),
			db.Column('match_seq_num',db.BigInteger,nullable=False),
			db.Column('tower_status_radiant',db.Integer,nullable=False),
			db.Column('tower_status_dire',db.Integer,nullable=False),
			db.Column('barracks_status_radiant',db.Integer,nullable=False),
			db.Column('barracks_status_dire',db.Integer,nullable=False),
			db.Column('cluster',db.Integer,nullable=False),
			db.Column('first_blood_time',db.Integer,nullable=False),
			db.Column('lobby_type',db.Integer,nullable=False),
			db.Column('human_players',db.Integer,nullable=False),
			db.Column('leagueid',db.Integer,nullable=False),
			db.Column('positive_votes',db.Integer,nullable=False),
			db.Column('negative_votes',db.Integer,nullable=False),
			db.Column('game_mode',db.Integer,nullable=False),
			db.Column('flags',db.Integer,nullable=False),
			db.Column('engine',db.Integer,nullable=False),
			db.Column('radiant_score',db.Integer,nullable=False),
			db.Column('dire_score',db.Integer,nullable=False),
			db.Column('radiant_win',db.Boolean,nullable=True))

		self.player_match_details = db.Table('player_match_details', self.metadata,
			db.Column('match_id',db.BigInteger,primary_key=True,nullable=False),
			db.Column('account_id',db.BigInteger,nullable=False),
			db.Column('player_slot',db.Integer,primary_key=True,nullable=False),
			db.Column('hero_id',db.Integer,nullable=False),
			db.Column('item_0',db.Integer,nullable=False),
			db.Column('item_1',db.Integer,nullable=False),
			db.Column('item_2',db.Integer,nullable=False),
			db.Column('item_3',db.Integer,nullable=False),
			db.Column('item_4',db.Integer,nullable=False),
			db.Column('item_5',db.Integer,nullable=False),
			db.Column('backpack_0',db.Integer,nullable=False),
			db.Column('backpack_1',db.Integer,nullable=False),
			db.Column('backpack_2',db.Integer,nullable=False),
			db.Column('item_neutral',db.Integer,nullable=False),
			db.Column('kills',db.Integer,nullable=False),
			db.Column('deaths',db.Integer,nullable=False),
			db.Column('assists',db.Integer,nullable=False),
			db.Column('leaver_status',db.Integer,nullable=False),
			db.Column('last_hits',db.Integer,nullable=False),
			db.Column('denies',db.Integer,nullable=False),
			db.Column('gold_per_min',db.Integer,nullable=False),
			db.Column('xp_per_min',db.Integer,nullable=False),
			db.Column('level',db.Integer,nullable=False),
			db.Column('hero_damage',db.Integer,nullable=False),
			db.Column('tower_damage',db.Integer,nullable=False),
			db.Column('hero_healing',db.Integer,nullable=False),
			db.Column('gold',db.Integer,nullable=False),
			db.Column('gold_spent',db.Integer,nullable=False),
			db.Column('scaled_hero_damage',db.Integer,nullable=False),
			db.Column('scaled_tower_damage',db.Integer,nullable=False),
			db.Column('scaled_hero_healing',db.Integer,nullable=False),
			db.Column('aghanims_scepter',db.Integer,nullable=True),
			db.Column('aghanims_shard',db.Integer,nullable=True),
			db.Column('moonshard',db.Integer,nullable=True),
			db.Column('net_worth',db.BigInteger,nullable=True),
			db.Column('team_number',db.Integer,nullable=True),
			db.Column('team_slot',db.Integer,nullable=True))

		self.ability_details = db.Table('ability_details', self.metadata,
			db.Column('match_id',db.BigInteger,nullable=False),
			db.Column('player_slot',db.Integer,nullable=False),
			db.Column('ability',db.Integer,nullable=False),
			db.Column('time',db.Integer,nullable=False),
			db.Column('level',db.Integer,nullable=False))

		self.pick_details = db.Table('pick_details', self.metadata,
			db.Column('match_id',db.BigInteger,primary_key=True,nullable=False),
			db.Column('is_pick',db.Boolean,nullable=False),
			db.Column('hero_id',db.Integer,nullable=False),
			db.Column('team',db.Integer,nullable=False),
			db.Column('order',db.Integer,primary_key=True,nullable=False))

		self.bear_details = db.Table('bear_details', self.metadata,
			db.Column('match_id',db.BigInteger,primary_key=True,nullable=False),
			db.Column('player_slot',db.Integer,primary_key=True,nullable=False),
			db.Column('unitname',db.String,nullable=False),
			db.Column('item_0',db.Integer,nullable=False),
			db.Column('item_1',db.Integer,nullable=False),
			db.Column('item_2',db.Integer,nullable=False),
			db.Column('item_3',db.Integer,nullable=False),
			db.Column('item_4',db.Integer,nullable=False),
			db.Column('item_5',db.Integer,nullable=False),
			db.Column('backpack_0',db.Integer,nullable=False),
			db.Column('backpack_1',db.Integer,nullable=False),
			db.Column('backpack_2',db.Integer,nullable=False),
			db.Column('item_neutral',db.Integer,nullable=False))
			

		self.metadata.create_all(self.conn)

	def request_extended_match_details(self,match_id):
		request_time = int(time.mktime(datetime.datetime.now().timetuple()))
		emdr = self.extended_match_details_request
		q = db.select([emdr.c.match_id]).where(emdr.c.match_id == match_id)
		result = self.conn.execute(q).fetchall()
		if len(result) == 0:
			insert = emdr.insert().values(match_id=match_id,request_time=request_time,match_details_retrieved=False)
			self.conn.execute(insert)

	def update_extended_match_details_requests(self,match_id):
		emdr = self.extended_match_details_request
		update = emdr.update().values(match_details_retrieved = True).where(emdr.c.match_id == match_id)
		self.conn.execute(update)
	
	def get_extended_match_details_requests(self):
		current_time = int(time.mktime(datetime.datetime.now().timetuple()))
		emdr = self.extended_match_details_request
		q = db.select([emdr.c.match_id]).where(db.and_(emdr.c.request_time+WAIT_REPLAY < current_time, 
											~emdr.c.match_details_retrieved))
		result = self.conn.execute(q).fetchall()
		return result
	
	def insert_dfs(self,df_dict):
		trans = self.conn.begin()
		for key,df in df_dict.items():
			try:	
				df.to_sql(key,self.conn,schema='Kali',if_exists='append',index=False)
			except Exception as e:
				trans.rollback()
				raise e
		trans.commit()

	def insert_df(self, table_name, df):
		df.to_sql(table_name,self.conn,schema='Kali',if_exists='append',index=False)
		
	def select_table(self, table_name):
		results = self.conn.execute('SELECT * FROM "Kali".{}'.format(table_name))
		keys = results.keys()
		results = results.fetchall()		
		return keys,results

	def select_discord_ids(self):
		di = self.discord_ids
		q = db.select([di.c.discord_id,di.c.steam_id,di.c.account_id])
		result = self.conn.execute(q).fetchall()
		return result
		
	def insert_discord_id(self,discord_id,steam_id,account_id,username):
		di = self.discord_ids
		q = db.select([di.c.discord_id]).where(di.c.discord_id == discord_id)
		result = self.conn.execute(q).fetchall()
		if len(result) == 0:
			insert = di.insert().values(discord_id=discord_id,steam_id=steam_id,account_id=account_id,discord_name=username)
			return self.conn.execute(insert)
		elif len(result) == 1:
			update = di.update().values(steam_id = steam_id,account_id=account_id,discord_name=username).where(di.c.discord_id == discord_id)
			return self.conn.execute(update)
		elif len(result) > 1:
			print('Insert discord_id/steam_id/account_id failed for {} / {} / {}, multiple results returned'.format(discord_id,steam_id,account_id))
			return None

	def check_match_status(self,match_id):
		ms = self.match_status
		q = db.select([ms.c.status]).where(ms.c.match_id == match_id)
		result = self.conn.execute(q).fetchall()
		return result

	def get_unresolved_matches(self):
		q = '''SELECT DISTINCT lm.lobby_id
FROM "Kali".match_status AS ms
LEFT OUTER JOIN "Kali".live_matches AS lm
ON lm.match_id = ms.match_id
WHERE ms.status = 1'''
		result = self.conn.execute(q).fetchall()
		return result

	def replace_friends(self,friend_ids):
		begin_statement = '''BEGIN WORK;
LOCK TABLE "Kali".friends IN ACCESS EXCLUSIVE MODE;'''
		delete = str(self.friends.delete())+';'
		end_statement = text('COMMIT WORK;')
		friend_template = text('INSERT INTO "Kali".friends VALUES (:steam_id)')
		friend_statements = []
		for f in friend_ids:
			friend_statements.append(str(friend_template.bindparams(steam_id = f).compile(compile_kwargs={"literal_binds": True}))+';')
		friend_statement = '\n'.join(friend_statements)
		lock_statement = '''{}
{}
{}
{}'''.format(begin_statement,delete,friend_statement,end_statement)
		print(lock_statement)
		result = self.conn.execute(lock_statement)
		print('result rowcount:',result.rowcount)

	def select_friends(self):
		f = self.friends
		q = db.select([f])
		result = self.conn.execute(q).fetchall()
		return result
		
	def replace_live(self,lobbies):
		begin_statement = '''BEGIN WORK;
	LOCK TABLE "Kali".live_lobbies IN ACCESS EXCLUSIVE MODE;'''
		delete = str(self.live_lobbies.delete())+';'
		end_statement = text('COMMIT WORK;')
		lobby_template = text('INSERT INTO "Kali".live_lobbies VALUES (:lobby_id)')
		lobby_statements = []
		for l in lobbies:
			lobby_statements.append(str(lobby_template.bindparams(lobby_id = l).compile(compile_kwargs={"literal_binds": True}))+';')
		lobby_statement = '\n'.join(lobby_statements)
		lock_statement = '''{}
{}
{}
{}'''.format(begin_statement,delete,lobby_statement,end_statement)
		print(lock_statement)
		result = self.conn.execute(lock_statement)
		print('result rowcount:',result.rowcount)

	def get_live(self):
		ll = self.live_lobbies
		q = db.select([ll])
		result = self.conn.execute(q).fetchall()
		return result

	def insert_announce_message(self,lobby_id,message_id):
		lm = self.announce_message
		insert = lm.insert().values(lobby_id=lobby_id,message_id=message_id)
		result = self.conn.execute(insert)

	def select_announce_message(self,lobby_id):
		lm = self.announce_message
		s = db.select([lm.c.message_id]).where(lm.c.lobby_id == lobby_id)
		return self.conn.execute(s).fetchall()

	def insert_match_message(self,lobby_id,message_id):
		lm = self.lobby_message
		insert = lm.insert().values(lobby_id=lobby_id,message_id=message_id)
		result = self.conn.execute(insert)

	def select_match_message(self,lobby_id):
		lm = self.lobby_message
		s = db.select([lm.c.message_id]).where(lm.c.lobby_id == lobby_id)
		return self.conn.execute(s).fetchall()

	def select_lm(self,lobby_id,last_update_time=None,query_only=False):
		lm = self.live_matches
		if last_update_time == None:
			s = db.select([lm]).where(lm.c.lobby_id == lobby_id)
		else:
			s = db.select([lm]).where(db.and_(lm.c.lobby_id == lobby_id,
											  lm.c.last_update_time >= last_update_time))
		if query_only:
			return s
		else:
			results = self.conn.execute(s)
			keys = results.keys()
			results = results.fetchall()
			if len(results) == 0:
				print('select_lm returned 0 results (expected race when live_lobbies updates before live_matches)')
				print('lobby_id: {}'.format(lobby_id))
				print('last_update_time: {}'.format(last_update_time))
				print('query: {}'.format(str(s)))
			return results,keys

	def insert_lm(self,row):
		lm = self.live_matches
		insert = lm.insert().values(**row)
		result = self.conn.execute(insert)
	
	def insert_lp(self,players):
		lp = self.live_players
		insert = lp.insert().values(players)
		result = self.conn.execute(insert)
		if any(map(lambda x: x == 0,map(lambda x: x['hero_id'],players))):
			return LP_STATUS.INIT
		else:
			return LP_STATUS.VALID

	def update_live_players(self,players):
		lp = self.live_players
		begin_statement = '''BEGIN WORK;
	LOCK TABLE "Kali".live_players IN ACCESS EXCLUSIVE MODE;'''
		end_statement = text('COMMIT WORK;')
		lp_template = text('UPDATE "Kali".live_players SET hero_id = (:hero_id) WHERE match_id = (:match_id) AND player_num = (:player_num) AND account_id = (:account_id);')
		update_statements = []
		for p in players:
			update_statements.append(str(lp_template.bindparams(hero_id = p['hero_id'], match_id = p['match_id'], player_num = p['player_num'], account_id=p['account_id']).compile(compile_kwargs={"literal_binds": True}))+';')

		updates = '\n'.join(update_statements)
		lock_statement = '''{}
{}
{}'''.format(begin_statement,updates,end_statement)
		result = self.conn.execute(lock_statement)

	def check_lp(self,match_id):
		lp = self.live_players
		s = db.select([lp.c.hero_id]).where(lp.c.match_id == match_id)
		logging.info('executing: {}'.format(s))
		if (result := self.conn.execute(s).fetchall()):
			if len(result) != 10:
				logging.warning('Unexpected number of players in match_id {}'.format(match_id))
				return LP_STATUS.INVALID
			elif any(map(lambda x: x == 0,map(lambda x: x['hero_id'],result))):
				return LP_STATUS.INIT
			else:
				return LP_STATUS.VALID
		else:
			logging.info('no live players stored for match_id = {}'.format(match_id))
			return LP_STATUS.NOT_FOUND

	def read_lp(self,match_id):
		lp = self.live_players
		s = db.select([lp]).where(lp.c.match_id == match_id)
		results = self.conn.execute(s)
		keys = results.keys()
		results = results.fetchall()
		return results,keys

	def insert_rp(self,row):
		rp = self.rp_heroes
		s = db.select([rp]).where(db.and_(rp.c.lobby_id == row['lobby_id'],
						  rp.c.steam_id == row['steam_id']))
		logging.info('executing: {}'.format(s))
		if (result := self.conn.execute(s).fetchall()):
			logging.info('result: {}'.format(result))
			return False
		else:
			logging.info('result: {}'.format(result))
			insert = rp.insert().values(**row)
			logging.info('inserting with: {}'.format(insert))
			result = self.conn.execute(insert)
			return True

	def leaderboard(self):
		bl = self.balance_ledger
		s = db.select([bl.c.discord_id,bl.c.tokens]).where(bl.c.discord_id > 0).order_by(desc(bl.c.tokens))
		return self.conn.execute(s).fetchall()


	def lastx(self,discord_id,x=10):
		lastten_query = '''SELECT "Kali".player_match_details.*
FROM "Kali".discord_ids
LEFT OUTER JOIN "Kali".player_match_details
ON "Kali".discord_ids.account_id = "Kali".player_match_details.account_id
inner join (select match_id from "Kali"."match_status" where status = 2 or status = 3) ms
on ms.match_id = "Kali".player_match_details.match_id
WHERE "Kali".discord_ids.discord_id = :discord_id
ORDER BY match_id DESC
LIMIT :last_x;'''
		lastten_query = text(lastten_query).bindparams(discord_id = discord_id,last_x = x)
		result = self.conn.execute(lastten_query).fetchall()
		lastten_df = pd.DataFrame(result)
		del lastten_df['account_id']
		del lastten_df['player_slot']
		del lastten_df['item_0']
		del lastten_df['item_1']
		del lastten_df['item_2']
		del lastten_df['item_3']
		del lastten_df['item_4']
		del lastten_df['item_5']
		del lastten_df['backpack_0']
		del lastten_df['backpack_1']
		del lastten_df['backpack_2']
		del lastten_df['item_neutral']
		del lastten_df['leaver_status']
		del lastten_df['team_slot']
		del lastten_df['gold_per_min']
		del lastten_df['xp_per_min']
		del lastten_df['scaled_hero_damage']
		del lastten_df['scaled_tower_damage']
		del lastten_df['scaled_hero_healing']
		del lastten_df['aghanims_scepter']
		del lastten_df['aghanims_shard']
		del lastten_df['moonshard']
		del lastten_df['gold']
		del lastten_df['gold_spent']
		del lastten_df['team_number']
		del lastten_df['hero_healing']
		del lastten_df['tower_damage']
		del lastten_df['level']
		del lastten_df['last_hits']
		del lastten_df['denies']

		return lastten_df

	def feederboard(self):
		q = '''with T1 as (
select discord_id, ms.match_id, deaths, row_number() over (partition by discord_id order by ms.match_id desc) as rownum
from "Kali"."discord_ids" di
join "Kali"."player_match_details" pmd
    on di.account_id = pmd.account_id
inner join (select match_id from "Kali"."match_status" where status = 2 or status = 3) ms
    on ms.match_id = pmd.match_id
)
select discord_id, sum(deaths)
from T1     
where rownum <= 20
group by discord_id
order by 2 desc;'''
		result = self.conn.execute(q).fetchall()
		return result

	def redistribute_wealth(self,tax_rate):

		balances = self.leaderboard()
		balances_df = pd.DataFrame(balances,columns=['discord_id','tokens'],dtype=pd.Int64Dtype())

		balances_df

		if balances_df.empty:
			return "There's no users in the database"

		all_tokens = balances_df['tokens'].sum()
		print('all_tokens:',type(all_tokens),all_tokens)

		balances_df['tax_rate'] = tax_rate
		#balances_df['tax_amount'] = np.floor(balances_df['tokens']*balances_df['tax_rate']).astype(pd.Int64Dtype())
		balances_df['tax_amount'] = np.floor(balances_df['tokens']*balances_df['tax_rate']).astype(pd.Int64Dtype())
		balances_df['frac'] = balances_df['tokens']/all_tokens
		
		n_people = len(balances_df)
		all_taxes = int(balances_df['tax_amount'].sum())

		tax_per_person,remainder = divmod(all_taxes,n_people)
		all_tax_per_person = tax_per_person*n_people

		balances_df['target_tax_amount'] = balances_df['frac']*all_tax_per_person

		balances_df['split'] = tax_per_person

		balances_df['diff'] = np.floor(balances_df['split'] - balances_df['tax_amount']).astype(pd.Int64Dtype())
		total_redistributed = int(balances_df['diff'].sum())

		statement = []
		begin = '''BEGIN WORK;
LOCK TABLE "Kali".balance_ledger IN ACCESS EXCLUSIVE MODE;'''
		statement.append(begin)

		update_balance = text('UPDATE "Kali".balance_ledger SET tokens = tokens + :tokens WHERE discord_id = :discord_id')

		for discord_id, diff in balances_df[['discord_id','diff']].values:
			discord_id = int(discord_id)
			diff = int(diff)

			update_wager_sql = str(update_balance.bindparams(tokens = diff,
						discord_id = discord_id).compile(compile_kwargs={"literal_binds": True}))+';'
			statement.append(update_wager_sql)

		end = 'COMMIT WORK;'
		statement.append(end)


		statement = '\n'.join(statement)
		self.conn.execute(statement)

		return 'Rejoice, My Comrades! {} golden salt has been redistributed.'.format(all_tax_per_person)


	def check_user_exists(self,discord_id):
		s = db.select([self.balance_ledger.c.discord_id]).where(self.balance_ledger.c.discord_id == discord_id)
		if (result := self.conn.execute(s).fetchone()):
			return True
		return False

	def check_balance(self,discord_id):
		s = db.select([self.balance_ledger.c.tokens]).where(self.balance_ledger.c.discord_id == discord_id)
		if (result := self.conn.execute(s).fetchone()):
			if result[0] < SALT_MINE:
				active_bet_df = self.return_active_bets(discord_id)
				if active_bet_df is None:
					update = self.balance_ledger.update().values(tokens = SALT_MINE).where(self.balance_ledger.c.discord_id == discord_id)
					result = self.conn.execute(update)
					return SALT_MINE
				else:
					return result[0]
			else:
				return result[0]
		else:
			insert = self.balance_ledger.insert().values(discord_id=discord_id,tokens=NEW_PLAYER_STIPEND)
			result = self.conn.execute(insert)
			return NEW_PLAYER_STIPEND

	def return_active_bets(self,discord_id=None,match_id=None):
		bets = self.bet_ledger
		if discord_id and match_id:
			s = db.select([bets]).where(db.and_(bets.c.gambler_id == discord_id,
							bets.c.match_id == match_id,
							bets.c.finalized == False))
		elif discord_id:
			s = db.select([bets]).where(db.and_(bets.c.gambler_id == discord_id,
							bets.c.finalized == False))

		elif match_id:
			s = db.select([bets]).where(db.and_(bets.c.match_id == match_id,
							bets.c.finalized == False))
		else:
			s = db.select([bets]).where(bets.c.finalized == False)

		result = self.conn.execute(s)
		keys = result.keys()
		if (result := result.fetchall()):
			active_bets_df = pd.DataFrame(result,columns=keys)
			return active_bets_df
		else:
			return None

	def update_match_status(self,match_id,status):
		if status == int(MATCH_STATUS.UNRESOLVED):
			raise ValueError('cannot finalize unresolved match id {}'.format(match_id))

		query_time = int(time.mktime(datetime.datetime.now().timetuple()))
		statement = []

		begin = '''BEGIN WORK;
LOCK TABLE "Kali".bet_ledger IN ACCESS EXCLUSIVE MODE;
LOCK TABLE "Kali".charity IN ACCESS EXCLUSIVE MODE;
LOCK TABLE "Kali".balance_ledger IN ACCESS EXCLUSIVE MODE;
LOCK TABLE "Kali".match_status IN ACCESS EXCLUSIVE MODE;'''
		statement.append(begin)

		join_query = '''SELECT bl.gambler_id, bl.wager_id, bl.amount, bl.side FROM "Kali".bet_ledger bl
INNER JOIN "Kali".charity ch
ON bl.wager_id = ch.wager_id
WHERE bl.match_id = {}
AND bl.finalized = false'''.format(match_id)

		update_wager = text('UPDATE "Kali".bet_ledger SET finalized = true WHERE wager_id = :wager_id')

		if (result := self.conn.execute(join_query).fetchall()):
			rows = []
			for row in result:
				update_wager_sql = str(update_wager.bindparams(wager_id = row.wager_id)
										.compile(compile_kwargs={"literal_binds": True}))+';'
				statement.append(update_wager_sql)
				rows.append(row)
			charity_bets_df = pd.DataFrame(rows,columns=['gambler_id','wager_id','amount','side'])

		bets_query = '''SELECT bl.gambler_id, bl.wager_id, bl.amount, bl.side FROM "Kali".bet_ledger AS bl
WHERE bl.match_id = {}
AND bl.finalized = false
AND NOT EXISTS (SELECT FROM "Kali".charity AS ch WHERE bl.wager_id = ch.wager_id)'''.format(match_id)

		if (result := self.conn.execute(bets_query,match_id).fetchall()):
			rows = []
			for row in result:
				update_wager_sql = str(update_wager.bindparams(wager_id = row.wager_id)
										.compile(compile_kwargs={"literal_binds": True}))+';'
				statement.append(update_wager_sql)
				rows.append(row)
			bets_df = pd.DataFrame(rows,columns=['gambler_id','wager_id','amount','side'])
		else:
			bets_df = None

		total_losses = 0
		player_loss_payouts = []

		# seperate 'free' bets from user-placed bets so that free bets won't be refuned on cancelled match
		if status == int(MATCH_STATUS.ERROR):
			if bets_df is not None:
				for (gambler_id,amount) in bets_df[['gambler_id','amount']].values:
					update_balance = text('UPDATE "Kali".balance_ledger SET tokens = tokens + :tokens WHERE discord_id = :discord_id')
					update_wager_sql = str(update_balance.bindparams(tokens = int(amount),
												discord_id = int(gambler_id)).compile(compile_kwargs={"literal_binds": True}))+';'
					statement.append(update_wager_sql)
		else:
			loser_player_gamblers = 0

			if charity_bets_df is not None:
				for (gambler_id,side) in charity_bets_df[['gambler_id','side']].values:
					if status != side: 
						loser_player_gamblers += 1

			if bets_df is not None:
				for (gambler_id,amount,side) in bets_df[['gambler_id','amount','side']].values:
					if status == side: 
						winning_amount = int(amount)*2
						update_balance = text('UPDATE "Kali".balance_ledger SET tokens = tokens + :tokens WHERE discord_id = :discord_id')
						update_balance_sql = str(update_balance.bindparams(tokens = winning_amount,
													discord_id = int(gambler_id)).compile(compile_kwargs={"literal_binds": True}))+';'
						statement.append(update_balance_sql)

						bonus_amount = int(0.1*loser_player_gamblers*amount)
						if bonus_amount > 0:
							update_balance = text('UPDATE "Kali".balance_ledger SET tokens = tokens + :tokens WHERE discord_id = :discord_id')
							update_balance_sql = str(update_balance.bindparams(tokens = bonus_amount,
														discord_id = int(gambler_id)).compile(compile_kwargs={"literal_binds": True}))+';'
							statement.append(update_balance_sql)

							player_loss_payouts.append((gambler_id,bonus_amount))
					else:
						total_losses += int(amount)

			if charity_bets_df is not None:
				for (gambler_id,amount,side) in charity_bets_df[['gambler_id','amount','side']].values:
					if status == side: 
						amount = int(amount)
						update_balance = text('UPDATE "Kali".balance_ledger SET tokens = tokens + :tokens WHERE discord_id = :discord_id')
						update_balance_sql = str(update_balance.bindparams(tokens = amount, discord_id = int(gambler_id)).compile(compile_kwargs={"literal_binds": True}))+';'
						statement.append(update_balance_sql)

						amount = int(total_losses*0.2)
						if amount > 0:
							update_balance = text('UPDATE "Kali".balance_ledger SET tokens = tokens + :tokens WHERE discord_id = :discord_id')
							update_balance_sql = str(update_balance.bindparams(tokens = amount, discord_id = int(gambler_id)).compile(compile_kwargs={"literal_binds": True}))+';'
							statement.append(update_balance_sql)
							player_loss_payouts.append((gambler_id,amount))
					
		ms_update = text('UPDATE "Kali".match_status SET status = :status WHERE match_id = :match_id')
		ms_update_sql = str(ms_update.bindparams(status = int(status), match_id = match_id).compile(compile_kwargs={"literal_binds": True}))+';'
		statement.append(ms_update_sql)

		statement.append('COMMIT WORK;')
		statement = '\n'.join(statement)
		print(statement)
		self.conn.execute(statement)

		return bets_df,charity_bets_df,player_loss_payouts

	def insert_match_status(self,match_id,match_players):
		query_time = int(time.mktime(datetime.datetime.now().timetuple()))
		
		statement = []

		begin = '''BEGIN WORK;
LOCK TABLE "Kali".bet_ledger IN ACCESS EXCLUSIVE MODE;
LOCK TABLE "Kali".match_status IN ACCESS EXCLUSIVE MODE;
LOCK TABLE "Kali".charity IN ACCESS EXCLUSIVE MODE;'''

		statement.append(begin)
	
		insert_match_status = text('INSERT INTO "Kali".match_status VALUES (:match_id, :status)')
		insert_match_status_sql = str(insert_match_status.bindparams(match_id = match_id,
					 status = int(MATCH_STATUS.UNRESOLVED)).compile(compile_kwargs={"literal_binds": True}))+';'
		
		statement.append(insert_match_status_sql)

		insert_charity = text('''INSERT INTO "Kali".charity (wager_id, gambler_id, amount, reason, query_time) 
VALUES (currval(pg_get_serial_sequence('"Kali".bet_ledger', 'wager_id')), :gambler_id, :amount, :reason, :query_time)''')
		
		for discord_id,side in match_players:
			self.check_balance(discord_id)
			insert_bet = text('''INSERT INTO "Kali".bet_ledger (match_id, gambler_id, side, amount, finalized) 
VALUES (:match_id, :gambler_id, :side, :amount, FALSE)''')
			insert_free_bet_sql = str(insert_bet.bindparams(match_id = match_id,
								gambler_id = discord_id, 
								side = int(side), 
								amount = AUTOBET).compile(compile_kwargs={"literal_binds": True}))+';'
			
		
			insert_charity_sql = str(insert_charity.bindparams(gambler_id = discord_id, 
								amount = AUTOBET,
								reason = 'free',
								query_time = query_time).compile(compile_kwargs={"literal_binds": True}))+';'
			
			statement.append(insert_free_bet_sql)
			statement.append(insert_charity_sql)
			
		end = 'COMMIT WORK;'

		statement.append(end)
		
		statement = '\n'.join(statement)
		print(statement)
		self.conn.execute(statement)

	def insert_bet_helper(self,match_id,discord_id,side,amount,new_balance):

		query_time = int(time.mktime(datetime.datetime.now().timetuple()))
		statement = []
		
		begin = '''BEGIN WORK;
LOCK TABLE "Kali".bet_ledger IN ACCESS EXCLUSIVE MODE;
LOCK TABLE "Kali".balance_ledger IN ACCESS EXCLUSIVE MODE;'''
		statement.append(begin)

		update_balance = text('UPDATE "Kali".balance_ledger SET tokens = :tokens WHERE discord_id = :discord_id')

		insert_bet = text('''INSERT INTO "Kali".bet_ledger (match_id, gambler_id, side, amount, finalized) 
VALUES (:match_id, :gambler_id, :side, :amount, FALSE)''')

		update_balance_sql = str(update_balance.bindparams(tokens = new_balance,
										discord_id = discord_id).compile(compile_kwargs={"literal_binds": True}))+';'
		
		insert_bet_sql = str(insert_bet.bindparams(match_id = match_id,
										gambler_id = discord_id, 
										side = int(side), 
										amount = amount).compile(compile_kwargs={"literal_binds": True}))+';'
		statement.append(update_balance_sql)
		statement.append(insert_bet_sql)
		end = 'COMMIT WORK;'
		statement.append(end)

		statement = '\n'.join(statement)
		print(statement)
		self.conn.execute(statement)

	def insert_bet(self,match_id,discord_id,side,amount):

		balance = self.check_balance(discord_id)
		if balance < amount:
			return 'insufficent balance.'
		new_balance = balance - amount

		bl = self.bet_ledger
		s = db.select([bl.c.side]).where(db.and_(bl.c.match_id == match_id,bl.c.gambler_id == discord_id))

		if (result := self.conn.execute(s).fetchall()):
			if all(map(lambda y: y == side,map(lambda x: x[0],result))):
				self.insert_bet_helper(match_id,discord_id,side,amount,new_balance)
				return 'bet {} CURRENCY on {} win for match id {}.'.format(amount,side.name.lower().title(),match_id)
			else:
				return 'cannot bet on both sides.'
		else:
			self.insert_bet_helper(match_id,discord_id,side,amount,new_balance)
			return 'bet {} CURRENCY on {} win for match id {}.'.format(amount,side.name.lower().title(),match_id)

	def tip(self,tipper_id,tipee_id):

		balance = self.check_balance(tipper_id)
		if balance < SALT_MINE+1:
			return False

		statement = []
		begin = '''BEGIN WORK;
LOCK TABLE "Kali".balance_ledger IN ACCESS EXCLUSIVE MODE;'''
		statement.append(begin)

		update_balance_1 = text('UPDATE "Kali".balance_ledger SET tokens = tokens - 1 WHERE discord_id = :discord_id')
		update_balance_2 = text('UPDATE "Kali".balance_ledger SET tokens = tokens + 1 WHERE discord_id = :discord_id')

		update_wager_1_sql = str(update_balance_1.bindparams(discord_id = tipper_id).compile(compile_kwargs={"literal_binds": True}))+';'
		statement.append(update_wager_1_sql)
		update_wager_2_sql = str(update_balance_2.bindparams(discord_id = tipee_id).compile(compile_kwargs={"literal_binds": True}))+';'
		statement.append(update_wager_2_sql)

		end = 'COMMIT WORK;'
		statement.append(end)
		statement = '\n'.join(statement)
		self.conn.execute(statement)
		return True
