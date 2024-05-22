<template>
    <v-container>
        <v-row>
            <v-col class="v-col-9">
                <v-row>
                    <CreateDraftPicks />
                </v-row>
                <v-row>
                    <v-col>
                        <PlayerPicksAvailable v-model="selectedPlayer" />
                    </v-col>
                </v-row>
            </v-col>
            <v-col class="v-col-3">
                <v-row>
                    <PlayerStats v-model="selectedPlayer" />
                </v-row>
            </v-col>
        </v-row>

    </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { VContainer, VRow, VCol } from 'vuetify/components';
import CreateDraftPicks from './CreateDraftPicks.vue';
import PlayerPicksAvailable from './PlayerPicksAvailable.vue';
import PlayerStats from './PlayerStats.vue';

const selectedPlayer = ref(null);

</script>

<style scoped></style>