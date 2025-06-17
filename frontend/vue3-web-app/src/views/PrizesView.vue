<template>
    <v-container>
        <h3>Prizes</h3>
        <ol>
            <li v-for="(prize) in availablePrizes">{{ prize }}</li>
        </ol>
    </v-container>
</template>

<script setup lang="ts">
import { localApiService } from '@/services/localApiService';
import { onMounted, ref } from 'vue';
import { VContainer } from 'vuetify/components';

const availablePrizes = ref()

onMounted(() => {
    localApiService.getAllPrizes().then((result: any) => {
        availablePrizes.value = result
    })
})
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped></style>
