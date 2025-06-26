<template>
    <v-form>
        <span>A reset password code has been sent to your email if it is registered</span>
        <v-text-field v-model="email" name="email" placeholder="Email" required />
        <v-text-field v-model="confirmationCode" name="confirmationCode" placeholder="Confirmation Code" required
            :append-icon="confirmationCodeShow ? 'eye' : 'eye-slash'" :type="confirmationCodeShow ? 'text' : 'password'"
            @click:append="confirmationCodeShow = !confirmationCodeShow" />
        <v-text-field v-model="password" name="password" placeholder="New Password" required
            :append-icon="passShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
            :type="passShow ? 'text' : 'password'" @click:append="passShow = !passShow" />
        <v-btn variant="outlined" @click="resetPassword(email, confirmationCode, password)">
            Reset Password
        </v-btn>
    </v-form>
    <ErrorDialog v-model="showErrorModal" :error="errorDetails!" />
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { VForm, VTextField, VBtn } from 'vuetify/components'
import ErrorDialog from '@/components/ErrorDialog.vue';
import { passwordRules } from '@/utilities/PasswordRules';
import { useAuthStore } from '@/stores/auth';

const email = defineModel<string>('email')

const authStore = useAuthStore()

const showErrorModal = ref(false);
const errorDetails = ref<Error>();

const confirmationCode = ref('');

const confirmationCodeShow = ref(false)

const password = ref('');

const passShow = ref(false)

const passwordRuleArray = [
    passwordRules.required,
    passwordRules.hasUpper,
    passwordRules.hasLower,
    passwordRules.hasDigit,
    passwordRules.hasSymbol,
    passwordRules.min
]

const resetPassword = (email: string | undefined, confirmationCode: string, pass: string) => {
    if (!email || !pass || !confirmationCode) return;
    return authStore.resetPassword(email, confirmationCode, pass)
        ?.catch((error: Error) => {
            showError(error)
        })
}

const showError = (error: Error) => {
    errorDetails.value = error;
    showErrorModal.value = true;
    window.scrollTo({
        top: 0,
        left: 0,
        behavior: 'smooth'
    });
}
</script>
