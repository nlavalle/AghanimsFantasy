import datetime, time

import sqlalchemy as db
from sqlalchemy import exc, text
import pandas as pd

import logging
from enum import Enum, auto, IntEnum

AUTOBET = 50
HOUSE_BET = 500

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

	def __init__(self,conn_string):
		self.engine = db.create_engine(conn_string,isolation_level="AUTOCOMMIT")
		self.conn = self.engine.connect()
		self.metadata = db.MetaData(schema='Kali')

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
			db.Column('match_id',db.BigInteger,nullable=False),
			db.Column('status',db.Integer,nullable=False))

		self.lobby_message = db.Table('lobby_message', self.metadata,
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
			db.Column('building_state',db.Integer,nullable=False))

		self.friends = db.Table('friends', self.metadata,
			db.Column('steam_id',db.BigInteger,nullable=False))

		self.discord_ids = db.Table('discord_ids', self.metadata,
			db.Column('discord_id',db.BigInteger,nullable=False),
			db.Column('steam_id',db.BigInteger,nullable=False))

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

		self.metadata.create_all(self.engine)

	def select_discord_ids(self):
		di = self.discord_ids
		q = db.select([di])
		result = self.conn.execute(q).fetchall()
		return result
		
	def insert_discord_id(self,discord_id,steam_id):
		di = self.discord_ids
		q = db.select([di.c.discord_id]).where(di.c.discord_id == discord_id)
		result = self.conn.execute(q).fetchall()
		if len(result) == 0:
			insert = di.insert().values(discord_id=discord_id,steam_id=steam_id)
			return self.conn.execute(insert)
		elif len(result) == 1:
			update = di.update().values(steam_id = steam_id).where(di.c.discord_id == discord_id)
			return self.conn.execute(update)
		elif len(result) > 1:
			print('Insert discord_id/steam_id failed for {} / {}, multiple results returned'.format(discord_id,steam_id))
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

	def update_lp(self,players):
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
			logging.info('no live players found for match_id = {}'.format(match_id))
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

	def check_balance(self,discord_id):
		s = db.select([self.balance_ledger.c.tokens]).where(self.balance_ledger.c.discord_id == discord_id)
		if (result := self.conn.execute(s).fetchone()):
			return result[0]
		else:
			insert = self.balance_ledger.insert().values(discord_id=discord_id,tokens=1000)
			result = self.conn.execute(insert)
			return 1000

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
			s = db.select([bets]).where(db.and_(bets.c.gambler_id == match_id,
							bets.c.finalized == False))
		else:
			s = db.select([bets]).where(bets.c.finalized == False)

		result = self.conn.execute(s)
		keys = result.keys()
		if (result := result.fetchall()):
			active_bets_df = pd.DataFrame(result,columns=keys)
			msg = str(active_bets_df)
			return msg
		else:
			return 'there are no active bets.'

	def update_match_status(self,match_id,status):
		if status == int(MATCH_STATUS.UNRESOLVED):
			raise ValueError('cannot finalize unresolved match id {}'.format(match_id))

		statement = []

		begin = '''BEGIN WORK;
LOCK TABLE "Kali".bet_ledger IN ACCESS EXCLUSIVE MODE;
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
		else:
			print('No charity positions in wagers for match_id {}, marking match complete'.format(match_id))
			ms = self.match_status
			update = ms.update().values(status = int(status)).where(ms.c.match_id == match_id)
			result = self.conn.execute(update)
			return

		bets_query = '''SELECT bl.gambler_id, bl.wager_id, bl.amount, bl.side FROM "Kali".bet_ledger AS bl
WHERE bl.match_id = {}
AND bl.finalized = false
AND NOT EXISTS (SELECT FROM "Kali".charity AS ch WHERE bl.wager_id = ch.wager_id)'''.format(match_id)

		if (result := self.conn.execute(bets_query,match_id).fetchall()):
			rows = []
			for row in result:
				update_wager_sql = str(update_wager.bindparams(match_id = row.wager_id)
										.compile(compile_kwargs={"literal_binds": True}))+';'
				statement.append(update_wager_sql)
				rows.append(row)
			bets_df = pd.DataFrame(rows,columns=['gambler_id','wager_id','amount','side'])
		else:
			bets_df = None

		if status == int(MATCH_STATUS.ERROR):
			for (gambler_id,amount) in bets_df[['gambler_id','amount']].values:
				update_balance = text('UPDATE "Kali".balance_ledger SET tokens = tokens + :tokens WHERE discord_id = :discord_id')
				update_wager_sql = str(update_balance.bindparams(tokens = amount,
											discord_id = gambler_id).compile(compile_kwargs={"literal_binds": True}))+';'
				statement.append(update_wager_sql)
		else:
			radiant_pot = charity_bets_df.groupby(['side'])['amount'].agg('sum')[int(MATCH_STATUS.RADIANT)]
			dire_pot = charity_bets_df.groupby(['side'])['amount'].agg('sum')[int(MATCH_STATUS.DIRE)]

			if bets_df:
				radiant_pot += bets_df.groupby(['side'])['amount'].agg('sum')[int(MATCH_STATUS.RADIANT)]
				dire_pot += bets_df.groupby(['side'])['amount'].agg('sum')[int(MATCH_STATUS.DIRE)]

			total_pot = radiant_pot+dire_pot

			if bets_df:
				for (gambler_id,amount,side) in bets_df[['gambler_id','amount','side']].values:
					if status == side: 
						if status == MATCH_STATUS.RADIANT:
							new_amount = int(amount/radiant_pot*dire_pot + amount)
						elif status == MATCH_STATUS.DIRE:
							new_amount = int(amount/dire_pot*radiant_pot + amount)
						update_balance = text('UPDATE "Kali".balance_ledger SET tokens = tokens + :tokens WHERE discord_id = :discord_id')
						update_balance_sql = str(update_balance.bindparams(tokens = new_amount,
													discord_id = int(gambler_id)).compile(compile_kwargs={"literal_binds": True}))+';'
						statement.append(update_balance_sql)

			for (gambler_id,amount,side) in charity_bets_df[['gambler_id','amount','side']].values:
				if status == side: 
					if status == MATCH_STATUS.RADIANT:
						 new_amount = int(amount/radiant_pot*dire_pot)
					elif status == MATCH_STATUS.DIRE:
						 new_amount = int(amount/dire_pot*radiant_pot)
					update_balance = text('UPDATE "Kali".balance_ledger SET tokens = tokens + :tokens WHERE discord_id = :discord_id')
					update_balance_sql = str(update_balance.bindparams(tokens = new_amount, discord_id = int(gambler_id)).compile(compile_kwargs={"literal_binds": True}))+';'
					statement.append(update_balance_sql)
					
		ms_update = text('UPDATE "Kali".match_status SET status = :status WHERE match_id = :match_id')
		ms_update_sql = str(ms_update.bindparams(status = int(status), match_id = match_id).compile(compile_kwargs={"literal_binds": True}))+';'
		statement.append(ms_update_sql)

		statement.append('COMMIT WORK;')
		statement = '\n'.join(statement)
		print(statement)
		self.conn.execute(statement)

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
		
		for discord_id,side in match_players:
			self.check_balance(discord_id)
			insert_bet = text('''INSERT INTO "Kali".bet_ledger (match_id, gambler_id, side, amount, finalized) 
