<template>
    <div>
        <!-- Set up the sticky top objects -->
        <div v-if="isDesktop" style="margin-top:40px">
            <div class="sticky-parent right-0 d-flex">
                <div class="sticky-child" style="min-width: 100px;">
                    <span class="sticky-child gold d-flex">Budget:
                        <GoldSpan :font-size="1.0" :gold-value="'600'" />
                    </span>
                    <span :class="{ 'sticky-child': true, 'd-flex': true, 'gold': true, 'invalid': totalGold > 600 }"
                        style="top: 25px;">Cost:
                        <GoldSpan :font-size="1.0" :validation="totalGold > 600" :gold-value="totalGold.toFixed(0)" />
                    </span>
                </div>
                <v-btn class="btn-fantasy sticky-child" @click="clearDraft()">Clear Draft</v-btn>
                <v-btn
                    v-if="authenticated && leagueStore.selectedFantasyLeague && leagueStore.isDraftOpen(leagueStore.selectedFantasyLeague!) && totalGold <= 600"
                    class="btn-fantasy sticky-child" @click="saveDraft()">Save Draft</v-btn>
                <v-btn v-else class="btn-fantasy sticky-child" style="pointer-events: none;" disabled
                    @click="saveDraft()">Save Draft</v-btn>
            </div>
            <div class="sticky-parent left-0">
                <CreateDraftPicks class="sticky-child" style="z-index:10" />
            </div>
        </div>
        <div v-else style="margin-top:20px">
            <div class="sticky-parent left-0">
                <div class="sticky-child d-flex">
                    <div class="sticky-child mt-3" style="min-width: 60px;">
                        <span class="sticky-child gold d-flex" style="font-size:0.8rem">Budget:
                            <GoldSpan :font-size="0.8" :gold-value="'600'" />
                        </span>
                        <span
                            :class="{ 'sticky-child': true, 'd-flex': true, 'gold': true, 'invalid': totalGold > 600 }"
                            style="top: 25px;font-size:0.8rem">Cost:
                            <GoldSpan :font-size="0.8" :validation="totalGold > 600"
                                :gold-value="totalGold.toFixed(0)" />
                        </span>
                    </div>
                    <v-btn class="btn-fantasy sticky-child" style="top:10px;font-size:0.7rem"
                        @click="clearDraft()">Clear
                        Draft</v-btn>
                    <v-btn
                        v-if="authenticated && leagueStore.selectedFantasyLeague && leagueStore.isDraftOpen(leagueStore.selectedFantasyLeague!) && totalGold <= 600"
                        class="btn-fantasy sticky-child" style="top:10px;font-size:0.7rem" @click="saveDraft()">Save
                        Draft</v-btn>
                    <v-btn v-else class="btn-fantasy sticky-child"
                        style="top:20px;font-size:0.7rem; pointer-events: none;" disabled @click="saveDraft()">Save
                        Draft</v-btn>
                </div>
                <CreateDraftPicks class="mt-0 sticky-child" style="top:70px;z-index:10" @click="toggleDrawer" />
            </div>
        </div>
        <!-- Set up collapsible (on mobile) player stats sidebar -->
        <div v-if="isDesktop">
            <div class="sticky-parent right-0" style="width:450px; margin-top:100px;">
                <PlayerStats class="sticky-child" style="height:600px;top:50px" />
            </div>
        </div>
        <div v-else>
            <v-navigation-drawer v-model="mobileDrawer" temporary location="right" :width="300">
                <PlayerStats @save-player="toggleDrawer" />
            </v-navigation-drawer>
        </div>


        <div v-if="isDesktop" style="margin-top:120px;margin-right:450px;min-height:620px;">
            <PlayerPicksAvailable class="picks-available" />
        </div>
        <div v-else style="margin-top:140px;">
            <PlayerPicksAvailable class="picks-available" @click="toggleDrawer" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { VBtn, VNavigationDrawer } from 'vuetify/components';
import CreateDraftPicks from './CreateDraftPicks.vue';
import PlayerPicksAvailable from './PlayerPicksAvailable.vue';
import PlayerStats from './PlayerStats.vue';
import GoldSpan from '@/components/Dom/GoldSpan.vue';
import { useAuthStore } from '@/stores/auth';
import { fantasyDraftState } from '../fantasyDraft';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';

const isDesktop = ref(window.outerWidth >= 600);

const authStore = useAuthStore();

const mobileDrawer = ref(false);

const leagueStore = useFantasyLeagueStore();
const { selectedPlayer, fantasyDraftPicks, fantasyPlayerPointsAvailable, setFantasyPlayerPoints, clearFantasyDraftPicks } = fantasyDraftState();

onMounted(() => {
    if (fantasyPlayerPointsAvailable.value.length == 0) {
        leagueStore.fetchFantasyPlayerPoints()?.then(() => setFantasyPlayerPoints(leagueStore.fantasyPlayerPoints))
    }
    if (leagueStore.fantasyPlayersStats.length == 0) {
        leagueStore.fetchFantasyPlayerViewModels();
    }
})

watch(() => leagueStore.selectedFantasyLeague, () => {
    selectedPlayer.value = undefined;
})

const emit = defineEmits(['saveDraft']);

const saveDraft = () => {
    emit('saveDraft');
}

const clearDraft = () => {
    clearFantasyDraftPicks();
}

const toggleDrawer = () => {
    mobileDrawer.value = !mobileDrawer.value;
}

const authenticated = computed(() => {
    return authStore.authenticated
})

const totalGold = computed(() => {
    return fantasyDraftPicks.value.reduce(function (accumulator, increment) {
        return accumulator + (leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.dotaAccountId == increment.dotaAccountId)?.cost ?? 0)
    }, 0)
})

</script>

<style scoped>
.btn-fantasy {
    color: var(--aghanims-fantasy-white);
    background-color: var(--aghanims-fantasy-main-2);
    margin: 10px;
    height: 40px;
}

.sticky-parent {
    position: absolute;
    height: 90%;
    pointer-events: none;
    z-index: 20;
}

.sticky-child {
    position: sticky;
    overflow: visible !important;
    top: 5px;
    pointer-events: auto;
}

.gold {
    color: rgb(249, 194, 43);
    text-overflow: ellipsis;
    white-space: nowrap;
    overflow: hidden;
}

.invalid {
    color: red;
}
</style>