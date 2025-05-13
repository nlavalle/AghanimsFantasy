<template>
    <v-form v-model="isFormValid">
        <v-text-field v-model="email" name="email" placeholder="Email" required :rules="[passwordRules.required]" />
        <v-text-field v-model="password" name="password" placeholder="Password" required
            :append-icon="passShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
            :type="passShow ? 'text' : 'password'" @click:append="passShow = !passShow" />
        <v-text-field v-model="confirmPassword" name="confirmPassword" placeholder="Confirm Password" required
            :append-icon="confirmPassShow ? 'eye' : 'eye-slash'" :rules="confirmPasswordRuleArray"
            :type="confirmPassShow ? 'text' : 'password'" @click:append="confirmPassShow = !confirmPassShow" />
        <v-btn variant="outlined" @click="register(email, password)" :disabled="!isFormValid">
            Register
        </v-btn>
    </v-form>
    <ErrorDialog v-model="showErrorModal" :error="errorDetails!" />
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { ref } from 'vue';
import { VForm, VTextField, VBtn } from 'vuetify/components'
import ErrorDialog from '@/components/ErrorDialog.vue';
import { passwordRules } from '@/utilities/PasswordRules';

const authStore = useAuthStore()

const showErrorModal = ref(false);
const errorDetails = ref<Error>();

const email = ref('');
const password = ref('');
const confirmPassword = ref('');

const passShow = ref(false)
const confirmPassShow = ref(false)

const isFormValid = ref(false)

const passwordRuleArray = [
    passwordRules.required,
    passwordRules.hasUpper,
    passwordRules.hasLower,
    passwordRules.hasDigit,
    passwordRules.hasSymbol,
    passwordRules.min
]

const confirmPasswordRuleArray = [
    ...passwordRuleArray,
    (value: string) => value === password.value || 'Passwords must match'
]

const register = (email: string, pass: string) => {
    if (!email || !pass) return;
    return authStore.register(email, pass)
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