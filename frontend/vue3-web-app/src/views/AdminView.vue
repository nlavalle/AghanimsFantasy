<template>
  <div class="admin">
    <h3>This is an admin page</h3>
  </div>
  <v-container>
    <v-row>
      <v-col>
        <v-row>
          <v-tabs v-model="statsTab">
            <v-tab value="league">League</v-tab>
            <v-tab value="fantasyLeague">Fantasy League</v-tab>
          </v-tabs>
        </v-row>
      </v-col>
    </v-row>
    <v-row v-if="isMounted">
      <CrudTable :table-columns="columns" :table-items="items" :default-item-specified="defaultItem" @save="saveItem" @edit="editItem"
        @delete="deleteItem" />
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { VContainer, VRow, VCol, VTabs, VTab } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import CrudTable from '@/components/Admin/CrudTable.vue'

const leagueStore = useFantasyLeagueStore();

const statsTab = ref('fantasy')
const leagueData = ref(leagueStore.allLeagues);
const fantasyLeagueData = ref([]);

const isMounted = ref(false);

onMounted(() => {
  localApiService.getLeagues('true').then((result: any) => {
    leagueStore.setLeagues(result);
    leagueData.value = leagueStore.allLeagues;
  })
  localApiService.getFantasyLeagues().then((result: any) => {
    fantasyLeagueData.value = result;
  })
  isMounted.value = true;
})

const saveItem = (item: any) => {
  switch (statsTab.value) {
    case "league":
      localApiService.postLeague(item)?.then(() => {
        localApiService.getLeagues('true').then((result: any) => {
          leagueStore.setLeagues(result);
          leagueData.value = leagueStore.allLeagues;
        })
      })
      break;
    case "fantasyLeague":
      localApiService.postFantasyLeague(item)?.then(() => {
        localApiService.getFantasyLeagues().then((result: any) => {
          fantasyLeagueData.value = result;
        })
      })
      break;
  }
}

const editItem = (item: any) => {
  switch (statsTab.value) {
    case "league":
      localApiService.putLeague(item)?.then(() => {
        localApiService.getLeagues('true').then((result: any) => {
          leagueStore.setLeagues(result);
          leagueData.value = leagueStore.allLeagues;
        })
      })
      break;
    case "fantasyLeague":
      localApiService.putFantasyLeague(item)?.then(() => {
        localApiService.getFantasyLeagues().then((result: any) => {
          fantasyLeagueData.value = result;
        })
      })
      break;
  }
}

const deleteItem = (item: any) => {
  switch (statsTab.value) {
    case "league":
      localApiService.deleteLeague(item)?.then(() => {
        localApiService.getLeagues('true').then((result: any) => {
          leagueStore.setLeagues(result);
          leagueData.value = leagueStore.allLeagues;
        })
      });
      break;
    case "fantasyLeague":
      localApiService.deleteFantasyLeague(item)?.then(() => {
        localApiService.getFantasyLeagues().then((result: any) => {
          fantasyLeagueData.value = result;
        })
      });
      break;
  }

}

const columns = computed(() => {
  switch (statsTab.value) {
    case "league":
      return leagueColumns;
    case "fantasyLeague":
      return fantasyLeagueColumns;
  }
  return [];
});

const items = computed(() => {
  switch (statsTab.value) {
    case "league":
      return leagueData.value
    case "fantasyLeague":
      return fantasyLeagueData.value
  }
  return [];
})

const defaultItem = computed(() => {
  switch (statsTab.value) {
    case "league":
      return leagueDefaultItemSpecified
    case "fantasyLeague":
      return fantasyLeagueDefaultItemSpecified
  }
  return undefined;
})

const leagueDefaultItemSpecified = {
  id: 0,
  name: "string",
  isActive: true
}

const fantasyLeagueDefaultItemSpecified = {
  id: 0,
  leagueId: 0,
  name: "string",
  isActive: true,
  isPrivate: false,
  fantasyDraftLocked: 0,
  leagueStartTime: 0,
  leagueEndTime: 0
}

const leagueColumns = [
  {
    title: 'ID',
    value: 'id'
  },
  {
    title: 'League Name',
    value: 'name',
  },
  {
    title: 'Active?',
    value: 'isActive',
  },
];

const fantasyLeagueColumns = [
  {
    title: 'League ID',
    value: 'leagueId',
  },
  {
    title: 'Fantasy League ID',
    value: 'id'
  },
  {
    title: 'Fantasy League Name',
    value: 'name',
  },
  {
    title: 'Active?',
    value: 'isActive',
  },
  {
    title: 'Fantasy Lock Time',
    value: 'fantasyDraftLocked',
  },
  {
    title: 'League Start Time',
    value: 'leagueStartTime',
  },
  {
    title: 'League End Time',
    value: 'leagueEndTime',
  },
];

</script>

<style></style>