VALUES (:match_id, :gambler_id, :side, :amount, FALSE)''')
			insert_free_bet_sql = str(insert_bet.bindparams(match_id = match_id,
								gambler_id = discord_id, 
								side = int(side), 
								amount = AUTOBET).compile(compile_kwargs={"literal_binds": True}))+';'
			
			insert_charity = text('''INSERT INTO "Kali".charity (wager_id, gambler_id, amount, reason, query_time) 
VALUES (currval(pg_get_serial_sequence('"Kali".bet_ledger', 'wager_id')), :gambler_id, :amount, :reason, :query_time)''')
		
			insert_charity_sql = str(insert_charity.bindparams(gambler_id = discord_id, 
								amount = AUTOBET,
								reason = 'free',
								query_time = query_time).compile(compile_kwargs={"literal_binds": True}))+';'
			
			statement.append(insert_free_bet_sql)
			statement.append(insert_charity_sql)
			
		insert_house_radiant_bet_sql = str(insert_bet.bindparams(match_id = match_id,
							gambler_id = -int(MATCH_STATUS.RADIANT),
							side = int(MATCH_STATUS.RADIANT),
							amount = HOUSE_BET).compile(compile_kwargs={"literal_binds": True}))+';'

		insert_charity_radiant_sql = str(insert_charity.bindparams(gambler_id = -2, 
							amount = HOUSE_BET,
							reason = 'house bet radiant',
							query_time = query_time).compile(compile_kwargs={"literal_binds": True}))+';'

		insert_house_dire_bet_sql = str(insert_bet.bindparams(match_id = match_id,
							gambler_id = -int(MATCH_STATUS.DIRE),
							side = int(MATCH_STATUS.DIRE),
							amount = HOUSE_BET).compile(compile_kwargs={"literal_binds": True}))+';'

		insert_charity_dire_sql = str(insert_charity.bindparams(gambler_id = -3, 
							amount = HOUSE_BET,
							reason = 'house bet dire',
							query_time = query_time).compile(compile_kwargs={"literal_binds": True}))+';'
		
		end = 'COMMIT WORK;'

		statement.append(insert_house_radiant_bet_sql)
		statement.append(insert_charity_radiant_sql)
		statement.append(insert_house_dire_bet_sql)
		statement.append(insert_charity_dire_sql)
		statement.append(end)
		
		statement = '\n'.join(statement)
		print(statement)
		self.conn.execute(statement)

	def insert_bet_helper(self,match_id,discord_id,side,amount,new_balance):

		query_time = int(time.mktime(datetime.datetime.now().timetuple()))
		
		begin = '''BEGIN WORK;
LOCK TABLE "Kali".bet_ledger IN ACCESS EXCLUSIVE MODE;
LOCK TABLE "Kali".balance_ledger IN ACCESS EXCLUSIVE MODE;'''

		update_balance = text('UPDATE "Kali".balance_ledger SET tokens = :tokens WHERE discord_id = :discord_id')

		insert_bet = text('''INSERT INTO "Kali".bet_ledger (match_id, gambler_id, side, amount, finalized) 
VALUES (:match_id, :gambler_id, :side, :amount, FALSE)''')

		update_balance_sql = str(update_balance.bindparams(tokens = new_balance,
										discord_id = discord_id).compile(compile_kwargs={"literal_binds": True}))+';'
		
		insert_bet_sql = str(insert_bet.bindparams(match_id = match_id,
										gambler_id = discord_id, 
										side = int(side), 
										amount = amount).compile(compile_kwargs={"literal_binds": True}))+';'

		end = 'COMMIT WORK;'
		statement = '\n'.join(begin,update_balance_sql,insert_bet_sql,end)
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