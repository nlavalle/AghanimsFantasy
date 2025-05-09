<template>
    <v-dialog class="confirm-dialog">
        <template v-slot:default="{ isActive }">
            <v-card>
                <v-card-text>
                    <span class="text-h6">{{ props.title }}</span>
                </v-card-text>

                <v-card-text>
                    <p>{{ props.body }}</p>
                </v-card-text>

                <v-card-actions align="right">
                    <v-spacer></v-spacer>
                    <v-btn @click="onCancelClick(isActive)">Cancel</v-btn>
                    <v-btn @click="onOKClick(isActive)">OK</v-btn>
                </v-card-actions>
            </v-card>
        </template>
    </v-dialog>
</template>

<script setup lang="ts">
import { VDialog, VCard, VCardText, VCardActions, VBtn, VSpacer } from 'vuetify/components';
import { type Ref } from 'vue';

const props = defineProps({
    title: {
        type: String,
        required: true
    },
    body: {
        type: String,
        required: true
    }
})

const emit = defineEmits(['ok', 'cancel']);

const onOKClick = (isActive: Ref<boolean>) => {
    isActive.value = false;
    emit('ok');
    scrollAfterAlertDialog()
}

const onCancelClick = (isActive: Ref<boolean>) => {
    isActive.value = false;
    emit('cancel');
    scrollAfterAlertDialog()
}

const scrollAfterAlertDialog = () => {
    setTimeout(function () {
        window.scrollTo({
            top: 0,
            left: 0,
            behavior: 'smooth'
        });
    }, 200);
}
</script>

<style scoped>
.confirm-dialog {
    z-index: 1000;
    max-width: 500px;
}
</style>