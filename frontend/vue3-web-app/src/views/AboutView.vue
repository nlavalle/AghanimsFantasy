<!-- eslint-disable vue/valid-v-slot -->
<template>
  <div class="about">
    <v-container>
      <div>
        <h3>Table of Contents</h3>
        <ol class="toc-list" role="list">
          <li>
            <a href="#introduction">
              <span class="title">About</span>
            </a>
          </li>
          <li>
            <a href="#scoring">
              <span class="title">Scoring</span>
            </a>
          </li>
          <li>
            <a href="#how-to-play">
              <span class="title">How to Play</span>
            </a>
          </li>
          <li>
            <a href="#discord-authentication">
              <span class="title">Discord Authentication</span>
            </a>
          </li>
        </ol>
      </div>
      <div class="mt-5">
        <h2 id="introduction">About Aghanim's Fantasy</h2>
        <p class="text-white" style="max-width: 800px;">
          Aghanim's Fantasy is a Dota 2 Fantasy League that can be played online or via the Aghanim's Fantasy Discord
          bot. For Dota tournaments, fantasy leagues will be created for the different stages of the game
          (group stage, playoffs, etc.) where you'll be allowed to draft one of the competing players for each of the 5
          positions. For your draft players, each of their matches will be scored based on their stats for the match,
          such as kills, deaths, assists, and support stats like dewards and camps stacked. A leaderboard shows the
          best player drafts in the fantasy leagues, and occasionally we will give out prizes.
          <br /><br />
          This site is a passion project by Nick (gloom.cloud on Discord) trying to make Dota a better community than
          other e-Sports, giving viewers more fun ways to engage with tournaments, and trying to spotlight up and coming
          talent entering the Dota scene.
        </p>
      </div>
      <div class="mt-5">
        <h2 id="scoring">Scoring</h2>
        <p class="text-white" style="max-width: 800px;">
          Points are calculated using the pattern below trying to follow the TI fantasy scoring.
          Certain statistics require parsing the .dem replay files to measure, so the unavailable
          metrics won't be involved in the calculation until that is added to the backend service that
          fetches match data.
        </p>
        <v-data-table class="scoring-table" :items="statistics" :items-per-page="15" :headers="statsHeaders"
          hide-default-footer density="compact">
          <template v-slot:item.value="{ item }">
            <span :style="getPointsPer(item)">{{ item.value }}</span>
          </template>
          <template v-slot:item.available="{ item }">
            <span :style="getAvailability(item)">{{ item.available }}</span>
          </template>
        </v-data-table>
      </div>
      <div class="mt-5">
        <h2 id="how-to-play">How to Play</h2>
        <p class="text-white" style="max-width: 800px;">
          Dota refers to their tournaments as leagues, and custom lobbies in that tournament will be ticketed as that
          league. You can identify available leagues to draft in with the navigation bar at the top. Any league without
          a lock icon is open to draft in, so clicking on them will show the fantasy leagues available for that league.
          Anything grayed out indicates that the league/fantasy league isn't currently active, where the current time is
          before when that league begins/after when the league ends.
          <br /><br />
          When you have a fantasy league selected that you're interested in drafting in, you can navigate to the Fantasy
          tab to make your draft. The "Current Draft" will show what you have drafted, and the "Draft Players" will show
          the available player cards you can draft for the fantasy league. Similar to the Dota in-game drafting, you
          will see the 5 positions at the top, going from 1 (Carry) to 5 (Hard Support) left to right, as you draft for
          each role it will display the available players for that role you can choose from, and disable the other
          roles, you cannot draft a player for a role outside of his intended role.
          <br /><br />
          Clicking a player card will show you additional details about that player across past games, such as their top
          3 heroes and their win rates, along with some charts on their playstyle and main sources of fantasy points. If
          you would still like to draft this player you can click the "Draft Player" button which will add them to your
          5 draft slots at the top. These picks aren't locked in until you click the "Save Draft" button at the top
          right, if you want to start over at any point you can click the "Clear Draft" button to remove any picks you
          have chosen from the draft.
          <br /><br />
          Once you have chosen your 5 players and saved your draft you should see those 5 players in your Current Draft
          tab, these Current Draft players will be what you earn your fantasy points on, and throughout the tournament
          you'll be able to see their current Fantasy Point totals at that state of the games.
          <br /><br />
          If you're interested in a detailed list of matches, you can click the Fantasy Matches tab to see just the Dota
          matches that include your Fantasy draft, with a breakdown of the stats they earned their Fantasy Points from.
        </p>
      </div>
      <div class="mt-5">
        <h2 id="discord-authentication">Discord Authentication</h2>
        <p class="text-white" style="max-width: 800px;">
          This site uses <a href="https://discord.com/developers/docs/topics/oauth2">Discord OAuth2</a> to identify
          which Fantasy drafts belong to which players. OAuth2 works by having you log into Discord via Discord's
          portal, then Aghanim's Fantasy requests a token that allows it to identify the current user through the <a
            href="https://discord.com/developers/docs/resources/user#get-current-user">@me endpoint</a>. At no point
          will Aghanim's Fantasy ever see your login credentials or anything not available through the <a
            href="https://discord.com/developers/docs/topics/oauth2#shared-resources-oauth2-scopes">"identify"
            scope</a>.
          <br /><br />
          An alternative to drafting via the browser is to Draft through the Aghanim's Fantasy Discord. If you join the
          Aghanim's Fantasy Discord you can use the "/set-fantasy-draft" command to draft your team straight from
          Discord. Since the bot can identify the Discord user issuing the command there will be no login prompt like
          the browser has.
          <br /><br />
          If you have any additional questions about the authentication or security in general feel free to reach out to
          the support email at the bottom of this page.
        </p>
      </div>
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { VContainer, VDataTable } from 'vuetify/components'

