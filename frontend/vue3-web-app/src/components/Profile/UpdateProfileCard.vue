<template>
    <v-card title="Profile">
        <v-row>
            <v-text-field v-model="userId" label="User ID" name="userId" disabled />
        </v-row>
        <v-row>
            <v-col cols="8">
                <v-text-field v-model="displayName" label="Display Name" name="displayName"
                    placeholder="World's best drafter" required />
            </v-col>
            <v-col cols="1">
                <v-btn @click="changeDisplayName()">Save</v-btn>
            </v-col>
        </v-row>
        <v-row>
            <v-col>
                <v-text-field v-model="email" label="Email" name="email" disabled />
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="8">
                <v-text-field v-model="isEmailConfirmed" label="Confirmed Email?" name="emailConfirmed" disabled />
            </v-col>
            <v-col cols="1">
                <v-btn @click="resendConfirmationEmail()">Resend</v-btn>
            </v-col>
        </v-row>
        <v-row>
            <v-text-field v-model="discordName" label="Discord Name" name="discordName" disabled />
        </v-row>
        <v-snackbar v-model="showResendSuccess" timeout="20000" location="top right" color="success" elevation="4">
            Resent confirmation email, please wait 5-10 minutes. It may be in your spam folder
        </v-snackbar>
        <v-snackbar v-model="showNameSuccess" timeout="5000" location="top right" color="success" elevation="4">
            Successfully updated name
        </v-snackbar>
    </v-card>
</template>

<script setup lang="ts">
import { authApiService } from '@/services/authApiService';
import { useAuthStore } from '@/stores/auth';
import { computed, ref } from 'vue';
import { VCard, VTextField, VBtn, VSnackbar } from 'vuetify/components'

const authStore = useAuthStore()
const updateDisplayName = ref()
const updateEmail = ref()
const showResendSuccess = ref(false)
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
const isEmailConfirmed = computed({
    get: () => authStore.user.emailConfirmed,
    set: () => { }
})
const discordName = computed({
    get: () => authStore.user.discordName,
    set: () => { }
})

const changeDisplayName = () => {
    authStore.updateDisplayName(updateDisplayName.value)?.then(() => {
        showNameSuccess.value = true
    })
}

const resendConfirmationEmail = () => {
    if (!authStore.user.email) return;
    authApiService.resendConfirmationEmail(authStore.user.email).then(() => {
        showResendSuccess.value = true
    });
}

</script>