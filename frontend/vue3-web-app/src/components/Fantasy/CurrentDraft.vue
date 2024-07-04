<template>
    <v-container v-if="FantasyPoints?.fantasyDraft">
        <v-row justify="space-evenly">
            <v-col class="ma-1 pa-1" v-for="(fantasyDraftPoints, index) in CombinedFantasyDraftPoints" :key="index">
                <v-row class="mt-1" justify="center">
                    <draft-pick-card-hover class="draft-pick-card" :fantasyPlayer="fantasyDraftPoints.fantasyPlayer"
                        :fantasyPoints="fantasyDraftPoints.fantasyPoints" />
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import DraftPickCardHover from '@/components/Fantasy/DraftPickCardHover.vue';
import { VContainer, VRow, VCol } from 'vuetify/components';

const props = defineProps({
    FantasyPoints: {
        type: Object,
        required: false,
    }
})

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
</style>