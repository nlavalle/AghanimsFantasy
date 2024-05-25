<template>
    <v-container v-if="FantasyPoints?.fantasyDraft">
        <v-row justify="space-evenly">
            <v-col class="ma-1 pa-1" v-for="fantasyDraftPoints in CombinedFantasyDraftPoints"
                :key="fantasyDraftPoints.id">
                <draft-pick-card :name="fantasyDraftPoints.name" :team="fantasyDraftPoints.team"
                    :fantasyPoints="fantasyDraftPoints.fantasyPoints" :role="fantasyDraftPoints.role"
                    :description="fantasyDraftPoints.description" :playerImageSrc="fantasyDraftPoints.playerImageSrc"
                    :teamImageSrc="fantasyDraftPoints.teamImageSrc" />
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import DraftPickCard from '@/components/Fantasy/DraftPickCard.vue';
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

<style scoped></style>