
import asyncio, json, datetime, re, time
from pathlib import Path

from filelock import Timeout, FileLock
import requests
import discord
import discord.utils
from discord.ext import commands

import pandas as pd
import sqlalchemy as db

from tokens import TOKEN, CONNECTION_STRING
from app_db import PGDB, MATCH_STATUS
from DotaWebAPI import schuck_match_details, perform_import

SUPERUSER_ID = 148293973331542017

# things to do: add a auto balance check for discord ids -2 and -3

GAMBLING_CLOSE_WAIT = 120
RESTART_LAST_CHANCE = 250

LONGEST_HERO_NAME = None

CURRENCY = 'golden salt'
INFO_CHANNEL = 'dota-bet-info'
COMM_CHANNEL = 'dota-bet'

pgdb = PGDB(CONNECTION_STRING,'DotaBet_discord')


with open('heroes.json') as f:
	hero_data = json.load(f)
	hero_data = dict(map(lambda x: (x['id'],(x['name'],x['localized_name'])),hero_data['heroes']))
	hero_data[0] = ('null','null')
	LONGEST_HERO_NAME = max(map(len,map(lambda x: x[1],hero_data.values())))
	LOCALIZED_STATUS = dict([(MATCH_STATUS.UNRESOLVED,'Match In Progress'),
							(MATCH_STATUS.RADIANT,'Radiant'),
							(MATCH_STATUS.DIRE,'Dire'),
							(MATCH_STATUS.ERROR,'Error parsing match result')])

