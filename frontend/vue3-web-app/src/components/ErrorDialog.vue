<template>
    <v-dialog class="alert-dialog">
        <v-card dark>
            <v-card-text>
                <span class="text-h6">Error</span>
            </v-card-text>

            <v-card-text>
                <p>There was an issue updating the fantasy draft. Error details: {{ props.error }}</p>
            </v-card-text>

            <v-card-actions align="right">
                <v-btn flat label="OK" color="white" v-close-popup @click="onOKClick"></v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
import { VDialog, VCard, VCardText, VCardActions, VBtn } from 'vuetify/components';
import { defineEmits, defineProps } from 'vue';

const props = defineProps({
    error: {
    }
})

const emit = defineEmits(['ok']);

const onOKClick = () => {
    emit('ok');
}
</script>

<style scoped>
.alert-dialog {
    background-color: #192038;
    z-index: 1000;
}
</style>