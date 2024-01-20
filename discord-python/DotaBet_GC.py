import logging, datetime, time, json
from collections import defaultdict
from pathlib import Path

import filelock
import gevent

from steam.client import SteamClient
from steam.enums import EResult
from steam.enums.emsg import EMsg
from steam.utils.proto import proto_to_dict
from dota2.client import Dota2Client
from dota2.proto_enums import EDOTAGCMsg

from tokens import STEAM_BOT_ACCOUNT, STEAM_BOT_PASSWORD, CONNECTION_STRING
from app_db import PGDB, LP_STATUS

logging.basicConfig(format='[%(asctime)s] %(levelname)s: %(message)s', level=logging.INFO)

class DotaBet_GC:

	def __init__(self,steam_client,dota_client,db):
		self.steam_client = steam_client
		self.dota_client = dota_client
		self.db = db

		self.friends_synced = False
		self.sleep_time = 15
		self.extended_match_details_path = Path.cwd() / 'extended_match_details'
		self.extended_match_details_path.mkdir(exist_ok=True)

		# set of all active games
		self.active_source_tv_lobbies = set()

		# mapping for friend ids to "watchable" game ids
		self.friend_game_ids = {}
		
		# handles player pick states for different match_ids
		self.match_id_status = defaultdict(lambda: LP_STATUS.NOT_FOUND)

		# only check once per session to avoid pissing off the GC
		self.session_check_extended_match_details = set()

		self.last_called = None
	

		# set-up event handlers
		self._LobbyLoop = self.dota_client.on('ready')(self.LobbyLoop)
		self._UpdateClientPersonaState = self.steam_client.on(EMsg.ClientPersonaState)(self.UpdateClientPersonaState)
		self._ProcessLobbyStates = self.dota_client.on(EDOTAGCMsg.EMsgGCToClientFindTopSourceTVGamesResponse)(self.ProcessLobbyStates)
		self._StartDota = self.steam_client.on('logged_on')(self.StartDota)
		self._ProcessMatchDetailsResponse = self.dota_client.on(EDOTAGCMsg.EMsgGCMatchDetailsResponse)(self.ProcessMatchDetailsResponse)

		# the disconnect/reconnect code in the steam/dota2 packages doesn't seem to work properly.
		#self._HandleDisconnect = self.steam_client.on('disconnect')(self.HandleDisconnect)

		# rich presence status types we're interested in tracking
		#self.track_lobby_types = ['#DOTA_lobby_type_name_unranked', 
		#							'#DOTA_lobby_type_name_ranked']

	def LobbyLoop(self):
		logging.info('Starting LobbyLoop')
		last_checked_lobbies = set()

		if not self.last_called:
			self.db.replace_live(last_checked_lobbies) # clear lobbies on first loop

		self.last_called = int(time.mktime(datetime.datetime.now().timetuple()))
		last_called = self.last_called

		while last_called == self.last_called:
			# not guaranteed to have friend list 'ready' during other initialization steps
			if not self.friends_synced:
				self.SyncFriends()
			gevent.sleep(self.sleep_time)
			logging.info('Checking lobbies...')
			logging.info('active_source_tv_lobbies: {}'.format(self.active_source_tv_lobbies))
			n_lobbies = len(self.active_source_tv_lobbies)
			if n_lobbies > 0:
				if last_checked_lobbies != self.active_source_tv_lobbies:
					self.db.replace_live(self.active_source_tv_lobbies)
					last_checked_lobbies = self.active_source_tv_lobbies.copy()
				data = dict([('lobby_ids',list(self.active_source_tv_lobbies))])
				logging.info('Checking {} lobbies'.format(n_lobbies))
				self.dota_client.send(EDOTAGCMsg.EMsgClientToGCFindTopSourceTVGames,data)
			elif n_lobbies == 0 and len(last_checked_lobbies) != 0:
				last_checked_lobbies = set()
				self.db.replace_live(self.active_source_tv_lobbies)
			self.CheckExtendedMatchDetails()

	def UpdateClientPersonaState(self,msg):
		'''Fires for every status change in the friend's list. each msg contains the friend's new state.'''
	
		# header is always information about your connected bot
		#logging.info('{}'.format(proto_to_dict(msg.header))) 
		#logging.info('{}'.format(proto_to_dict(msg.body)))

		parsed_msg = proto_to_dict(msg.body)
		if 'friends' in parsed_msg.keys():
			for friend in parsed_msg['friends']:
				steam_id = friend['friendid']
				try:
					rich_presence = friend['rich_presence']
					game_id = friend['gameid']
					if game_id != 570:
						self.friend_game_ids[steam_id] = None
						continue
				except KeyError:
					self.friend_game_ids[steam_id] = None
					continue
					
				logging.info('friend in dota: {}'.format(proto_to_dict(msg.body)))

				status = None
				param0 = None
				watchable_game_id = None
				
				for d in rich_presence:
					try:
						if d['key'] == 'status':
							status = d['value']
					except KeyError:
						pass
					try:
						if d['key'] == 'param0':
							param0 = d['value']
					except KeyError:
						pass
					try:
						if d['key'] == 'WatchableGameID':
							watchable_game_id = int(d['value'])
					except KeyError:
						pass
					
				if status is None or param0 is None or watchable_game_id is None:
					self.friend_game_ids[steam_id] = None
					continue

				if param0 == '#DOTA_lobby_type_name_lobby':
					self.friend_game_ids[steam_id] = None
					continue
				if param0 == '#game_mode_18':
					self.friend_game_ids[steam_id] = None
					continue
				if param0 == '#game_mode_23':
					self.friend_game_ids[steam_id] = None
					continue
				if param0 == '#game_mode_23':
					self.friend_game_ids[steam_id] = None
					continue
				if param0 == '#game_mode_lobby_name_20':
					self.friend_game_ids[steam_id] = None
					continue
				if param0 == '#game_mode_lobby_name_7':
					self.friend_game_ids[steam_id] = None
					continue
				if param0 == '#game_mode_lobby_name_8':
					self.friend_game_ids[steam_id] = None
					continue
				if param0 == '#game_mode_lobby_name_9':
					self.friend_game_ids[steam_id] = None
					continue
				if param0 == '#game_mode_lobby_name_11':
					self.friend_game_ids[steam_id] = None
					continue
					

				if status in ['#DOTA_RP_PLAYING_AS','#DOTA_RP_HERO_SELECTION','#DOTA_RP_STRATEGY_TIME'] and watchable_game_id != 0:
					self.friend_game_ids[steam_id] = watchable_game_id
				else:
					self.friend_game_ids[steam_id] = None

		self.active_source_tv_lobbies = set(filter(lambda y: y is not None,map(lambda x: x[1],self.friend_game_ids.items())))

	def ProcessLobbyStates(self,msg):
		logging.info('Starting process_lobby_states')
		query_time = int(time.mktime(datetime.datetime.now().timetuple()))
		if msg.specific_games:	
			logging.info('Specific lobbies returned')
			for game_proto in msg.game_list:
				game = proto_to_dict(game_proto)
				logging.info('match_id: {}'.format(game['match_id']))
				logging.info('game_time: {}'.format(game['game_time']))
				match_id = game['match_id']
				game['query_time'] = query_time
				players = game['players']
				del game['players']
				self.db.insert_lm(game)
				# this looks janky but it works. tbh i don't remember why. 
				if self.match_id_status[match_id] == LP_STATUS.NOT_FOUND:
					self.match_id_status[match_id] = self.db.check_lp(match_id)
				if self.match_id_status[match_id] == LP_STATUS.NOT_FOUND:
					for p,i in zip(players,range(len(players))):
						p['match_id'] = match_id
						p['player_num'] = i
					self.match_id_status[match_id] = self.db.insert_lp(players)
				elif self.match_id_status[match_id] == LP_STATUS.INIT:
					if not any(map(lambda x: x == 0,map(lambda x: x['hero_id'],players))):
						for p,i in zip(players,range(len(players))):
							p['match_id'] = match_id
							p['player_num'] = i
						self.match_id_status[match_id] = self.db.update_live_players(players)

	def SyncFriends(self):
		'''this is synced into the database for use by the discord bot'''
		if self.steam_client.friends.ready:
			friend_ids = list(map(lambda x: int(x.steam_id),self.steam_client.friends))
			self.db.replace_friends(friend_ids)
			self.friends_synced = True

	def StartDota(self):
		logging.info('Starting Dota GC communication')
		self.dota_client.launch()

	def ProcessMatchDetailsResponse(self,msg):
		game = proto_to_dict(msg)
		match_id = game['match']['match_id']
		
		lock_name = self.extended_match_details_path / '{}_extended.json.lock'.format(match_id)
		file_name = self.extended_match_details_path / '{}_extended.json'.format(match_id)

		with filelock.FileLock(lock_name):
			with open(file_name, 'w') as f_out:
				json.dump(game, f_out)
				self.db.update_extended_match_details_requests(match_id)

	def CheckExtendedMatchDetails(self):
		results = self.db.get_extended_match_details_requests()
		for match_id in results:
			match_id = match_id[0]
			logging.info('checking for extended details on {}'.format(match_id))
			file_name = self.extended_match_details_path / '{}_extended.json'.format(match_id)
			if not file_name.exists():
				if match_id not in self.session_check_extended_match_details:
					self.dota_client.request_match_details(match_id)
					# only check once per session to avoid pissing off the GC
					self.session_check_extended_match_details.add(match_id)
					gevent.sleep(1)
			else:
				# this should probably be logged with greater visibility/persistence
				msg = '{} already exists'.format(file_name)
				logging.warning(msg)

if __name__ == "__main__":
	pgdb = PGDB(CONNECTION_STRING,'DotaBet_GC')
	steam_client = SteamClient()
	dota_client = Dota2Client(steam_client)
	dotabet_gc = DotaBet_GC(steam_client,dota_client,pgdb)
	steam_login_result = steam_client.cli_login(username=STEAM_BOT_ACCOUNT,password=STEAM_BOT_PASSWORD)
	if steam_login_result != EResult.OK:
		logging.error('Could not log in')	
		raise SystemExit
	steam_client.run_forever()

