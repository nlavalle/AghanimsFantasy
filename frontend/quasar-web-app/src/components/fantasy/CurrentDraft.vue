<template>
    <div v-if="FantasyPoints.fantasyDraft" class="flex-container">
        <div class="row justify-center">
            <span class="text-white current-draft-header">Current Draft</span>
        </div>
        <div class="row justify-evenly">
            <draft-pick-card class="col-2 draft-card" v-for="fantasyDraftPoints in CombinedFantasyDraftPoints" :key="fantasyDraftPoints.id"
                :name="fantasyDraftPoints.name"
                :team="fantasyDraftPoints.team" 
                :fantasyPoints="fantasyDraftPoints.fantasyPoints"
                :role="fantasyDraftPoints.role" 
                :description="fantasyDraftPoints.description"
                :playerImageSrc="fantasyDraftPoints.playerImageSrc"
                :teamImageSrc="fantasyDraftPoints.teamImageSrc" />
        </div>
    </div>
</template>

<script>
import { defineComponent } from 'vue';
import DraftPickCard from 'src/components/fantasy/DraftPickCard.vue';

export default defineComponent({
    name: 'CurrentDraft',
    components: {
        DraftPickCard,
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
                    role: "Core",
                    description: "Stuff",
                    playerImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
                    teamImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.teamId ?? 0,
                },
                {
                    id: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.id,
                    name: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
                    team: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.team.name ?? "",
                    fantasyPoints: this.FantasyPoints.draftPickTwoPoints ?? 0,
                    role: "Core",
                    description: "Stuff",
                    playerImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
                    teamImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.teamId ?? 0,
                },
                {
                    id: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.id,
                    name: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
                    team: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.team.name ?? "",
                    fantasyPoints: this.FantasyPoints.draftPickThreePoints ?? 0,
                    role: "Core",
                    description: "Stuff",
                    playerImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
                    teamImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.teamId ?? 0,
                },
                {
                    id: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.id,
                    name: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
                    team: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.team.name ?? "",
                    fantasyPoints: this.FantasyPoints.draftPickFourPoints ?? 0,
                    role: "Core",
                    description: "Stuff",
                    playerImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
                    teamImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.teamId ?? 0,
                },
                {
                    id: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.id,
                    name: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.dotaAccount.name ?? "none",
                    team: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.team.name ?? "",
                    fantasyPoints: this.FantasyPoints.draftPickFivePoints ?? 0,
                    role: "Core",
                    description: "Stuff",
                    playerImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
                    teamImageSrc: this.FantasyPoints.fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.teamId ?? 0,
                },
            ];
        }
    },
    methods: {
        getTeamLogo(player) {
            return `~assets/logos/teams_logo_${player.teamId}.png`;
        },
    }
});
</script>

<style scoped>
.draft-card {
    min-width: 300px;
}

.debug {
    border: 1px solid red;
    padding: 10px;
}

.current-draft-header {
    font-size: 28px;
}

.flex-container {
    display: flex;
    flex-flow: row wrap;
    max-width: 100%;
    padding: 10px;
}

.flex-break {
    flex: 1 0 100% !important
}

.row {
    width: 100%;
}

.row .flex-break {
    height: 0 !important
}

.column .flex-break {
    width: 0 !important
}
</style>