<template>
    <v-dialog class="alert-dialog">
        <v-card>
            <v-card-text>
                <span class="text-h6">Success!</span>
            </v-card-text>

            <v-card-text>
                <p>Your Fantasy Draft was updated.</p>
            </v-card-text>

            <v-card-actions align="right">
                <v-btn flat label="OK" color="white" v-close-popup @click="onOKClick"></v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
import { VDialog, VCard, VCardText, VCardActions, VBtn } from 'vuetify/components';
import { defineEmits } from 'vue';

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