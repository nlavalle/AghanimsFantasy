<template>
    <v-row class="gallery" style="max-width:700px">
        <v-col v-for="index in 5" :key="index" class="gallery-item">
            <v-row v-if="fantasyDraftPicks[index]" @click="changeActiveDraftPlayer(index)">
                <v-col>
                    <v-row class="parallelogram" justify="center"
                        :style="{ width: isDesktop ? '120px' : '60px', height: isDesktop ? '80px' : '40px', 'margin-left': isDesktop ? '5px' : '' }"
                        :class="{ 'glow-active-slot': currentActiveDraftPlayerCheck(index) }">
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
                        :style="{ 'min-height': isDesktop ? '1.5rem' : '1.2rem', 'max-width': isDesktop ? '120px' : '60px' }"></v-row>
                </v-col>
            </v-row>
        </v-col>
    </v-row>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { fantasyDraftState } from '../fantasyDraft';
import { VRow, VCol } from 'vuetify/components';

const isDesktop = ref(window.outerWidth >= 600);

const { selectedPlayer, currentDraftSlotSelected, fantasyDraftPicks } = fantasyDraftState();

const changeActiveDraftPlayer = (activeDraftPlayerSlot: number) => {
    currentDraftSlotSelected.value = activeDraftPlayerSlot;
    selectedPlayer.value = fantasyDraftPicks.value[activeDraftPlayerSlot];
}

const currentActiveDraftPlayerCheck = (draftSlot: number) => {
    return draftSlot == currentDraftSlotSelected.value;
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
    /* Gold border color */
    box-shadow: 0 0 5px #ffd700, 0 0 10px #ffd700, 0 0 15px #ffd700, 0 0 20px #ffd700;
    border-radius: 10px;
    /* Optional: rounded corners */
}

.caption {
    font-size: 1em;
    color: #AAA;
    padding-left: 20px;
    text-overflow: ellipsis;
    white-space: nowrap;
    overflow: hidden;
}
</style>