def convert_steam_to_account(steam_id):
	id_str = str(steam_id)
	id64_base = 76561197960265728
	offset_id = int(id_str) - id64_base
	account_type = offset_id % 2
	account_id = ((offset_id - account_type) // 2) + account_type
	return int(str((account_id * 2) - account_type))

class MatchMsg:

	players_only_msg_template = '''```New match id: {match_id}
{players_only_msg}

{gambling_status}```'''

	mobile_friendly_msg = '```!bet {match_id} radiant/dire amount```'

	players_msg_template = '''========= Radiant =========
---------------------------
--\t{0}
--\t{1}
--\t{2}
--\t{3}
--\t{4}
---------------------------
========= Dire ============
---------------------------
--\t{5}
--\t{6}
--\t{7}
--\t{8}
--\t{9}
---------------------------\n'''

	match_msg_template_1 = '```Match ID: {match_id}\n'
	match_msg_template_2 = '''Game Time: {game_time}
Average MMR: {average_mmr}
Radiant Lead: {radiant_lead}
Radiant Score: {radiant_score}
Dire Score: {dire_score}
Building State: {building_state}'''

	def __init__(self,info_channel,comm_channel,lobby_id):
		self.info_channel = info_channel
		self.comm_channel = comm_channel
		self.lobby_id = lobby_id

		self.match_msg = None
		self.match_msg_obj = None
		self.match_msg_id = None

		self.announce_msg = None
		self.announce_msg_obj = None
		self.announce_msg_id = None

		self.winner = MATCH_STATUS.UNRESOLVED
		self.gambling_close = None
		self.gambling_initialized = False

	async def init_msg_objs(self):
		if self.match_msg_obj == None:	
			if (match_msg_id := pgdb.select_match_message(self.lobby_id)):
				print('using match_msg_id {} for lobby {}'.format(match_msg_id[0][0],self.lobby_id))
				self.match_msg_id = match_msg_id[0][0]
				self.match_msg_obj = await self.info_channel.fetch_message(self.match_msg_id)
				self.match_msg = self.match_msg_obj.content
		if self.announce_msg_obj == None:
			if (announce_msg_id := pgdb.select_announce_message(self.lobby_id)):
				self.announce_msg_id = announce_msg_id[0][0]
				self.announce_msg_obj = await self.comm_channel.fetch_message(self.announce_msg_id)
				self.announce_msg = self.announce_msg_obj.content

	async def update(self, match_details, players_msgs, players_only_msgs, pick_phase):
		match_msg_1 = MatchMsg.match_msg_template_1.format(**match_details)
		match_msg_2 = MatchMsg.match_msg_template_2.format(**match_details)
		players_msg = MatchMsg.players_msg_template.format(*players_msgs)

		match_msg = match_msg_1+players_msg+match_msg_2

		winning_side = LOCALIZED_STATUS[self.winner]

		match_msg += '\nMatch Winner: {}'.format(winning_side)

		if self.winner != MATCH_STATUS.UNRESOLVED:
			self.gambling_message = 'Bets are closed'

		elif pick_phase:
			self.gambling_message = 'Bets are open'

		elif not pick_phase and not self.gambling_close:
			if match_details['game_time'] > RESTART_LAST_CHANCE:
				self.gambling_close = int(time.mktime(datetime.datetime.now().timetuple()))
				self.gambling_message = 'Bets are closed'
			else:
				self.gambling_close = int(time.mktime(datetime.datetime.now().timetuple()))+GAMBLING_CLOSE_WAIT
				self.gambling_message = 'Bets are closing in ~{} second(s)'.format(GAMBLING_CLOSE_WAIT)
		elif not pick_phase:
			current_time = int(time.mktime(datetime.datetime.now().timetuple()))
			if current_time > self.gambling_close:
				self.gambling_message = 'Bets are closed'
			else:
				remaining = self.gambling_close - current_time
				self.gambling_message = 'Bets are closing in ~{} second(s)'.format(remaining)

		match_msg += '\n\n'+self.gambling_message+'```'

		if self.match_msg_obj == None:
			self.match_msg_obj = await self.info_channel.send(match_msg)
			self.match_msg = match_msg
			self.match_msg_id = self.match_msg_obj.id
			print('new match_msg_id {} for lobby {}'.format(self.match_msg_id,self.lobby_id))
			pgdb.insert_match_message(self.lobby_id,self.match_msg_id)
		else:
			if self.match_msg != match_msg:
				await self.match_msg_obj.edit(content=match_msg)
				print('edited match_msg {}'.format(self.match_msg_id))
				self.match_msg = match_msg
			else:
				print('no update for match_msg_id {}'.format(self.match_msg_id,self.lobby_id))

		announce_msg = '\n'.join(players_only_msgs)
		announce_msg = MatchMsg.players_only_msg_template.format(match_id=match_details['match_id'],
									players_only_msg=announce_msg,
									gambling_status=self.gambling_message)

		if self.announce_msg_obj == None:	
			self.announce_msg_obj = await self.comm_channel.send(announce_msg)
			self.announce_msg = announce_msg
			self.announce_msg_id = self.announce_msg_obj.id
			print('new announce_msg_id {} for lobby {}'.format(self.announce_msg_id,self.lobby_id))
			pgdb.insert_announce_message(self.lobby_id,self.announce_msg_id)
			mobile_friendly_msg = MatchMsg.mobile_friendly_msg.format(match_id=match_details['match_id'])
			await self.comm_channel.send(mobile_friendly_msg)

		else:
			if self.announce_msg != announce_msg:
				await self.announce_msg_obj.edit(content=announce_msg)
				print('edited announce_msg {}'.format(self.announce_msg_id))
				self.announce_msg = announce_msg
			else:
				print('no update for announce_msg_id {}'.format(self.announce_msg_id,self.lobby_id))
		self.gambling_initialized = True
			
	async def announce_winner(self,winner):
		self.gambling_close = -1
		if winner == MATCH_STATUS.RADIANT:
			w = 'Radiant'
		elif winner == MATCH_STATUS.DIRE:
			w = 'Dire'
		elif winner == MATCH_STATUS.ERROR:
			w = 'Not scored'
		else:
			print('announce winner called without winner for lobby id {}'.format(self.lobby_id))
			return
		self.winner = winner
		if self.match_msg_obj != None:
			self.match_msg = self.match_msg_obj.content
			self.match_msg = self.match_msg.replace('Match In Progress',w)
			await self.match_msg_obj.edit(content=self.match_msg)
			print('edited match_msg {}'.format(self.match_msg_id))
		else:
			print('cannot announce winner with non-existent lobby_id {}'.format(self.lobby_id))

	async def oneshot_message():
		pass
		
class Lobby:
	player_msg_template = '{{hero_name:{}}} | {{discord_name}}'.format(LONGEST_HERO_NAME)

	def __init__(self,lobby_id,info_channel,comm_channel,friend_df,client):
		self.lobby_id = lobby_id
		self.friend_df = friend_df
		self.client = client
		self.match_id = None
		self.last_update = None

		self.comm_channel = comm_channel

		self.match_obj = MatchMsg(info_channel,comm_channel,lobby_id)

	async def check_match_details(self):
		print('checking match details for {}'.format(self.match_id))
		status = schuck_match_details(self.match_id)
		if status == MATCH_STATUS.UNRESOLVED:
			print('match id {} has unresolved match details'.format(self.match_id))
			return
		else:
			print('match id {} winner is {}'.format(self.match_id,status))
			pgdb.request_extended_match_details(self.match_id)
			await self.match_obj.announce_winner(status)
			bets_df,charity_df,player_loss_payouts = pgdb.update_match_status(self.match_id,status)

			if status == MATCH_STATUS.ERROR:
				if bets_df is None or bets_df.empty:
					msg = '```Cancelled match id: {}```'.format(self.match_id)
					await self.comm_channel.send(msg)
				else:
					msgs = ['Cancelled match id: {}'.format(self.match_id)]
					msgs.append('Bets Returned:')
					for (gambler_id,amount,side) in bets_df[['gambler_id','amount','side']].values:
						user = await self.client.cached_user(gambler_id)
						msgs.append('{:12d} | {}'.format(amount,user.name))

					for msg in format_long('\n'.join(msgs)):
						await self.comm_channel.send(msg)

			else:
				msgs = ['********************************************\nMatch {} complete. Winner: {}'.format(self.match_id,LOCALIZED_STATUS[status])]
				winners = []
				losers = []
				bonus = []

				if bets_df is not None and not bets_df.empty:
					for (gambler_id,amount,side) in bets_df[['gambler_id','amount','side']].values:
						user = await self.client.cached_user(gambler_id)
						if status == side:
							winners.append('WINNER: {:12d} | {}'.format(amount,user.name))
						else:
							losers.append(' LOSER: {:12d} | {}'.format(amount,user.name))

				if charity_df is not None and not charity_df.empty:
					for (gambler_id,amount,side) in charity_df[['gambler_id','amount','side']].values:
						user = await self.client.cached_user(gambler_id)
						if status == side:
							winners.append('WINNER: {:12d} | {}'.format(amount,user.name))
						else:
							losers.append(' LOSER: {:12d} | {}'.format(amount,user.name))

				for (gid,amount) in player_loss_payouts:
					user = await self.client.cached_user(gid)
					bonus.append(' BONUS: {:12d} | {}'.format(amount,user.name))


				msgs.extend(winners)
				msgs.append('')
				msgs.extend(losers)
				msgs.append('')
				msgs.extend(bonus)

				for msg in format_long('\n'.join(msgs)):
					await self.comm_channel.send(msg)
			failed_imports = perform_import(pgdb)
			for f in failed_imports:
				error_msg = '```Failed importing stats from match id {}```'.format(f.stem)
				await self.comm_channel.send(error_msg)
				

	async def announce(self):
		db_result,columns = pgdb.select_lm(self.lobby_id,self.last_update)
		if len(db_result) == 0:
			return
		live_df = pd.DataFrame(db_result,columns=columns)
		last_update_df = live_df[live_df['query_time'] == live_df['query_time'].max()]
		match_details = list(last_update_df.to_dict().items())
		match_details = dict(map(lambda x: (x[0],next(iter(x[1].values()))),match_details))

		self.last_update = match_details['last_update_time']
		init_match_status = False

		if self.match_id == None:
			self.match_id = match_details['match_id']
			self.client.match_id_to_lobby[self.match_id] = self
			check_match_status = pgdb.check_match_status(self.match_id)
			if len(check_match_status) == 0:
				init_match_status = True
			else:
				check_match_status = check_match_status[0][0]
				if check_match_status != MATCH_STATUS.UNRESOLVED:
					print('match {} already complete, result: {}'.format(self.match_id,check_match_status))
					await self.match_obj.announce_winner(check_match_status)
				else:
					print('match {} in progress'.format(self.match_id))
					
		player_table,columns = pgdb.read_lp(self.match_id)

		player_df = pd.DataFrame(player_table,columns=columns)
		player_df = pd.merge(player_df,self.friend_df,how='left',on=['account_id'])
		pick_phase = any(player_df['hero_id'] == 0)
		player_df.sort_values(by=['player_num'],inplace=True)
		player_df['hero_name'] = player_df['hero_id'].apply(lambda x: hero_data[x][1])
		player_df['side'] = player_df['player_num'].apply(lambda x: int(MATCH_STATUS.RADIANT) if x < 5 else int(MATCH_STATUS.DIRE))
		player_df['side_str'] = player_df['player_num'].apply(lambda x: 'Radiant' if x < 5 else 'Dire')

		if init_match_status:
			pgdb.insert_match_status(self.match_id,player_df[['discord_id','side']].dropna().values)

		if pick_phase:
			players_only_msg_template = '{side_str:7} | {discord_name}'
		else:
			players_only_msg_template = '{{side_str:7}} | {{hero_name:{}}} | {{discord_name}}'.format(LONGEST_HERO_NAME)

		players_only_df = player_df.dropna(subset=['discord_name'])
		players_only_msg = players_only_df.apply(lambda x: players_only_msg_template.format(**x),1)

		player_df['discord_name'] = player_df['discord_name'].fillna('')

		player_msg = player_df.apply(lambda x: Lobby.player_msg_template.format(**x),1)

		await self.match_obj.update(match_details,player_msg.values,players_only_msg.values,pick_phase)



class BookKeeper(commands.Bot):
	def __init__(self, *args, **kwargs):
		super().__init__(*args,**kwargs)

		self.lobby_objs = {}
		self.match_id_to_lobby = {}
		self.waiting_on = set()
		self.tried_once = set()

		self.currently_retrieving_replays = False

		self.info_channel = None
		self.comm_channel = None
		self.friend_df = None

		self.discord_mapping = {}

		self.lobby_listener = self.loop.create_task(self.check_lobbies_loop())

	async def cached_user(self,discord_id):
		try:
			user = self.discord_mapping[discord_id]
		except KeyError:
			user = await self.fetch_user(discord_id)
			self.discord_mapping[discord_id] = user
		return user

	async def check_lobbies_loop(self):
		await self.wait_until_ready()
		print('Logged in as',self.user.name,self.user.id)

		self.info_channel = discord.utils.get(self.get_all_channels(), name=INFO_CHANNEL)
		self.comm_channel = discord.utils.get(self.get_all_channels(), name=COMM_CHANNEL)

		discord_id_df = pd.DataFrame(pgdb.select_discord_ids(),columns=['discord_id','steam_id','account_id'],dtype=pd.Int64Dtype())

		self.friend_df = pd.DataFrame(map(lambda x: x[0],pgdb.select_friends()),columns=['steam_id'],dtype=pd.Int64Dtype())
		self.friend_df['account_id'] = self.friend_df['steam_id'].apply(convert_steam_to_account)
		self.friend_df = pd.merge(discord_id_df,self.friend_df,how='left')

		for index,row in self.friend_df.iterrows():
			self.discord_mapping[row['discord_id']] = await self.fetch_user(row['discord_id'])

		self.friend_df['discord_name'] = self.friend_df['discord_id'].apply(lambda x: self.discord_mapping[x].name)

		active_lobbies = set(map(lambda x: x[0],pgdb.get_unresolved_matches()))
		for lobby_id in active_lobbies:
			print('retrieving lobby id {}'.format(lobby_id))
			await self.refresh_lobby(lobby_id)
			
		while True:
			print('****************************')
			await asyncio.sleep(5)

			live_lobbies = pgdb.get_live()
			live_lobbies = set(map(lambda x: x[0],live_lobbies))

			for live_lobby_id in live_lobbies:
				await self.refresh_lobby(live_lobby_id)

			zombie_lobbies = active_lobbies - live_lobbies

			for zombie in zombie_lobbies:
				print('checking for finished match for lobby id {}'.format(zombie))
				await self.lobby_objs[zombie].check_match_details()

			active_lobbies = live_lobbies

			print('finished iterating through {} lobbies'.format(len(live_lobbies)))

			print('checking for replays...'.format(len(live_lobbies)))
			await self.try_retrieve_replays()



	async def refresh_lobby(self,live_lobby_id):
		print('lobby found: {}'.format(live_lobby_id))

		if live_lobby_id in self.lobby_objs.keys():
			live_lobby = self.lobby_objs[live_lobby_id]
		else:
			live_lobby = Lobby(live_lobby_id,self.info_channel,self.comm_channel,self.friend_df,self)
			await live_lobby.match_obj.init_msg_objs()
			self.lobby_objs[live_lobby_id] = live_lobby

		await live_lobby.announce()

	async def try_retrieve_replays(self):
		if self.currently_retrieving_replays:
			print('\tcannot retrieve replays, method never returned')
			return

		self.currently_retrieving_replays = True
		extended_match_details_path = Path.cwd() / 'extended_match_details'
		extended_match_details_path.mkdir(exist_ok=True)

		retrieved_extended_match_details_path = Path.cwd() / 'retrieved_extended_match_details'
		retrieved_extended_match_details_path.mkdir(exist_ok=True)

		ext_match_details = extended_match_details_path.glob('*.json')

		for ext_md_file in ext_match_details:
			print('checking for replay from {}'.format(ext_md_file.name))
			# only try and download replay once per app run
			if ext_md_file in self.tried_once:
				print('\treplay already checked once this session, skipping.')
				continue
			else:
				self.tried_once.add(ext_md_file)

			lock_file = ext_md_file.parent / (ext_md_file.name + '.lock')
			json_load_success = False
			try:
				with FileLock(lock_file,timeout=5):
					with open(str(ext_md_file), 'r') as json_file:
						try:
							ext_md = json.load(json_file)
							json_load_success = True
						except json.decoder.JSONDecodeError as e:
							# seems like this sometimes goes wrong if the other process
							# is still writing the json. gonna have to think about best solution.
							print('\tcannot process json {}'.format(ext_md_file))
							continue
			except Timeout:
				print('\tcannot process json {} because of file lock'.format(ext_md_file))
				continue

			if json_load_success:
				lock_file.unlink()
				
			cluster = ext_md['match']['cluster']
			app_id = 570
			match_id = ext_md['match']['match_id']
			replay_salt = ext_md['match']['replay_salt']

			replay_url = 'http://replay{0}.valve.net/{1}/{2}_{3}.dem.bz2'.format(cluster, app_id, match_id, replay_salt)
			replay_file = retrieved_extended_match_details_path / '{}.dem.bz2'.format(match_id)

			if not replay_file.exists():
				try:
					r = requests.get(replay_url,timeout = 5,stream=True)
					with open(str(replay_file), 'wb') as f:
						for chunk in r.iter_content(chunk_size=1024):
							if chunk: # filter out keep-alive new chunks
								f.write(chunk)
					print('\treplay {} downloaded'.format(replay_file.name))
					new_match_file = retrieved_extended_match_details_path / ext_md_file.name
					ext_md_file.rename(new_match_file)
				except (requests.ConnectionError, requests.Timeout, requests.RequestException):
					print('try_retrieve_replays {} failed'.format(replay_url))
					continue
			else:
				print('\t!!! replay already exists')

		self.currently_retrieving_replays = False

bot = BookKeeper(command_prefix='!', description='DotaBet')

@bot.command()
async def add_steam_id(ctx,*arg):
	"""Associate your steam id with your discord id."""
	if ctx.guild is None:
		await ctx.send('{}, ALL COMMUNICATION MUST NOW BE PUBLIC'.format(ctx.message.author.mention))
		return
	if ctx.guild and str(ctx.message.channel) != COMM_CHANNEL:
		return
	if len(arg) != 1:
		await ctx.send('Invalid syntax, try !add_steam_id steam_id')
		return
	steam_id = arg[0]
	discord_id = ctx.message.author.id
	if len(steam_id) != 17 or not str(steam_id).isdigit():
		await ctx.send('Invalid syntax, steam_id is a 17 digit number')
	else:
		account_id = convert_steam_to_account(steam_id)
		if (result := pgdb.insert_discord_id(discord_id,steam_id,account_id,ctx.message.author.name)):
			await ctx.send('Added steam_id {} for {}'.format(steam_id,ctx.message.author.mention))

@bot.command()
async def hi(ctx,*arg):
	"""?"""
	if ctx.guild is None:
		await ctx.send('{}, ALL COMMUNICATION MUST NOW BE PUBLIC'.format(ctx.message.author.mention))
		return
	if not ctx.guild or str(ctx.message.channel) == COMM_CHANNEL:
		await ctx.send('{} ?'.format(ctx.message.author.mention))

@bot.command()
async def sacrifice(ctx,*arg):
	"""Sacrifice Nick."""
	if ctx.guild is None:
		await ctx.send('{}, ALL COMMUNICATION MUST NOW BE PUBLIC'.format(ctx.message.author.mention))
		return
	if ctx.guild and str(ctx.message.channel) == COMM_CHANNEL:
		await ctx.send('Kali is pleased.')

@bot.command()
async def balance(ctx,*arg):
	"""Display your balance."""
	if ctx.guild is None:
		await ctx.send('{}, ALL COMMUNICATION MUST NOW BE PUBLIC'.format(ctx.message.author.mention))
		return
	if ctx.guild and str(ctx.message.channel) != COMM_CHANNEL:
		return
	amount = pgdb.check_balance(ctx.message.author.id)
	if amount != 1:
		msg = '{0.author.mention}, you have {1} {2}s'.format(ctx.message,amount,CURRENCY)
	else:
		msg = '{0.author.mention}, you have {1} {2}'.format(ctx.message,amount,CURRENCY)
	await ctx.send(msg)





@bot.command()
async def tip(ctx,*arg):
	if len(arg) != 1:
		await ctx.send('Need to specify tipee.')
		return
	else:
		tipee_id = arg[0]
	regex = re.compile(r'^<@(\d{18}|\d{17})>$')
	matches = regex.search(tipee_id)
	if matches is None:
		await ctx.send('Cannot parse user {}.'.format(arg[0]))
		return
	else:
		tipee_id = matches.groups()[0]
	try:
		tipee_id = int(tipee_id)
	except ValueError:
		await ctx.send('Cannot parse user {}.'.format(arg[0]))
		return

	tipper_id = ctx.message.author.id
	if tipper_id == tipee_id:
		return

	if pgdb.check_user_exists(tipee_id):
		tip_successful = pgdb.tip(tipper_id,tipee_id)
		if tip_successful:
			await ctx.send("{} sent {} a salty tip.".format(ctx.message.author.mention,arg[0]))
		else:
			await ctx.send("{}, you're too poor.".format(ctx.message.author.mention))
	else:
		await ctx.send("User {} isn't participating in Aghanim's Wager.".format(arg[0]))






@bot.command()
async def last(ctx,x_matches='10',user_id=None):
	"""Display your last x matches,  1 <= x <= 50"""
	if ctx.guild is None:
		await ctx.send('{}, ALL COMMUNICATION MUST NOW BE PUBLIC'.format(ctx.message.author.mention))
		return
	if ctx.guild and str(ctx.message.channel) != COMM_CHANNEL:
		return
	try:
		x_matches = int(x_matches)
		if x_matches < 1:
			for msg in format_long('>:('):
				await ctx.send(msg)
				return
		if x_matches > 50:
			for msg in format_long('>:('):
				await ctx.send(msg)
				return
	except ValueError:
		await ctx.send('Invalid syntax, try "!last x" where x is greater than 0 and less than or equal to 50')
		return
		
	if user_id is not None:
		user = user_id
		regex = re.compile(r'^<@(\d{18}|\d{17})>$')
		matches = regex.search(user_id)
		if matches is None:
			await ctx.send('Cannot parse user {}.'.format(arg[0]))
			return
		else:
			user_id = matches.groups()[0]
		try:
			user_id = int(user_id)
		except ValueError:
			await ctx.send('Cannot parse user {}.'.format(arg[0]))
			return
		user_id_repr = await bot.cached_user(user_id)
	else:
		user = ctx.message.author
		user_id = ctx.message.author.id
		user_id_repr = ctx.message.author

	lastten_df = pgdb.lastx(user_id,x_matches)
	if len(lastten_df) > 0:
		try:
			lastten_df['hero'] = lastten_df['hero_id'].apply(lambda x: hero_data[x][1])
		except KeyError:
			await ctx.send('{}, no games were returned, are you sure {} has public stats enabled?'.format(ctx.message.author.mention,user))
			return
		del lastten_df['hero_id']
		lastten_df = lastten_df[['match_id','hero','kills','deaths','assists','hero_damage','net_worth']]
		mean_k = lastten_df['kills'].mean()
		mean_d = lastten_df['deaths'].mean()
		mean_a = lastten_df['assists'].mean()
		s = "```" + str(user_id_repr) + ' average K/D/A: {:.1f} / {:.1f} / {:.1f}'.format(mean_k,mean_d,mean_a) + "```"
		for msg in format_long(lastten_df.to_string()):
			await ctx.send(msg)
		await ctx.send(s)
	else:
		await ctx.send('No matches found')


MAX_MSG_LEN = 2000-6
def format_long(msg):
	while len(msg) > MAX_MSG_LEN:
		split,msg = msg[:MAX_MSG_LEN],msg[MAX_MSG_LEN:]
		chunk_lines = split.split('\n')
		for i in range(len(chunk_lines)-1,0,-1):
			guess = '\n'.join(chunk_lines[:i])
			if len(guess) <= MAX_MSG_LEN:
				yield '```'+guess+'```'
				remainder = '\n'.join(chunk_lines[i:])
				msg = remainder + msg
				break
	yield '```'+msg+'```'

@bot.command()
async def leaderboard(ctx,*arg):
	"""Display leaderboard."""
	if ctx.guild is None:
		await ctx.send('{}, ALL COMMUNICATION MUST NOW BE PUBLIC'.format(ctx.message.author.mention))
		return
	if ctx.guild and str(ctx.message.channel) != COMM_CHANNEL:
		return

	db_result = pgdb.leaderboard()
	agg = []
	for i,(discord_id,amount) in enumerate(db_result):
		user = await bot.cached_user(discord_id)
		agg.append('{:5d} {:12d} | {}'.format(i+1,amount,user.name))

	for msg in format_long('\n'.join(agg)):
		await ctx.send(msg)
		
@bot.command()
async def feederboard(ctx,*arg):
	"""Display the cooler leaderboard."""
	if ctx.guild is None:
		await ctx.send('{}, ALL COMMUNICATION MUST NOW BE PUBLIC'.format(ctx.message.author.mention))
		return
	if ctx.guild and str(ctx.message.channel) != COMM_CHANNEL:
		return

	db_result = pgdb.feederboard()
	agg = []
	agg.append('Rank  | Deaths [last 20] | Name ')
	for i,(discord_id,amount) in enumerate(db_result):
		user = await bot.cached_user(discord_id)
		agg.append('{:5d} | {:16d} | {}'.format(i+1,amount,user.name))

	for msg in format_long('\n'.join(agg)):
		await ctx.send(msg)

@bot.command()
async def redistribute_wealth(ctx,*arg):
	"""Spread the love."""
	if ctx.guild is None:
		await ctx.send('{}, ALL COMMUNICATION MUST NOW BE PUBLIC'.format(ctx.message.author.mention))
		return
	if ctx.guild and str(ctx.message.channel) != COMM_CHANNEL:
		return
	discord_id = ctx.message.author.id
	if ctx.message.author.id == SUPERUSER_ID:
		msg = pgdb.redistribute_wealth(0.05)
		await bot.comm_channel.send(msg)
	else:
		await ctx.send('{}, No.'.format(ctx.message.author.mention))

@bot.command()
async def active_bets(ctx,*arg):
	"""Show currently active bets."""

	#if ctx.guild is None:
	#	await ctx.send('{}, ALL COMMUNICATION MUST NOW BE PUBLIC'.format(ctx.message.author.mention))
	#	return
	if ctx.guild and str(ctx.message.channel) != COMM_CHANNEL:
		return
	if len(arg) == 1:
		match_id = arg[0]
		if not match_id.isdigit():
			await ctx.send('{}, match_id must be an integer'.format(ctx.message.author.mention))
			return 
		else:
			match_id = int(match_id)
		active_bets_df = pgdb.return_active_bets(match_id=match_id)
	else:
		active_bets_df = pgdb.return_active_bets()

	if active_bets_df is not None and not active_bets_df.empty:
		active_bets_df['amount'] = active_bets_df.groupby(['match_id', 'gambler_id', 'side', 'finalized'])['amount'].transform(sum)
		del active_bets_df['wager_id']
		active_bets_df = active_bets_df.drop_duplicates()
		active_bets_df = pd.merge(active_bets_df,bot.friend_df,how='left',left_on=['gambler_id'],right_on=['discord_id'])
		active_bets_df['side_str'] = active_bets_df['side'].apply(lambda x: 'Radiant' if x == MATCH_STATUS.RADIANT else 'Dire')
		active_bets_df['amount_str'] = active_bets_df['amount'].astype(str)
		active_bets_df['match_id_str'] = active_bets_df['match_id'].astype(str)

		acc = []
		for match_id,amount,side,discord_name in active_bets_df[['match_id_str','amount_str','side_str','discord_name']].values:
			acc.append('{} | {:>7} | {:>12} | {}'.format(match_id,side,amount,discord_name))

		for msg in format_long('\n'.join(acc)):
			await ctx.send(msg)
	else:
		await ctx.send('{}, there are no active bets'.format(ctx.message.author.mention))
		
@bot.command()
async def bet(ctx,*arg):
	"""Make a wager on a match."""
	if ctx.guild is None:
		await ctx.send('{}, ALL COMMUNICATION MUST NOW BE PUBLIC'.format(ctx.message.author.mention))
		return
	if ctx.guild and str(ctx.message.channel) != COMM_CHANNEL:
		return
	if len(arg) != 3:
		await ctx.send('{}, try !bet match_id side amount'.format(ctx.message.author.mention))
		return 
	match_id,side,amount = arg
	if not match_id.isdigit():
		await ctx.send('{}, match_id must be an integer'.format(ctx.message.author.mention))
		return 
	else:
		match_id = int(match_id)

	side = side.lower()
	if side == 'radiant':
		side = MATCH_STATUS.RADIANT
	elif side == 'dire':
		side = MATCH_STATUS.DIRE
	else:
		await ctx.send('{}, must enter side as either radiant or dire'.format(ctx.message.author.mention))
		return

	if not amount.isdigit():
		await ctx.send('{}, amount must be a non-negative integer'.format(ctx.message.author.mention))
		return
	else:
		amount = int(amount)
		if amount == 0:
			await ctx.send('{}, amount must be a non-negative integer'.format(ctx.message.author.mention))
			return

	try:
		lobby = bot.match_id_to_lobby[match_id]
	except:
		await ctx.send('{}, match id {} not found'.format(ctx.message.author.mention,match_id))
		return
		
	current_time = int(time.mktime(datetime.datetime.now().timetuple()))
	if lobby.match_obj.gambling_initialized and (not lobby.match_obj.gambling_close or lobby.match_obj.gambling_close > current_time):
		msg = pgdb.insert_bet(match_id,ctx.message.author.id,side,amount)
		msg = msg.replace('CURRENCY',CURRENCY)
		await ctx.send('{}, {}'.format(ctx.message.author.mention,msg))
		return
	else:
		await ctx.send('{}, betting for {} is closed'.format(ctx.message.author.mention,match_id))
		return
		
bot.run(TOKEN)

