<template>
    <div class="league-container">
        <span @click="login">
            <i class="icon fa-solid fa-trophy"></i>Leagues
        </span>
        <div>
            <dropdown class="league-selector" :options="leagueOptions" optionLabel="name" v-model="selectedLeague"
                @update:model-value="updateSelectedLeague" inputStyle="font-size:12px" />
        </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useLeagueStore } from '@/stores/league';
import { localApiService } from '@/services/localApiService';
import Dropdown from 'primevue/dropdown';

const leagueStore = useLeagueStore();
const selectedLeague = ref(null);

const leagueOptions = computed(() => {
    return leagueStore.activeLeagues;
});

onMounted(() => {
    localApiService.getLeagues()
        .then(result => {
            leagueStore.setLeagues(result);
            //default to most recent league
            selectedLeague.value = leagueStore.activeLeagues.reduce((max, current) => {
                return current.id > max.id ? current : max;
            }, leagueStore.activeLeagues[0]);
            leagueStore.setSelectedLeague(selectedLeague.value);
        });
});

function updateSelectedLeague() {
    leagueStore.setSelectedLeague(selectedLeague.value);
}

</script>

<style scoped>
.league-container {
    /* box-sizing: border-box; */
    /* border: 1px solid var(--nadcl-white); */
    /* border-radius: 5px; */
    /* margin: auto; */
    /* height: 40px; */
    flex-direction: column;
    justify-self: left;
}

.league-selector {
    max-width: 150px;
    margin-top: 5px;

}

.icon {
    flex-shrink: 0;
    width: 25px;
    margin-right: 10px;
}
</style>