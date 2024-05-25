<template>
    <v-dialog class="alert-dialog">
        <template v-slot:default="{ isActive }">
            <v-card>
                <v-card-text>
                    <span class="text-h6">Success!</span>
                </v-card-text>

                <v-card-text>
                    <p>Your Fantasy Draft was updated.</p>
                </v-card-text>

                <v-card-actions align="right">
                    <v-btn flat label="OK" color="white" @click="onOKClick(isActive)"></v-btn>
                </v-card-actions>
            </v-card>
        </template>
    </v-dialog>
</template>

<script setup lang="ts">
import { VDialog, VCard, VCardText, VCardActions, VBtn } from 'vuetify/components';
import { defineEmits, type Ref } from 'vue';

const emit = defineEmits(['ok']);

const onOKClick = (isActive: Ref<boolean>) => {
    isActive.value = false;
    emit('ok');
}
</script>

<style scoped>
.alert-dialog {
    background-color: #192038;
    z-index: 1000;
}
</style>