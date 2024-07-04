<template>
    <div>
        <!-- Set up the sticky top objects -->
        <div v-if="isDesktop" style="margin-top:40px">
            <div class="sticky-parent right-0">
                <v-btn class="btn-fantasy sticky-child" @click="clearDraft()">Clear Draft</v-btn>
                <v-btn class="btn-fantasy sticky-child" @click="saveDraft()">Save Draft</v-btn>
            </div>
            <div class="sticky-parent left-0">
                <CreateDraftPicks class="sticky-child" style="z-index:10" />
            </div>
        </div>
        <div v-else style="margin-top:20px">
            <div class="sticky-parent left-0">
                <v-btn class="btn-fantasy sticky-child" style="top:20px;" @click="clearDraft()">Clear Draft</v-btn>
                <v-btn class="btn-fantasy sticky-child" style="top:20px;" @click="saveDraft()">Save Draft</v-btn>
                <CreateDraftPicks class="mt-2 sticky-child" style="top:70px;z-index:10" />
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
                <PlayerStats />
            </v-navigation-drawer>
        </div>


        <div v-if="isDesktop" style="margin-top:120px;margin-right:450px;">
            <PlayerPicksAvailable class="picks-available" />
        </div>
        <div v-else style="margin-top:120px;">
            <PlayerPicksAvailable class="picks-available" @click="toggleDrawer" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { VBtn, VNavigationDrawer } from 'vuetify/components';
import CreateDraftPicks from './CreateDraftPicks.vue';
import PlayerPicksAvailable from './PlayerPicksAvailable.vue';
import PlayerStats from './PlayerStats.vue';
import { fantasyDraftState } from '../fantasyDraft';

const isDesktop = ref(window.outerWidth >= 600);

const mobileDrawer = ref(false);

const { clearFantasyDraftPicks } = fantasyDraftState();

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
}

.sticky-child {
    position: sticky;
    overflow: visible !important;
    top: 5px;
    pointer-events: auto;
}
</style>