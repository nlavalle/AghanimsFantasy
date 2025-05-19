<template>
    <v-card title="Profile">
        <v-row>
            <v-text-field v-model="userId" label="User ID" name="userId" disabled />
        </v-row>
        <v-row>
            <v-col>
                <v-text-field v-model="displayName" label="Display Name" name="displayName"
                    placeholder="World's best drafter" required />
            </v-col>
            <v-col>
                <v-btn @click="changeDisplayName()">Save</v-btn>
            </v-col>
        </v-row>
        <v-row>
            <v-col>
                <v-text-field v-model="email" label="Email" name="email" disabled />
            </v-col>
        </v-row>
        <!-- <v-row>
            <v-col>
                <v-text-field v-model="isEmailConfirmed" label="Confirmed Email?"
                    name="emailConfirmed" disabled />
            </v-col>
            <v-col cols="3">
                <v-btn>Resend email confirmation</v-btn>
            </v-col>
        </v-row> -->
        <v-row>
            <v-text-field v-model="discordName" label="Discord Name" name="discordName" disabled />
        </v-row>
        <v-snackbar v-model="showNameSuccess" timeout="5000" location="top right" color="success" elevation="4">
            Successfully updated name
        </v-snackbar>
    </v-card>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { computed, ref } from 'vue';
import { VCard, VTextField, VBtn, VSnackbar } from 'vuetify/components'

const authStore = useAuthStore()
const updateDisplayName = ref()
const updateEmail = ref()
const showNameSuccess = ref(false)

const userId = computed({
    get: () => authStore.user.id,
    set: () => { }
})
const displayName = computed({
    get: () => authStore.user.name,
    set: (newDisplayName) => updateDisplayName.value = newDisplayName
})
const email = computed({
    get: () => authStore.user.email,
    set: (newEmail) => updateEmail.value = newEmail
})
const isEmailConfirmed = ref(false)
const discordName = computed({
    get: () => authStore.user.discordName,
    set: () => { }
})

const changeDisplayName = () => {
    authStore.updateDisplayName(updateDisplayName.value)?.then(() => {
        showNameSuccess.value = true
    })
}

</script>