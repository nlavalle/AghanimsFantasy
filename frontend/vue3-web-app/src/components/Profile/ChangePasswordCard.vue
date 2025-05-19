<template>
    <v-card title="Change Password" class="pa-3 pt-1">
        <v-form v-model="isFormValid">
            <v-text-field v-model="currentPassword" label="Current Password" required
                :append-icon="currentPasswordShow ? 'eye' : 'eye-slash'" :rules="currentPasswordRuleArray"
                :type="currentPasswordShow ? 'text' : 'password'"
                @click:append="currentPasswordShow = !currentPasswordShow" />
            <v-text-field v-model="newPassword" label="New Password" required
                :append-icon="newPasswordShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
                :type="newPasswordShow ? 'text' : 'password'" @click:append="newPasswordShow = !newPasswordShow" />
            <v-text-field v-model="confirmNewPassword" label="Confirm New Password" required
                :append-icon="confirmNewPasswordShow ? 'eye' : 'eye-slash'" :rules="confirmPasswordRuleArray"
                :type="confirmNewPasswordShow ? 'text' : 'password'"
                @click:append="confirmNewPasswordShow = !confirmNewPasswordShow" />
            <v-btn variant="outlined" @click="changePassword()" :disabled="!isFormValid">
                Update Password
            </v-btn>
        </v-form>
        <v-snackbar v-model="showPasswordSuccess" timeout="5000" location="top right" color="success" elevation="4">
            Successfully updated password
        </v-snackbar>
        <v-snackbar v-model="showPasswordError" timeout="5000" location="top right" color="error" elevation="4">
            {{ showPasswordErrorMessage }}
        </v-snackbar>
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

const showPasswordSuccess = ref(false)
const showPasswordError = ref(false)
const showPasswordErrorMessage = ref('')

const isFormValid = ref(false)

const passwordRules = {
    required: (value: string) => !!value || 'Required',
    hasUpper: (value: string) => /[A-Z]/.test(value) || 'Requires an uppercase letter',
    hasLower: (value: string) => /[a-z]/.test(value) || 'Requires a lowercase letter',
    hasDigit: (value: string) => /\d/.test(value) || 'Requires a digit',
    hasSymbol: (value: string) => /[^a-zA-Z0-9]/.test(value) || 'Requires a symbol',
    min: (v: string) => v.length >= 8 || 'Min 8 characters'
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
    ...currentPasswordRuleArray
]

const confirmPasswordRuleArray = [
    ...currentPasswordRuleArray,
    (value: string) => value === newPassword.value || 'Passwords must match'
]

const changePassword = () => {
    if (!currentPassword.value || !newPassword.value || !confirmNewPassword.value) return
    authStore.changePassword(currentPassword.value, newPassword.value)
        .then(() => {
            showPasswordSuccess.value = true
        })
        .catch((error) => {
            showPasswordError.value = true
            showPasswordErrorMessage.value = error
        })
}

</script>