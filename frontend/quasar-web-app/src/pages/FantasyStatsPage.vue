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
                        <div style="width:55%; max-width:300px; padding:10px">
                            <q-input v-model="fantasyFilter" debounce="500" color="red-13" label="Search" dense outlined>
                                <template v-slot:append>
                                    <q-icon name="search" />
                                </template>
                            </q-input>
                        </div>
                        <q-space />
                        <q-tabs v-if="!this.isDesktop" v-model="fantasyTab" dense class="text-grey-5" active-color="grey-1"
                            indicator-color="red-13" style="width:45%;margin-bottom:5px">
                            <q-tab name="kda" label="K/D/A" />
                            <q-tab name="farm" label="Farm" />
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
                        </q-table>
                    </div>
                </q-tab-panel>
                <q-tab-panel name="league" style="padding: 0px">
                    <div class="row">
                        <div style="width:55%; max-width:300px; padding:10px">
                            <q-input v-model="leagueFilter" debounce="500" color="red-13" label="Search" dense outlined>
                                <template v-slot:append>
                                    <q-icon name="search" />
                                </template>
                            </q-input>
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
                    <div class="row">
                        <q-table class="league-stats-table" dense :columns="displayedLeagueColumns"
                            :rows="fantasyLeagueMetadataStatsIndexed" virtual-scroll :rows-per-page-options="[0]"
                            separator="vertical" style="width:100%">
                            <template v-slot:body-cell-leaguePlayer="props">
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
                                            </div>
                                        </div>
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalKills="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.kills }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalDeaths="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.deaths }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalAssists="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.assists }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalLastHits="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.lastHits }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalDenies="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.denies }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalGoldPerMin="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.goldPerMin }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalXpPerMin="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.xpPerMin }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalSupportGoldSpent="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        <span :style="{fontSize: isDesktop ? '1em' : '0.85em'}">
                                            {{ props.value.supportGoldSpent.toLocaleString() }}
                                        </span>
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalObsPlaced="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.observerWardsPlaced }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalSentriesPlaced="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.sentryWardsPlaced }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalWardsDewarded="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.wardsDewarded }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalCampsStacked="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.campsStacked }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalHeroDamage="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.heroDamage.toLocaleString() }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalTowerDamage="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.towerDamage.toLocaleString() }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalHeroHealing="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.heroHealing.toLocaleString() }}
                                    </div>
                                </q-td>
                            </template>
                            <template v-slot:body-cell-totalStunDuration="props">
                                <q-td :props="props">
                                    <div style="white-space:normal">
                                        {{ props.value.stunDuration.toFixed(1).toLocaleString() }}
                                    </div>
                                </q-td>
                            </template>
                        </q-table>
                    </div>
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
        const leagueFilter = ref('');
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
                sortable: true
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
                label: isDesktop.value ? 'Average Gold Per Min' : 'G',
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
                label: isDesktop.value ? 'Average XP Per Min' : 'XP',
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

        const commonLeagueColumns = [
            {
                name: 'leaguePlayer',
                label: 'Player/Team',
                align: 'left',
                field: row => {
                    return {
                        playerName: row.player.dotaAccount.name,
                        playerPicture: row.player.dotaAccount.steamProfilePicture,
                        teamName: row.player.team.name
                    };
                },
                style: 'width: 400px',
                sortable: true,
                sort: (a, b) => {
                    if (a.playerName > b.playerName) return 1;
                    if (a.playerName < b.playerName) return -1;
                }
            },
        ];
        const kdaLeagueColumns = [
            {
                name: 'totalKills',
                label: isDesktop.value ? 'Kills' : 'K',
                align: 'left',
                field: row => {
                    return {
                        kills: row.matchDetailsPlayers.kills,
                    };
                },
                sortable: true,
                sort: (a, b) => a.kills - b.kills
            },
            {
                name: 'totalDeaths',
                label: isDesktop.value ? 'Deaths' : 'D',
                align: 'left',
                field: row => {
                    return {
                        deaths: row.matchDetailsPlayers.deaths,
                    };
                },
                sortable: true,
                sort: (a, b) => a.deaths - b.deaths
            },
            {
                name: 'totalAssists',
                label: isDesktop.value ? 'Assists' : 'A',
                align: 'left',
                field: row => {
                    return {
                        assists: row.matchDetailsPlayers.assists,
                    };
                },
                sortable: true,
                sort: (a, b) => a.assists - b.assists
            },
        ];
        const farmLeagueColumns = [
            {
                name: 'totalLastHits',
                label: isDesktop.value ? 'Last Hits' : 'LH',
                align: 'left',
                field: row => {
                    return {
                        lastHits: row.matchDetailsPlayers.lastHits,
                    };
                },
                sortable: true,
                sort: (a, b) => a.lastHits - b.lastHits
            },
            {
                name: 'totalDenies',
                label: isDesktop.value ? 'Denies' : 'DN',
                align: 'left',
                field: row => {
                    return {
                        denies: row.matchDetailsPlayers.denies,
                    };
                },
                sortable: true,
                sort: (a, b) => a.denies - b.denies
            },
            {
                name: 'totalGoldPerMin',
                label: isDesktop.value ? 'Avg GPM' : 'G',
                align: 'left',
                field: row => {
                    return {
                        goldPerMin: row.matchDetailsPlayers.goldPerMin,
                    };
                },
                sortable: true,
                sort: (a, b) => a.goldPerMin - b.goldPerMin
            },
            {
                name: 'totalXpPerMin',
                label: isDesktop.value ? 'Avg XPM' : 'XP',
                align: 'left',
                field: row => {
                    return {
                        xpPerMin: row.matchDetailsPlayers.xpPerMin
                    };
                },
                sortable: true,
                sort: (a, b) => a.xpPerMin - b.xpPerMin
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
                field: row => {
                    return {
                        observerWardsPlaced: row.metadataPlayer?.observerWardsPlaced ?? 0,
                    };
                },
                sortable: true,
                sort: (a, b) => a.observerWardsPlaced - b.observerWardsPlaced
            },
            {
                name: 'totalSentriesPlaced',
                label: isDesktop.value ? 'Sentires Placed' : 'SN',
                align: 'left',
                field: row => {
                    return {
                        sentryWardsPlaced: row.metadataPlayer?.sentryWardsPlaced ?? 0,
                    };
                },
                sortable: true,
                sort: (a, b) => a.sentryWardsPlaced - b.sentryWardsPlaced
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
                field: row => {
                    return {
                        campsStacked: row.metadataPlayer?.campsStacked ?? 0
                    };
                },
                sortable: true,
                sort: (a, b) => a.campsStacked - b.campsStacked
            },
        ];
        const damageHealingLeagueColumns = [
            {
                name: 'totalHeroDamage',
                label: isDesktop.value ? 'Hero Dmg' : 'HD',
                align: 'left',
                field: row => {
                    return {
                        heroDamage: row.matchDetailsPlayers?.heroDamage ?? 0,
                    };
                },
                sortable: true,
                sort: (a, b) => a.heroDamage - b.heroDamage
            },
            {
                name: 'totalTowerDamage',
                label: isDesktop.value ? 'Tower Dmg' : 'TD',
                align: 'left',
                field: row => {
                    return {
                        towerDamage: row.matchDetailsPlayers?.towerDamage ?? 0,
                    };
                },
                sortable: true,
                sort: (a, b) => a.towerDamage - b.towerDamage
            },
            {
                name: 'totalHeroHealing',
                label: isDesktop.value ? 'Hero Healing' : 'HH',
                align: 'left',
                field: row => {
                    return {
                        heroHealing: row.matchDetailsPlayers?.heroHealing ?? 0,
                    };
                },
                sortable: true,
                sort: (a, b) => a.sentryWardsPlaced - b.sentryWardsPlaced
            },
            {
                name: 'totalStunDuration',
                label: isDesktop.value ? 'Stun Dur.' : 'SD',
                align: 'left',
                field: row => {
                    return {
                        stunDuration: row.metadataPlayer?.stunDuration ?? 0
                    };
                },
                sortable: true,
                sort: (a, b) => a.stunDuration - b.stunDuration
            },
        ];

        const selectedFantasyColumns = ref(commonFantasyColumns.map(column => column.name));

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
            leagueFilter,
            playerFantasyStats,
            playerFantasyStatsIndexed,
            fantasyLeagueMetadataStats,
            fantasyLeagueMetadataStatsIndexed,
            selectedFantasyColumns,
            commonFantasyColumns,
            kdaFantasyColumns,
            farmFantasyColumns,
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
                return [...this.commonFantasyColumns, ...this.kdaFantasyColumns, ...this.farmFantasyColumns];
            }
            else {
                switch (this.fantasyTab) {
                    case 'kda':
                        return [...this.commonFantasyColumns, ...this.kdaFantasyColumns];
                    case 'farm':
                        return [...this.commonFantasyColumns, ...this.farmFantasyColumns];
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

/* .fantasy-stats-table {
    width: 1400px;
    height: 800px;
} */

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
</style>