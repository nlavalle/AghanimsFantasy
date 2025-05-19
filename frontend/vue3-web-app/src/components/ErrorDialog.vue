<template>
    <v-dialog class="error-dialog">
        <template v-slot:default="{ isActive }">
            <v-card dark>
                <v-card-text>
                    <span class="text-h6">Error</span>
                </v-card-text>

                <v-card-text>
                    <p>
                        {{ props.error }}
                    </p>
                </v-card-text>

                <v-card-actions align="right">
                    <v-btn @click="onOKClick(isActive)">OK</v-btn>
                </v-card-actions>
            </v-card>
        </template>
    </v-dialog>
</template>

<script setup lang="ts">
import { VDialog, VCard, VCardText, VCardActions, VBtn } from 'vuetify/components';
import { type Ref } from 'vue';

const props = defineProps({
    error: {
        type: Error,
        required: false
    }
})

const emit = defineEmits(['ok']);

const onOKClick = (isActive: Ref<boolean>) => {
    isActive.value = false;
    emit('ok');
}
</script>

<style scoped>
.error-dialog {
    z-index: 1000;
    max-width: 500px;
}
</style>