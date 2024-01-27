<template>
    <div class="league-container">
        <q-select class="league-selector" dense filled dark v-model="selectedLeague" :options="leagueOptions"
            @update:model-value="updateSelectedLeague" option-label="name" option-value="id" label="Select League"
            color="nadcl-accent" />
    </div>
</template>

<script>
import { ref } from 'vue';
import { useLeagueStore } from 'stores/league';
import { localApiService } from 'src/services/localApiService';

export default {
    name: 'LeagueSelector',
    setup() {
        const leagueStore = useLeagueStore();
        const selectedLeague = ref(null);

        return { leagueStore, selectedLeague }
    },
    computed: {
        leagueOptions() { return this.leagueStore.activeLeagues; },
    },
    mounted() {
        localApiService.getLeagues()
            .then(result => {
                this.leagueStore.setLeagues(result);
            });
    },
    methods: {
        updateSelectedLeague() {
            this.leagueStore.setSelectedLeague(this.selectedLeague);
        }
    }
};
</script>
<style>
.league-container {
    box-sizing: border-box;
    border: 1px solid var(--nadcl-white);
    border-radius: 5px;
    margin: auto;
    align-items: center;
    height: 40px;
}

.league-selector {
    font-size: 12px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    /* max-width: 150px; */
}
</style>