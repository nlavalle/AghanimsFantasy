<template>
    <v-container v-if="FantasyPoints.fantasyDraft">
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

<script>
import { defineComponent } from 'vue';
import DraftPickCard from '@/components/Fantasy/DraftPickCard.vue';
import { VContainer, VRow } from 'vuetify/components';

export default defineComponent({
    name: 'CurrentDraft',
    components: {
        DraftPickCard,
        VContainer,
        VRow
    },
    props: {
        FantasyPoints: {
            type: Object,
            required: false,
        }
    },
    computed: {
        CombinedFantasyDraftPoints() {
            return [
                {
                    id: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.id,
                    name: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
                    team: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.team.name ?? "",
                    fantasyPoints: this.FantasyPoints.draftPickOnePoints ?? 0,
                    role: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.teamPosition ?? 0,
                    description: "Stuff",
                    playerImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
                    teamImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.teamId ?? 0,
                },
                {
                    id: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.id,
                    name: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
                    team: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.team.name ?? "",
                    fantasyPoints: this.FantasyPoints.draftPickTwoPoints ?? 0,
                    role: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.teamPosition ?? 0,
                    description: "Stuff",
                    playerImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
                    teamImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.teamId ?? 0,
                },
                {
                    id: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.id,
                    name: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
                    team: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.team.name ?? "",
                    fantasyPoints: this.FantasyPoints.draftPickThreePoints ?? 0,
                    role: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.teamPosition ?? 0,
                    description: "Stuff",
                    playerImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
                    teamImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.teamId ?? 0,
                },
                {
                    id: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.id,
                    name: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
                    team: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.team.name ?? "",
                    fantasyPoints: this.FantasyPoints.draftPickFourPoints ?? 0,
                    role: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.teamPosition ?? 0,
                    description: "Stuff",
                    playerImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
                    teamImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.teamId ?? 0,
                },
                {
                    id: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.id,
                    name: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
                    team: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.team.name ?? "",
                    fantasyPoints: this.FantasyPoints.draftPickFivePoints ?? 0,
                    role: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.teamPosition ?? 0,
                    description: "Stuff",
                    playerImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
                    teamImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.teamId ?? 0,
                },
            ];
        }
    }
});
</script>

<style scoped>
.draft-card {
    min-width: 300px;
}

.current-draft-header {
    font-size: 28px;
}
</style>