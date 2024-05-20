<template>
    <v-container v-if="FantasyPoints?.fantasyDraft">
        <v-row class="d-flex justify-center">
            <span class="text-white current-draft-header">Current Draft</span>
        </v-row>
        <v-row class="d-flex justify-space-evenly" style="flex-flow:row wrap">
            <draft-pick-card class="col-2 draft-card" v-for="fantasyDraftPoints in CombinedFantasyDraftPoints"
                :key="fantasyDraftPoints.id" :name="fantasyDraftPoints.name" :team="fantasyDraftPoints.team"
                :fantasyPoints="fantasyDraftPoints.fantasyPoints" :role="fantasyDraftPoints.role"
                :description="fantasyDraftPoints.description" :playerImageSrc="fantasyDraftPoints.playerImageSrc"
                :teamImageSrc="fantasyDraftPoints.teamImageSrc" />
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
import { computed, defineProps } from 'vue';
import DraftPickCard from '@/components/Fantasy/DraftPickCard.vue';
import { VContainer, VRow } from 'vuetify/components';

const props = defineProps({
    FantasyPoints: {
        type: Object,
        required: false,
    }
})

const CombinedFantasyDraftPoints = computed(() => {
    return [
        {
            id: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 1)[0]?.fantasyPlayer?.id,
            name: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 1)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
            team: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 1)[0]?.fantasyPlayer?.team.name ?? "",
            fantasyPoints: props.FantasyPoints?.draftPickOnePoints ?? 0,
            role: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 1)[0]?.fantasyPlayer?.teamPosition ?? 0,
            description: "Stuff",
            playerImageSrc: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 1)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
            teamImageSrc: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 1)[0]?.fantasyPlayer?.teamId ?? 0,
        },
        {
            id: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 2)[0]?.fantasyPlayer?.id,
            name: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 2)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
            team: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 2)[0]?.fantasyPlayer?.team.name ?? "",
            fantasyPoints: props.FantasyPoints?.draftPickTwoPoints ?? 0,
            role: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 2)[0]?.fantasyPlayer?.teamPosition ?? 0,
            description: "Stuff",
            playerImageSrc: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 2)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
            teamImageSrc: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 2)[0]?.fantasyPlayer?.teamId ?? 0,
        },
        {
            id: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 3)[0]?.fantasyPlayer?.id,
            name: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 3)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
            team: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 3)[0]?.fantasyPlayer?.team.name ?? "",
            fantasyPoints: props.FantasyPoints?.draftPickThreePoints ?? 0,
            role: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 3)[0]?.fantasyPlayer?.teamPosition ?? 0,
            description: "Stuff",
            playerImageSrc: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 3)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
            teamImageSrc: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 3)[0]?.fantasyPlayer?.teamId ?? 0,
        },
        {
            id: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 4)[0]?.fantasyPlayer?.id,
            name: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 4)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
            team: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 4)[0]?.fantasyPlayer?.team.name ?? "",
            fantasyPoints: props.FantasyPoints?.draftPickFourPoints ?? 0,
            role: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 4)[0]?.fantasyPlayer?.teamPosition ?? 0,
            description: "Stuff",
            playerImageSrc: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 4)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
            teamImageSrc: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 4)[0]?.fantasyPlayer?.teamId ?? 0,
        },
        {
            id: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 5)[0]?.fantasyPlayer?.id,
            name: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 5)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
            team: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 5)[0]?.fantasyPlayer?.team.name ?? "",
            fantasyPoints: props.FantasyPoints?.draftPickFivePoints ?? 0,
            role: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 5)[0]?.fantasyPlayer?.teamPosition ?? 0,
            description: "Stuff",
            playerImageSrc: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 5)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
            teamImageSrc: props.FantasyPoints?.fantasyDraft.draftPickPlayers.filter((dpp: any) => dpp.draftOrder == 5)[0]?.fantasyPlayer?.teamId ?? 0,
        },
    ];
})

</script>

<style scoped>
.draft-card {
    min-width: 300px;
}

.current-draft-header {
    font-size: 28px;
}
</style>