const statsHeaders = [
  {
    title: 'Name',
    value: 'name',
    width: '50%'
  },
  {
    title: 'Points Per',
    value: 'value',
    width: '30%'
  },
  {
    title: 'Available?',
    value: 'available',
    width: '20%'
  },
];

const statistics = [
  {
    name: 'Kill',
    value: '0.3',
    available: 'Yes'
  },
  {
    name: 'Death',
    value: '-0.3',
    available: 'Yes'
  },
  {
    name: 'Assist',
    value: '0.15',
    available: 'Yes'
  },
  {
    name: 'Last hit',
    value: '0.003',
    available: 'Yes'
  },
  {
    name: 'Gold per minute',
    value: '0.002',
    available: 'Yes'
  },
  {
    name: 'XP per minute',
    value: '0.002',
    available: 'Yes'
  },
  {
    name: 'Ward Planted',
    value: '0.15',
    available: 'Yes'
  },
  {
    name: 'Deward',
    value: '0.15',
    available: 'Yes'
  },
  {
    name: 'Camp Stacked',
    value: '0.5',
    available: 'Yes'
  },
  {
    name: 'Courier Kill',
    value: '0.2',
    available: 'Yes'
  },
  {
    name: 'Stun Duration (sec)',
    value: '0.025',
    available: 'Yes'
  },
  {
    name: 'Tower Kill',
    value: '1',
    available: 'No'
  },
  {
    name: 'Roshan Kill',
    value: '1',
    available: 'No'
  },
  {
    name: 'Team Fight',
    value: '3',
    available: 'No'
  },
  {
    name: 'Runes Grabbed',
    value: '0.25',
    available: 'No'
  },
  {
    name: 'First Blood',
    value: '4.0',
    available: 'No'
  }
]

const getPointsPer = (field: { available: string; name: string }) => {
  return field.available == 'Yes'
    ? field.name == 'Death'
      ? 'color: red'
      : 'color: white'
    : 'color: grey'
}

const getAvailability = (field: { available: string }) => {
  return field.available == 'Yes' ? 'color: white' : 'color: grey'
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.toc-list {
  list-style-position: inside;
}

.scoring-table {
  margin-top: 20px;
  max-width: 375px;
  border: 2px solid var(--aghanims-fantasy-main-2);
  border-radius: 5px;
}

.v-data-table ::v-deep(thead) {
  background-color: var(--aghanims-fantasy-main-2);
}

.v-data-table ::v-deep(td) {
  border-right: 1px solid var(--aghanims-fantasy-main-2);
}
</style>
