<template>
    <v-row>
        <v-col>
            <div class="gallery">
                <div v-for="index in 5" :key="index" class="gallery-item">
                    <div v-if="fantasyDraftPicks[index]" @click="changeActiveDraftPlayer(index)">
                        <div class="parallelogram"
                            :class="{ 'glow-active-slot': currentActiveDraftPlayerCheck(index) }">
                            <img :src="fantasyDraftPicks[index].dotaAccount.steamProfilePicture"
                                :alt="fantasyDraftPicks[index].dotaAccount.name" />
                        </div>
                        <div class="caption">{{ fantasyDraftPicks[index].dotaAccount.name }}</div>
                    </div>
                    <div v-else @click="changeActiveDraftPlayer(index)">
                        <div class="parallelogram"
                            :class="{ 'glow-active-slot': currentActiveDraftPlayerCheck(index) }">
                        </div>
                    </div>
                </div>
            </div>
        </v-col>
        <v-col>
            <v-row justify="center">
                <!-- <v-spacer /> -->
                <v-btn class="mr-10 bg-primary" @click="clearDraft">Clear Draft</v-btn>
            </v-row>
        </v-col>
    </v-row>

</template>

<script setup lang="ts">
import { fantasyDraftState } from '../fantasyDraft';
import { VBtn, VRow, VCol } from 'vuetify/components';

const { currentDraftSlotSelected, fantasyDraftPicks, clearFantasyDraftPicks } = fantasyDraftState();

const changeActiveDraftPlayer = (activeDraftPlayerSlot: number) => {
    currentDraftSlotSelected.value = activeDraftPlayerSlot;
}

const currentActiveDraftPlayerCheck = (draftSlot: number) => {
    return draftSlot == currentDraftSlotSelected.value;
}

const clearDraft = () => {
    clearFantasyDraftPicks();
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
    width: 120px;
    height: 80px;
    overflow: hidden;
    transform: skew(20deg);
    margin-bottom: 5px;
    margin-left: 10px;
    border: 1px solid white;
}

.parallelogram img {
    height: 100%;
    object-fit: cover;
    transform: skew(-20deg);
}

.glow-active-slot {
    border: 2px solid #ffd700;
    /* Gold border color */
    box-shadow: 0 0 5px #ffd700, 0 0 10px #ffd700, 0 0 15px #ffd700, 0 0 20px #ffd700;
    border-radius: 10px;
    /* Optional: rounded corners */
}

.caption {
    font-size: 1em;
    color: #AAA;
    padding-left: 20px;
}
</style>