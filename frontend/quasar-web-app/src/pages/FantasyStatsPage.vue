<template>
    <div class="flex-container">
        <div class="row" style="max-width:400px">
            <q-tabs v-model="tab" class="text-grey-5" active-color="grey-1" indicator-color="red-13" narrow-indicator>
                <q-tab name="fantasy" label="Fantasy" />
                <q-tab name="league" label="League" />
            </q-tabs>
        </div>
        <q-separator />
        <div class="row">
            <q-tab-panels v-model="tab" animated style="width:100%;max-width:1800px">
                <q-tab-panel name="fantasy" style="padding: 0px">
                    <div class="row">
                        <div style="width:55%;max-width:300px; padding:10px;">
                            <q-input v-model="fantasyFilter" debounce="500" color="red-13" label="Search" dense outlined>
                                <template v-slot:append>
                                    <q-icon name="search" />
                                </template>
                            </q-input>
                        </div>
                        <div style="display:flex;align-items: center;">
                            <q-btn v-if="selectedFantasyPlayer.length > 0" flat icon="highlight_off" size="14px"
                                padding="md xs" @click="clearSelectedFantasyPlayers" />
                        </div>
                    </div>
                    <div class="row">
                        <q-tabs v-if="!this.isDesktop" v-model="fantasyTab" dense class="text-grey-5" active-color="grey-1"
                            indicator-color="red-13" style="margin-bottom:5px">
                            <q-tab name="kda" label="K/D/A" />
                            <q-tab name="farm" label="Farm" />
                            <q-tab name="support" label="Supp." />
                            <q-tab name="damageHealing" label="Dmg/Heal" />
                        </q-tabs>
                    </div>
                    <div class="row">
                        <q-table class="fantasy-stats-table" dense :columns="displayedFantasyColumns"
                            :rows="playerFantasyStatsIndexed" virtual-scroll :rows-per-page-options="[0]" style="width:100%"
                            separator="vertical">
                            <template v-slot:body-cell-fantasyPlayerRank="props">
                                <q-td :props="props" style="padding:0" auto-width>
                                    <div>
                                        {{ props.value }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-fantasyPlayer="props">
                                <q-td :props="props">
                                    <div class="row">
                                        <div v-if="this.isDesktop" class="col" style="max-width:65px">
                                            <q-img height="60px" width="60px" :src="props.value.playerPicture" />
                                        </div>
                                        <div class="col">
                                            <div style="white-space:normal">
                                                <b>{{ props.value.playerName }}</b>
                                                <br>
                                                {{ props.value.teamName }}
                                                <q-img :src=getPositionIcon(props.value.teamPosition) height="15px" width="15px"/>
                                            </div>
                                            <div class="text-grey-6">
                                                {{ props.value.matches }} games
                                            </div>
                                        </div>
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalKills="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.killPoints }}</b>
                                        <br>
                                        ({{ props.value.kills }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalDeaths="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.deathPoints }}</b>
                                        <br>
                                        ({{ props.value.deaths }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalAssists="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.assistPoints }}</b>
                                        <br>
                                        ({{ props.value.assists }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalLastHits="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.lastHitsPoints }}</b>
                                        <br>
                                        ({{ props.value.lastHits }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalGoldPerMin="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.goldPerMinPoints }}</b>
                                        <br>
                                        ({{ props.value.goldPerMin }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalXpPerMin="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.xpPerMinPoints }}</b>
                                        <br>
                                        ({{ props.value.xpPerMin }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalSupportGoldSpent="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.supportGoldSpentPoints }}</b>
                                        <br>
                                        ({{ props.value.supportGoldSpent }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalObsPlaced="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.observerWardsPlacedPoints }}</b>
                                        <br>
                                        ({{ props.value.observerWardsPlaced }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalSentriesPlaced="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.sentryWardsPlacedPoints }}</b>
                                        <br>
                                        ({{ props.value.sentryWardsPlaced }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalWardsDewarded="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.wardsDewardedPoints }}</b>
                                        <br>
                                        ({{ props.value.wardsDewarded }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalCampsStacked="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.campsStackedPoints }}</b>
                                        <br>
                                        ({{ props.value.campsStacked }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalHeroDamage="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.heroDamagePoints }}</b>
                                        <br>
                                        ({{ props.value.heroDamage }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalTowerDamage="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.towerDamagePoints }}</b>
                                        <br>
                                        ({{ props.value.towerDamage }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalHeroHealing="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.heroHealingPoints }}</b>
                                        <br>
                                        ({{ props.value.heroHealing }})
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalStunDuration="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <b>{{ props.value.stunDurationPoints }}</b>
                                        <br>
                                        ({{ props.value.stunDuration }})
                                    </div>
                                </q-td>
                            </template>
                        </q-table>
                    </div>
                    <!-- <q-footer class="q-pa-sm compare-footer">
                        <q-btn flat style="background: var(--nadcl-main-2)" label="Compare" />
                    </q-footer> -->
                </q-tab-panel>
                <q-tab-panel class="collapse-transition" name="league" style="padding: 0px">
                    <div class="row">
                        <div style="width:55%;max-width:300px; padding:10px;">
                            <q-input v-model="leagueFilter" debounce="500" color="red-13" label="Search" dense outlined>
                                <template v-slot:append>
                                    <q-icon name="search" />
                                </template>
                            </q-input>
                        </div>
                        <div style="display:flex;align-items: center;">
                            <q-btn v-if="selectedLeaguePlayer.length > 0" flat icon="highlight_off" size="14px"
                                padding="md xs" @click="clearSelectedLeaguePlayers" />
                        </div>
                    </div>
                    <div class="row">
                        <q-tabs v-if="!this.isDesktop" v-model="leagueTab" dense class="text-grey-5" active-color="grey-1"
                            indicator-color="red-13" style="margin-bottom:5px">
                            <q-tab name="kda" label="K/D/A" />
                            <q-tab name="farm" label="Farm" />
                            <q-tab name="support" label="Supp." />
                            <q-tab name="damageHealing" label="Dmg/Heal" />
                        </q-tabs>
                    </div>
                    <div class="row" id="leagueTable">
                        <q-table class="league-stats-table" dense :columns="displayedLeagueColumns"
                            :rows="fantasyLeagueMetadataStatsIndexed" virtual-scroll
                            :rows-per-page-options="[0, 5, 10, 15, 25, 50]" separator="vertical" style="width:100%"
                            selection="multiple" v-model:selected="selectedLeaguePlayer" :row-key="row => row.player.id"
                            :pagination="leaguePagination">
                            <template v-slot:header="props">
                                <q-tr :props="props">
                                    <q-th v-for="col in props.cols" :key="col.name" :props="props">
                                        {{ col.label }}
                                    </q-th>
                                </q-tr>
                            </template>
                            <template v-slot:body="props">
                                <q-tr :props="props" @click="selectLeagueRow(props.row)">
                                    <q-td key="leaguePlayer" :props="props">
                                        <div class="row" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            <div v-if="this.isDesktop" class="col" style="max-width:65px">
                                                <q-img height="60px" width="60px"
                                                    :src="props.row.player.dotaAccount.steamProfilePicture" />
                                            </div>
                                            <div class="col">
                                                <div style="white-space:normal">
                                                    <b>{{ props.row.player.dotaAccount.name }}</b>
                                                    <br>
                                                    {{ props.row.player.team.name }}
                                                    <q-img :src=getPositionIcon(props.row.player.teamPosition) height="15px" width="15px"/>
                                                </div>
                                            </div>
                                        </div>
                                    </q-td>
                                    <q-td key="totalKills" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.kills }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalDeaths" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.deaths }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalAssists" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.assists }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalLastHits" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.lastHits }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalDenies" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.denies }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalGoldPerMin" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.goldPerMin }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalXpPerMin" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.xpPerMin }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalSupportGoldSpent" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ (props.row.supportGoldSpent / 1000 ?? 0).toFixed(1) + 'k' }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalObsPlaced" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.observerWardsPlaced ?? 0 }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalSentriesPlaced" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.sentryWardsPlaced ?? 0 }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalWardsDewarded" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.wardsDewarded ?? 0 }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalCampsStacked" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ props.row.campsStacked ?? 0 }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalHeroDamage" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ (props.row.heroDamage ?? 0).toLocaleString() }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalTowerDamage" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ (props.row.towerDamage ?? 0).toLocaleString() }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalHeroHealing" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ (props.row.heroHealing ?? 0).toLocaleString() }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalStunDuration" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ (props.row.stunDuration ?? 0).toFixed(1).toLocaleString() }}
                                        </div>
                                    </q-td>
                                </q-tr>
                            </template>
                        </q-table>
                    </div>
                    <div class="row collapsed" id="leagueCompareTable">
                        <q-table class="league-stats-table" dense :columns="displayedLeagueColumns"
                            :rows="compareLeaguePlayers" virtual-scroll separator="vertical" style="width:100%"
                            selection="multiple" v-model:selected="selectedLeaguePlayer" :row-key="row => row.player.id">
                            <template v-slot:header="props">
                                <q-tr :props="props">
                                    <q-th v-for="col in props.cols" :key="col.name" :props="props">
                                        {{ col.label }}
                                    </q-th>
                                </q-tr>
                            </template>
                            <template v-slot:body="props">
                                <q-tr :props="props" @click="selectLeagueRow(props.row)">
                                    <q-td key="leaguePlayer" :props="props">
                                        <div class="row" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            <div v-if="this.isDesktop" class="col" style="max-width:65px">
                                                <q-img height="60px" width="60px"
                                                    :src="props.row.player.dotaAccount.playerPicture" />
                                            </div>
                                            <div class="col">
                                                <div style="white-space:normal">
                                                    <b>{{ props.row.player.dotaAccount.name }}</b>
                                                    <br>
                                                    {{ props.row.player.team.name }}
                                                    <q-img :src=getPositionIcon(props.row.player.teamPosition) height="15px" width="15px"/>
                                                </div>
                                            </div>
                                        </div>
                                    </q-td>
                                    <q-td key="totalKills" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.killsAverage.toFixed(1) :
                                                props.row.kills }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalDeaths" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.deathsAverage.toFixed(1) :
                                                props.row.deaths }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalAssists" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.assistsAverage.toFixed(1) :
                                                props.row.assists }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalLastHits" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.lastHitsAverage.toFixed(1) :
                                                props.row.lastHits }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalDenies" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.deniesAverage.toFixed(1) : props.row.denies }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalGoldPerMin" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.goldPerMinAverage.toFixed(1) : props.row.goldPerMin }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalXpPerMin" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.xpPerMinAverage.toFixed(1) : props.row.xpPerMin }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalSupportGoldSpent" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.supportGoldSpentAverage.toFixed(1) : (props.row.supportGoldSpent / 1000 ?? 0).toFixed(1) + 'k' }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalObsPlaced" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.observerWardsPlacedAverage.toFixed(1) :props.row.observerWardsPlaced ?? 0 }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalSentriesPlaced" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.sentryWardsPlacedAverage.toFixed(1) :props.row.sentryWardsPlaced ?? 0 }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalWardsDewarded" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.wardsDewardedAverage.toFixed(1) :props.row.wardsDewarded ?? 0 }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalCampsStacked" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.campsStackedAverage.toFixed(1) :props.row.campsStacked ?? 0 }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalHeroDamage" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.heroDamageAverage.toFixed(1) :(props.row.heroDamage ?? 0).toLocaleString() }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalTowerDamage" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.towerDamageAverage.toFixed(1) :(props.row.towerDamage ?? 0).toLocaleString() }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalHeroHealing" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.heroHealingAverage.toFixed(1) :(props.row.heroHealing ?? 0).toLocaleString() }}
                                        </div>
                                    </q-td>
                                    <q-td key="totalStunDuration" :props="props">
                                        <div style="white-space:normal" :style="{ fontSize: isDesktop ? '1em' : '0.90em' }">
                                            {{ leagueCompareTab == 'avg' ? props.row.stunDurationAverage.toFixed(1) :(props.row.stunDuration ?? 0).toFixed(1).toLocaleString() }}
                                        </div>
                                    </q-td>
                                </q-tr>
                            </template>
                        </q-table>
                    </div>
                    <q-footer v-if="selectedLeaguePlayer.length == 2 || compareOn" class="q-pa-sm compare-footer">
                        <div style="display:flex;">
                            <q-tabs v-if="compareOn" v-model="leagueCompareTab" dense class="text-grey-5"
                                active-color="grey-1" indicator-color="red-13" style="margin-bottom:5px">
                                <q-tab name="avg" label="AVG" />
                                <q-tab name="sum" label="TOTAL" />
                            </q-tabs>
                            <q-btn flat style="width:100%;background: var(--nadcl-main-2)" @click="CompareLeaguePlayers"
                                :label="compareOn ? 'Back' : 'Compare'" />
                        </div>
                    </q-footer>
                </q-tab-panel>
            </q-tab-panels>
        </div>
    </div>
</template>
  
<script>
import { ref, onMounted, watch, computed } from 'vue';
import { localApiService } from 'src/services/localApiService';
import { useAuthStore } from 'stores/auth';
import { useLeagueStore } from 'src/stores/league';

export default {
    name: 'FantasyStatsPage',
    setup() {
        const authStore = useAuthStore();
        const leagueStore = useLeagueStore();

        const tab = ref('fantasy');
        const fantasyTab = ref('kda');
        const fantasyFilter = ref('');
        const leagueTab = ref('kda');
        const leagueCompareTab = ref('avg');
        const leagueFilter = ref('');
        const compareOn = ref(false);
        const leaguePagination = ref({
            sortBy: 'desc',
            descending: false,
            page: 1,
            rowsPerPage: 15
        })
        const isDesktop = ref(window.outerWidth >= 600);

        const showFantasyFilters = ref(false);

        const playerFantasyStats = ref([]);
        const fantasyLeagueMetadataStats = ref([]);
        const commonFantasyColumns = [
            {
                name: 'fantasyPlayerRank',
                label: '',
                align: 'center',
                field: row => row.position,
                style: 'width: 15px',
                sortable: false
            },
            {
                name: 'fantasyPlayer',
                label: 'Player/Team/Games',
                align: 'left',
                field: row => {
                    return {
                        playerName: row.fantasyPlayer.dotaAccount.name,
                        playerPicture: row.fantasyPlayer.dotaAccount.steamProfilePicture,
                        teamName: row.fantasyPlayer.team.name,
                        teamPosition: row.fantasyPlayer.teamPosition,
                        matches: row.totalMatches
                    };
                },
                style: 'width: 400px',
                sortable: false
            },
            {
                name: 'totalPoints',
                label: isDesktop.value ? 'Total Points' : 'Pts',
                align: 'left',
                field: row => row.totalMatchFantasyPoints.toFixed(1),
                format: val => `${val.toLocaleString()}`,
                headerStyle: 'font-weight: bold',
                style: 'font-weight: bold',
                sortable: true,
                sort: (a, b) => a - b
            },
        ];
        const kdaFantasyColumns = [
            {
                name: 'totalKills',
                label: isDesktop.value ? 'Kills' : 'K',
                align: 'left',
                field: row => {
                    return {
                        kills: row.totalKills,
                        killPoints: row.totalKillsPoints.toFixed(1)
                    };
                },
                sortable: true,
                sort: (a, b) => a.killPoints - b.killPoints
            },
            {
                name: 'totalDeaths',
                label: isDesktop.value ? 'Deaths' : 'D',
                align: 'left',
                field: row => {
                    return {
                        deaths: row.totalDeaths,
                        deathPoints: row.totalDeathsPoints.toFixed(1)
                    };
                },
                sortable: true,
                sort: (a, b) => a.deathPoints - b.deathPoints
            },
            {
                name: 'totalAssists',
                label: isDesktop.value ? 'Assists' : 'A',
                align: 'left',
                field: row => {
                    return {
                        assists: row.totalAssists,
                        assistPoints: row.totalAssistsPoints.toFixed(1)
                    };
                },
                sortable: true,
                sort: (a, b) => a.assistPoints - b.assistPoints
            },
        ];
        const farmFantasyColumns = [
            {
                name: 'totalLastHits',
                label: isDesktop.value ? 'Last Hits' : 'LH',
                align: 'left',
                field: row => {
                    return {
                        lastHits: row.totalLastHits.toLocaleString(),
                        lastHitsPoints: row.totalLastHitsPoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.lastHitsPoints - b.lastHitsPoints
            },
            {
                name: 'totalGoldPerMin',
                label: isDesktop.value ? 'Avg GPM' : 'G',
                align: 'left',
                field: row => {
                    return {
                        goldPerMin: row.avgGoldPerMin.toFixed(0).toLocaleString(),
                        goldPerMinPoints: row.totalGoldPerMinPoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.goldPerMinPoints - b.goldPerMinPoints
            },
            {
                name: 'totalXpPerMin',
                label: isDesktop.value ? 'Avg XPM' : 'XP',
                align: 'left',
                field: row => {
                    return {
                        xpPerMin: row.avgXpPerMin.toFixed(0).toLocaleString(),
                        xpPerMinPoints: row.totalXpPerMinPoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.xpPerMinPoints - b.xpPerMinPoints
            },
        ];
        const supportFantasyColumns = [
            {
                name: 'totalSupportGoldSpent',
                label: isDesktop.value ? 'Supp. Gold Spent' : 'SG',
                align: 'left',
                field: row => {
                    return {
                        supportGoldSpent: row.totalSupportGoldSpent.toFixed(0).toLocaleString() ?? 0,
                        supportGoldSpentPoints: row.totalSupportGoldSpentPoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.supportGoldSpentPoints - b.supportGoldSpentPoints
            },
            {
                name: 'totalObsPlaced',
                label: isDesktop.value ? 'Obs Placed' : 'OB',
                align: 'left',
                field: row => {
                    return {
                        observerWardsPlaced: row.totalObserverWardsPlaced.toFixed(0).toLocaleString() ?? 0,
                        observerWardsPlacedPoints: row.totalObserverWardsPlacedPoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.observerWardsPlacedPoints - b.observerWardsPlacedPoints
            },
            {
                name: 'totalSentriesPlaced',
                label: isDesktop.value ? 'Sentries Placed' : 'SN',
                align: 'left',
                field: row => {
                    return {
                        sentryWardsPlaced: row.totalSentryWardsPlaced.toFixed(0).toLocaleString() ?? 0,
                        sentryWardsPlacedPoints: row.totalSentryWardsPlacedPoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.sentryWardsPlacedPoints - b.sentryWardsPlacedPoints
            },
            {
                name: 'totalWardsDewarded',
                label: isDesktop.value ? 'Dewards' : 'DW',
                align: 'left',
                field: row => {
                    return {
                        wardsDewarded: row.totalWardsDewarded.toFixed(0).toLocaleString() ?? 0,
                        wardsDewardedPoints: row.totalWardsDewardedPoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.wardsDewardedPoints - b.wardsDewardedPoints
            },
            {
                name: 'totalCampsStacked',
                label: isDesktop.value ? 'Camps Stacked' : 'C',
                align: 'left',
                field: row => {
                    return {
                        campsStacked: row.totalCampsStacked.toFixed(0).toLocaleString() ?? 0,
                        campsStackedPoints: row.totalCampsStackedPoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.campsStackedPoints - b.campsStackedPoints
            },
        ];
        const damageHealingFantasyColumns = [
            {
                name: 'totalHeroDamage',
                label: isDesktop.value ? 'Hero Dmg' : 'HD',
                align: 'left',
                field: row => {
                    return {
                        heroDamage: row.totalHeroDamage.toLocaleString() ?? 0,
                        heroDamagePoints: row.totalHeroDamagePoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.heroDamagePoints - b.heroDamagePoints
            },
            {
                name: 'totalTowerDamage',
                label: isDesktop.value ? 'Tower Dmg' : 'TD',
                align: 'left',
                field: row => {
                    return {
                        towerDamage: row.totalTowerDamage.toLocaleString() ?? 0,
                        towerDamagePoints: row.totalTowerDamagePoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.towerDamagePoints - b.towerDamagePoints
            },
            {
                name: 'totalHeroHealing',
                label: isDesktop.value ? 'Hero Healing' : 'HH',
                align: 'left',
                field: row => {
                    return {
                        heroHealing: row.totalHeroHealing.toLocaleString() ?? 0,
                        heroHealingPoints: row.totalHeroHealingPoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.heroHealingPoints - b.heroHealingPoints
            },
            {
                name: 'totalStunDuration',
                label: isDesktop.value ? 'Stun Dur.' : 'SD',
                align: 'left',
                field: row => {
                    return {
                        stunDuration: row.totalStunDuration.toFixed(1).toLocaleString() ?? 0,
                        stunDurationPoints: row.totalStunDurationPoints.toFixed(1).toLocaleString()
                    };
                },
                sortable: true,
                sort: (a, b) => a.stunDurationPoints - b.stunDurationPoints
            },
        ];

        const commonLeagueColumns = [
            {
                name: 'leaguePlayer',
                label: 'Player/Team',
                align: 'left',
                field: row => row.player.dotaAccount.name.toLowerCase(),
                style: 'width: 400px',
                sortable: true,
                sort: (a, b) => {
                    if (a > b) return 1;
                    if (a < b) return -1;
                }
            },
        ];
        const kdaLeagueColumns = [
            {
                name: 'totalKills',
                label: isDesktop.value ? 'Kills' : 'K',
                align: 'left',
                field: row => row.kills,
                sortable: true,
                sort: (a, b) => a - b
            },
            {
                name: 'totalDeaths',
                label: isDesktop.value ? 'Deaths' : 'D',
                align: 'left',
                field: row => row.deaths,
                sortable: true,
                sort: (a, b) => a - b
            },
            {
                name: 'totalAssists',
                label: isDesktop.value ? 'Assists' : 'A',
                align: 'left',
                field: row => row.assists,
                sortable: true,
                sort: (a, b) => a - b
            },
        ];
        const farmLeagueColumns = [
            {
                name: 'totalLastHits',
                label: isDesktop.value ? 'Last Hits' : 'LH',
                align: 'left',
                field: row => row.lastHits,
                sortable: true,
                sort: (a, b) => a - b
            },
            {
                name: 'totalDenies',
                label: isDesktop.value ? 'Denies' : 'DN',
                align: 'left',
                field: row => row.denies,
                sortable: true,
                sort: (a, b) => a - b
            },
            {
                name: 'totalGoldPerMin',
                label: isDesktop.value ? 'Avg GPM' : 'G',
                align: 'left',
                field: row => row.goldPerMin,
                sortable: true,
                sort: (a, b) => a - b
            },
            {
                name: 'totalXpPerMin',
                label: isDesktop.value ? 'Avg XPM' : 'XP',
                align: 'left',
                field: row => row.xpPerMin,
                sortable: true,
                sort: (a, b) => a - b
            },
        ];
        const supportLeagueColumns = [
            {
                name: 'totalSupportGoldSpent',
                label: isDesktop.value ? 'Supp. Gold Spent' : 'SG',
                align: 'left',
                field: row => {
                    return {
                        supportGoldSpent: row.metadataPlayer?.supportGoldSpent ?? 0,
                    };
                },
                sortable: true,
                sort: (a, b) => a.supportGoldSpent - b.supportGoldSpent
            },
            {
                name: 'totalObsPlaced',
                label: isDesktop.value ? 'Obs Placed' : 'OB',
                align: 'left',
                field: row => row.observerWardsPlaced ?? 0,
                sortable: true,
                sort: (a, b) => a - b
            },
            {
                name: 'totalSentriesPlaced',
                label: isDesktop.value ? 'Sentries Placed' : 'SN',
                align: 'left',
                field: row => row.sentryWardsPlaced ?? 0,
                sortable: true,
                sort: (a, b) => a - b
            },
            {
                name: 'totalWardsDewarded',
                label: isDesktop.value ? 'Dewards' : 'DW',
                align: 'left',
                field: row => {
                    return {
                        wardsDewarded: row.metadataPlayer?.wardsDewarded ?? 0
                    };
                },
                sortable: true,
                sort: (a, b) => a.wardsDewarded - b.wardsDewarded
            },
            {
                name: 'totalCampsStacked',
                label: isDesktop.value ? 'Camps Stacked' : 'C',
                align: 'left',
                field: row => row.campsStacked ?? 0,
                sortable: true,
                sort: (a, b) => a - b
            },
        ];
        const damageHealingLeagueColumns = [
            {
                name: 'totalHeroDamage',
                label: isDesktop.value ? 'Hero Dmg' : 'HD',
                align: 'left',
                field: row => row.heroDamage ?? 0,
                sortable: true,
                sort: (a, b) => a - b
            },
            {
                name: 'totalTowerDamage',
                label: isDesktop.value ? 'Tower Dmg' : 'TD',
                align: 'left',
                field: row => row.towerDamage ?? 0,
                sortable: true,
                sort: (a, b) => a - b
            },
            {
                name: 'totalHeroHealing',
                label: isDesktop.value ? 'Hero Healing' : 'HH',
                align: 'left',
                field: row => row.heroHealing ?? 0,
                sortable: true,
                sort: (a, b) => a - b
            },
            {
                name: 'totalStunDuration',
                label: isDesktop.value ? 'Stun Dur.' : 'SD',
                align: 'left',
                field: row => row.stunDuration ?? 0,
                sortable: true,
                sort: (a, b) => a - b
            },
        ];

        const selectedFantasyColumns = ref(commonFantasyColumns.map(column => column.name));
        const selectedLeaguePlayer = ref([]);
        const compareLeaguePlayers = ref([]);

        const selectedFantasyPlayer = ref([]);
        const compareFantasyPlayers = ref([]);

        onMounted(() => {
            if (leagueStore.selectedLeague) {
                localApiService.getPlayerFantasyStats(leagueStore.selectedLeague.id)
                    .then(result => playerFantasyStats.value = result);
                localApiService.getFantasyLeagueMetadataStats(leagueStore.selectedLeague.id)
                    .then(result => fantasyLeagueMetadataStats.value = result);
            }
        });

        // Define a function to stringify nested objects recursively
        const stringifyNested = (obj) => {
            if (typeof obj !== 'object' || obj === null) {
                return String(obj);
            }
            return Object.values(obj)
                .map(val => stringifyNested(val))
                .join(' ');
        };

        const selectLeagueRow = (selectedRow) => {
            const index = selectedLeaguePlayer.value.findIndex(row => row.player === selectedRow.player);

            if (index !== -1) {
                selectedLeaguePlayer.value.splice(index, 1);
            } else {
                if (selectedLeaguePlayer.value.length < 2) {
                    selectedLeaguePlayer.value.push(selectedRow);
                }
            }
        };

        const selectFantasyRow = (selectedRow) => {
            const index = selectedFantasyPlayer.value.findIndex(row => row.player === selectedRow.player);

            if (index !== -1) {
                selectedFantasyPlayer.value.splice(index, 1);
            } else {
                if (selectedFantasyPlayer.value.length < 2) {
                    selectedFantasyPlayer.value.push(selectedRow);
                }
            }
        };

        const clearSelectedLeaguePlayers = () => {
            selectedLeaguePlayer.value = [];
        };

        const clearSelectedFantasyPlayers = () => {
            selectedFantasyPlayer.value = [];
        };

        const CompareLeaguePlayers = () => {
            compareOn.value = !compareOn.value;
            var leagueTable = document.getElementById('leagueTable');
            leagueTable.classList.toggle("collapsed");

            var leagueCompareTable = document.getElementById('leagueCompareTable');
            leagueCompareTable.classList.toggle("collapsed");
            compareLeaguePlayers.value = [...selectedLeaguePlayer.value];
            clearSelectedLeaguePlayers();
        }

        const CompareFantasyPlayers = () => {
            compareOn.value = !compareOn.value;
            var fantasyTable = document.getElementById('fantasyTable');
            fantasyTable.classList.toggle("collapsed");

            var fantasyCompareTable = document.getElementById('fantasyCompareTable');
            fantasyCompareTable.classList.toggle("collapsed");
            compareFantasyPlayers.value = [...selectedFantasyPlayer.value];
            clearSelectedFantasyPlayers();
        }

        const playerFantasyStatsIndexed = computed(() => {
            return playerFantasyStats.value
                .map((player, index) => ({
                    ...player,
                    position: index + 1
                })).filter(item =>
                    Object.values(item).some(val => stringifyNested(val).toLowerCase().includes(fantasyFilter.value.toLowerCase())
                    ));
        });

        const fantasyLeagueMetadataStatsIndexed = computed(() => {
            return fantasyLeagueMetadataStats.value
                .map((player, index) => ({
                    ...player,
                    position: index + 1
                })).filter(item =>
                    Object.values(item).some(val => stringifyNested(val).toLowerCase().includes(leagueFilter.value.toLowerCase())
                    ));
        });

        watch(() => leagueStore.selectedLeague, (newValue) => {
            if (newValue) {
                if (leagueStore.selectedLeague) {
                    localApiService.getPlayerFantasyStats(leagueStore.selectedLeague.id)
                        .then(result => playerFantasyStats.value = result);
                    localApiService.getFantasyLeagueMetadataStats(leagueStore.selectedLeague.id)
                        .then(result => fantasyLeagueMetadataStats.value = result);
                }
            }
        });

        return {
            authStore,
            leagueStore,
            tab,
            fantasyTab,
            fantasyFilter,
            leagueTab,
            leagueCompareTab,
            leagueFilter,
            compareOn,
            leaguePagination,
            playerFantasyStats,
            playerFantasyStatsIndexed,
            fantasyLeagueMetadataStats,
            fantasyLeagueMetadataStatsIndexed,
            selectedLeaguePlayer,
            compareLeaguePlayers,
            selectLeagueRow,
            CompareLeaguePlayers,
            clearSelectedLeaguePlayers,
            selectedFantasyPlayer,
            compareFantasyPlayers,
            selectFantasyRow,
            CompareFantasyPlayers,
            clearSelectedFantasyPlayers,
            selectedFantasyColumns,
            commonFantasyColumns,
            kdaFantasyColumns,
            farmFantasyColumns,
            supportFantasyColumns,
            damageHealingFantasyColumns,
            commonLeagueColumns,
            kdaLeagueColumns,
            farmLeagueColumns,
            supportLeagueColumns,
            damageHealingLeagueColumns,
            showFantasyFilters,
            isDesktop
        }
    },
    computed: {
        displayedFantasyColumns() {
            if (this.isDesktop) {
                return [...this.commonFantasyColumns, ...this.kdaFantasyColumns, ...this.farmFantasyColumns, ...this.supportFantasyColumns, ...this.damageHealingFantasyColumns];
            }
            else {
                switch (this.fantasyTab) {
                    case 'kda':
                        return [...this.commonFantasyColumns, ...this.kdaFantasyColumns];
                    case 'farm':
                        return [...this.commonFantasyColumns, ...this.farmFantasyColumns];
                    case 'support':
                        return [...this.commonFantasyColumns, ...this.supportFantasyColumns];
                    case 'damageHealing':
                        return [...this.commonFantasyColumns, ...this.damageHealingFantasyColumns];
                    default:
                        return [...this.commonFantasyColumns];
                }
            }
        },
        displayedLeagueColumns() {
            if (this.isDesktop) {
                return [...this.commonLeagueColumns, ...this.kdaLeagueColumns, ...this.farmLeagueColumns, ...this.supportLeagueColumns, ...this.damageHealingLeagueColumns];
            }
            else {
                switch (this.leagueTab) {
                    case 'kda':
                        return [...this.commonLeagueColumns, ...this.kdaLeagueColumns];
                    case 'farm':
                        return [...this.commonLeagueColumns, ...this.farmLeagueColumns];
                    case 'support':
                        return [...this.commonLeagueColumns, ...this.supportLeagueColumns];
                    case 'damageHealing':
                        return [...this.commonLeagueColumns, ...this.damageHealingLeagueColumns];
                    default:
                        return [...this.commonLeagueColumns];
                }
            }
        }
    },
    methods: {
        getPositionIcon(positionInt) {
            return `icons/pos_${positionInt}.png`
        }
    }
}
</script>
  

<style lang="sass">
.fantasy-stats-table
  /* height or max-height is important */

  .q-table__top,
  .q-table__bottom,
  thead tr:first-child th
    /* bg color is important for th; just specify one */
    background-color: #1D1D1D

  thead tr th
    position: sticky
    z-index: 1
  thead tr:first-child th
    top: 0

  /* this is when the loading indicator appears */
  &.q-table--loading thead tr:last-child th
    /* height of all previous header rows */
    top: 48px

  /* prevent scrolling behind sticky top row on focus */
  tbody
    /* height of all previous header rows */
    scroll-margin-top: 24px
</style>
  <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.debug {
    border: 1px solid red;
}

thead th tr {
    position: sticky;
}

.compare-footer {
    position: fixed;
    bottom: 0;
    left: 0;
    right: 0;
    background: var(--nadcl-main-2);
}

.left-fixed {
    flex: 0 0 300px;
}

.flex-container {
    display: flex;
    flex-flow: row wrap;
    max-width: 100%;
}

.flex-break {
    flex: 1 0 100% !important
}

.row {
    width: 100%;
}

.row .flex-break {
    height: 0 !important
}

.column .flex-break {
    width: 0 !important
}

.collapse-transition {
    transition: height 1s ease;
    overflow: hidden;
    height: 100%;
}

.collapsed {
    height: 0px;
}</style>