<template>
    <v-row class="gallery" style="max-width:700px">
        <v-col v-for="index in 5" :key="index" class="gallery-item">
            <v-row v-if="fantasyDraftPicks[index]" @click="changeActiveDraftPlayer(index)">
                <v-col>
                    <v-row v-if="!isDesktop">
                        <GoldSpan :gold-value="getPlayerCost(fantasyDraftPicks[index].dotaAccount.id).toString()" />
                    </v-row>
                    <v-row class="parallelogram" justify="center"
                        :style="{ width: isDesktop ? '120px' : '60px', height: isDesktop ? '80px' : '40px', 'margin-left': isDesktop ? '5px' : '' }"
                        :class="{ 'glow-active-slot': currentActiveDraftPlayerCheck(index) }">
                        <GoldSpan v-if="isDesktop" class="gold"
                            :gold-value="getPlayerCost(fantasyDraftPicks[index].dotaAccount.id).toString()" />
                        <img :src="fantasyDraftPicks[index].dotaAccount.steamProfilePicture"
                            :alt="fantasyDraftPicks[index].dotaAccount.name" />
                    </v-row>
                    <v-row class="caption" justify="center"
                        :style="{ 'font-size': isDesktop ? '1rem' : '0.8rem', 'max-width': isDesktop ? '120px' : '60px' }">{{
                            fantasyDraftPicks[index].dotaAccount.name }}</v-row>
                </v-col>
            </v-row>
            <v-row v-else @click="changeActiveDraftPlayer(index)">
                <v-col>
                    <v-row class="parallelogram" justify="center" align="center"
                        :style="{ 'max-width': isDesktop ? '120px' : '60px', 'min-width': isDesktop ? '120px' : '60px', 'min-height': isDesktop ? '80px' : '40px', 'margin-left': isDesktop ? '5px' : '' }"
                        :class="{ 'glow-active-slot': currentActiveDraftPlayerCheck(index) }">
                        <img :style="{ width: isDesktop ? '70px' : '20px', height: isDesktop ? '70px' : '20px' }"
                            :src="`icons/pos_${index}.png`" />
                    </v-row>
                    <v-row class="caption" justify="center"
                        :style="{ 'min-height': isDesktop ? '1.5rem' : '1.2rem', 'max-width': isDesktop ? '120px' : '60px', 'padding-left': isDesktop ? '30px' : '10px' }"></v-row>
                </v-col>
            </v-row>
        </v-col>
    </v-row>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { fantasyDraftState } from '../fantasyDraft';
import { VRow, VCol } from 'vuetify/components';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import GoldSpan from '@/components/Dom/GoldSpan.vue';

const isDesktop = ref(window.outerWidth >= 600);

const { selectedPlayer, currentDraftSlotSelected, fantasyDraftPicks } = fantasyDraftState();
const leagueStore = useFantasyLeagueStore();

const changeActiveDraftPlayer = (activeDraftPlayerSlot: number) => {
    currentDraftSlotSelected.value = activeDraftPlayerSlot;
    selectedPlayer.value = fantasyDraftPicks.value[activeDraftPlayerSlot];
}

const currentActiveDraftPlayerCheck = (draftSlot: number) => {
    return draftSlot == currentDraftSlotSelected.value;
}

const getPlayerCost = (accountId: number) => {
    return leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.dotaAccountId == accountId)?.cost.toFixed(0) ?? 0;
}

</script>

<style scoped>
.gallery {
    display: flex;
    justify-content: space-evenly;
    align-items: center;
    margin-left: 10px;
}

.gallery-item {
    text-align: center;
}

.parallelogram {
    position: relative;
    overflow: hidden;
    transform: skew(20deg);
    border: 1px solid white;
    background: radial-gradient(at center, #474747 10%, #323232 90%);
}

.parallelogram img {
    height: 100%;
    object-fit: cover;
    transform: skew(-20deg);
}

.glow-active-slot {
    border: 1px solid #ffd700;
    box-shadow: 0 0 5px #ffd700, 0 0 10px #ffd700, 0 0 15px #ffd700, 0 0 20px #ffd700;
    border-radius: 10px;
}

.caption {
    font-size: 1em;
    color: #AAA;
    text-overflow: ellipsis;
    white-space: nowrap;
    overflow: hidden;
}

.gold {
    position: absolute;
    top: 5px;
    left: 10px;
    transform: skew(-20deg);
    overflow: hidden;
    z-index: 2;
}
</style>