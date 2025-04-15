<template>
    <v-container v-if="FantasyPoints?.fantasyDraft">
        <v-row v-if="fantasyLeagueStore.fantasyPlayersStats" class="mt-1 align-center">
            <total-winnings class="totalWinnings" />
        </v-row>
        <v-row justify="space-evenly">
            <v-col class="ma-1 pa-1" v-for="(fantasyDraftPoints, index) in CombinedFantasyDraftPoints" :key="index">
                <v-row class="mt-1" justify="center">
                    <draft-pick-card-hover v-if="fantasyDraftPoints.fantasyPlayer" class="draft-pick-card"
                        :fantasyPlayer="fantasyDraftPoints.fantasyPlayer"
                        :fantasyPoints="fantasyDraftPoints.fantasyPoints" />
                </v-row>
            </v-col>
        </v-row>
        <v-row class="mt-10" v-if="!fantasyLeagueStore.isDraftOpen(fantasyLeagueStore.selectedFantasyLeague)">
            <player-winnings class="playerWinnings" :selectedDraft="fantasyLeagueStore.selectedFantasyDraftPoints" />
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { VContainer, VRow, VCol } from 'vuetify/components';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import DraftPickCardHover from '@/components/Fantasy/DraftPickCardHover.vue';
import PlayerWinnings from '@/components/Fantasy/Winnings/PlayerWinnings.vue';
import TotalWinnings from '@/components/Fantasy/Winnings/TotalWinnings.vue';

const props = defineProps({
    FantasyPoints: {
        type: Object,
        required: false,
    }
})

const fantasyLeagueStore = useFantasyLeagueStore();

const CombinedFantasyDraftPoints = computed(() => {
    return [
        {
            fantasyPlayer: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 1)[0]?.fantasyPlayer,
            fantasyPoints: props.FantasyPoints?.draftPickOnePoints ?? 0,
        },
        {
            fantasyPlayer: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 2)[0]?.fantasyPlayer,
            fantasyPoints: props.FantasyPoints?.draftPickTwoPoints ?? 0,
        },
        {
            fantasyPlayer: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 3)[0]?.fantasyPlayer,
            fantasyPoints: props.FantasyPoints?.draftPickThreePoints ?? 0,
        },
        {
            fantasyPlayer: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 4)[0]?.fantasyPlayer,
            fantasyPoints: props.FantasyPoints?.draftPickFourPoints ?? 0,
        },
        {
            fantasyPlayer: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 5)[0]?.fantasyPlayer,
            fantasyPoints: props.FantasyPoints?.draftPickFivePoints ?? 0,
        },
    ];
})

</script>

<style scoped>
.draft-pick-card {
    /* max-height: 350px; */
    height: 300px;
    margin: 20px;
}

.totalWinnings {
    max-width: 300px;
}

.playerWinnings {
    max-width: 600px;
}
</style>