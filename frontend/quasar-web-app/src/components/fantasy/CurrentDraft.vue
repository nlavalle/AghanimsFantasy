<template>
    <div v-if="FantasyDraft.length ?? 0 > 0" class="flex-container">
        <div class="row justify-center">
            <span class="text-white current-draft-header">Current Draft</span>
        </div>
        <div class="row justify-evenly">
            <draft-pick-card class="col-2 draft-card" :name="FantasyDraft[0].players[0].dotaAccount.name"
                :team="FantasyDraft[0].players[0].team.name" :fantasyPoints="FantasyPoints.draftPickOnePoints ?? 0"
                role="Core" description="Stuff" :playerImageSrc="FantasyDraft[0].players[0].dotaAccount.steamProfilePicture"
                :teamImageSrc="FantasyDraft[0].players[0].teamId" />
            <draft-pick-card class="col-2 draft-card" :name="FantasyDraft[0].players[1].dotaAccount.name"
                :team="FantasyDraft[0].players[1].team.name" :fantasyPoints="FantasyPoints.draftPickTwoPoints ?? 0"
                role="Core" description="Stuff" :playerImageSrc="FantasyDraft[0].players[1].dotaAccount.steamProfilePicture"
                :teamImageSrc="FantasyDraft[0].players[1].teamId" />
            <draft-pick-card class="col-2 draft-card" :name="FantasyDraft[0].players[2].dotaAccount.name"
                :team="FantasyDraft[0].players[2].team.name" :fantasyPoints="FantasyPoints.draftPickThreePoints ?? 0"
                role="Core" description="Stuff" :playerImageSrc="FantasyDraft[0].players[2].dotaAccount.steamProfilePicture"
                :teamImageSrc="FantasyDraft[0].players[2].teamId" />
            <draft-pick-card class="col-2 draft-card" :name="FantasyDraft[0].players[3].dotaAccount.name"
                :team="FantasyDraft[0].players[3].team.name" :fantasyPoints="FantasyPoints.draftPickFourPoints ?? 0"
                role="Core" description="Stuff" :playerImageSrc="FantasyDraft[0].players[3].dotaAccount.steamProfilePicture"
                :teamImageSrc="FantasyDraft[0].players[3].teamId" />
            <draft-pick-card class="col-2 draft-card" :name="FantasyDraft[0].players[4].dotaAccount.name"
                :team="FantasyDraft[0].players[4].team.name" :fantasyPoints="FantasyPoints.draftPickFivePoints ?? 0"
                role="Core" description="Stuff" :playerImageSrc="FantasyDraft[0].players[4].dotaAccount.steamProfilePicture"
                :teamImageSrc="FantasyDraft[0].players[4].teamId" />
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
        FantasyDraft: {
            type: Array,
            required: false,
        },
        FantasyPoints: {
            type: Object,
            required: false,
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