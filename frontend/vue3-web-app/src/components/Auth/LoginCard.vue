<template>
    <v-form>
        <v-text-field v-model="email" name="email" placeholder="Email" required />
        <v-text-field v-model="password" name="password" placeholder="Password" required
            :append-icon="passShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
            :type="passShow ? 'text' : 'password'" @click:append="passShow = !passShow" />
        <v-btn class="mb-2 mr-10" variant="outlined" @click="login(email, password)">
            Login
        </v-btn>
        <v-btn class="mb-2" variant="outlined" @click="forgotPassword(email)">
            Forgot Password?
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

const tab = defineModel('tab')
const email = defineModel<string>('email')

const authStore = useAuthStore()

const showErrorModal = ref(false);
const errorDetails = ref<Error>();

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

const login = (email: string | undefined, pass: string) => {
    if (!email || !pass) return;
    return authStore.login(email, pass)
        ?.catch((error: Error) => {
            showError(error)
        })
}

const forgotPassword = (email: string | undefined) => {
    if (!email) return;
    return authStore.forgotPassword(email)
        ?.then(() => {
            tab.value = "resetPassword"
        })
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