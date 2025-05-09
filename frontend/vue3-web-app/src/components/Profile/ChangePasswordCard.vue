<template>
    <v-card title="Personal Data">
        <v-text-field v-model="currentPassword" label="Current Password" required
            :append-icon="currentPasswordShow ? 'eye' : 'eye-slash'" :rules="currentPasswordRuleArray"
            :type="currentPasswordShow ? 'text' : 'password'"
            @click:append="currentPasswordShow = !currentPasswordShow" />
        <v-text-field v-model="newPassword" label="New Password" required
            :append-icon="newPasswordShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
            :type="newPasswordShow ? 'text' : 'password'" @click:append="newPasswordShow = !newPasswordShow" />
        <v-text-field v-model="confirmNewPassword" label="Confirm New Password" required
            :append-icon="confirmNewPasswordShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
            :type="confirmNewPasswordShow ? 'text' : 'password'"
            @click:append="confirmNewPasswordShow = !confirmNewPasswordShow" />
        <v-btn variant="outlined" @click="authStore.changePassword(currentPassword, newPassword)">
            Update Password
        </v-btn>
    </v-card>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { ref } from 'vue';
import { VCard, VTextField, VBtn } from 'vuetify/components'

const authStore = useAuthStore()

const currentPassword = ref('');
const newPassword = ref('');
const confirmNewPassword = ref('');
const currentPasswordShow = ref(false)
const newPasswordShow = ref(false)
const confirmNewPasswordShow = ref(false)

const passwordRules = {
    required: (value: string) => !!value || 'Required',
    hasUpper: (value: string) => /[A-Z]/.test(value) || 'Requires an uppercase letter',
    hasLower: (value: string) => /[a-z]/.test(value) || 'Requires a lowercase letter',
    hasDigit: (value: string) => /\d/.test(value) || 'Requires a digit',
    hasSymbol: (value: string) => /[^a-zA-Z0-9]/.test(value) || 'Requires a symbol',
    min: (v: string) => v.length >= 8 || 'Min 8 characters',
    confirmMatch: (v: string) => v === newPassword.value || 'Passwords do not match'
}

const currentPasswordRuleArray = [
    passwordRules.required,
    passwordRules.hasUpper,
    passwordRules.hasLower,
    passwordRules.hasDigit,
    passwordRules.hasSymbol,
    passwordRules.min
]

const passwordRuleArray = [
    ...currentPasswordRuleArray,
    passwordRules.confirmMatch
]

</script>