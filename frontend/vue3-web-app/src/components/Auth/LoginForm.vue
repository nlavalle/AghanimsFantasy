<template>
    <div>
        <v-tabs v-model="registerTab">
            <v-tab>Login</v-tab>
            <v-tab>Sign Up</v-tab>
        </v-tabs>
        <!-- Register form -->
        <form v-if="registerTab == 1">
            <v-text-field v-model="email" name="email" placeholder="Email" required />
            <v-text-field v-model="password" name="password" placeholder="Password" required
                :append-icon="passShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
                :type="passShow ? 'text' : 'password'" @click:append="passShow = !passShow" />
            <v-text-field v-model="confirmPassword" name="confirmPassword" placeholder="Confirm Password" required
                :append-icon="confirmPassShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
                :type="confirmPassShow ? 'text' : 'password'" @click:append="confirmPassShow = !confirmPassShow" />
            <v-btn variant="outlined" @click="register(email, password)">
                Register
            </v-btn>
        </form>
        <!-- Login form -->
        <form v-else>
            <v-text-field v-model="email" name="email" placeholder="Email" required />
            <v-text-field v-model="password" name="password" placeholder="Password" required
                :append-icon="passShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
                :type="passShow ? 'text' : 'password'" @click:append="passShow = !passShow" />
            <v-btn variant="outlined" @click="login(email, password)">
                Login
            </v-btn>
        </form>

        <ErrorDialog v-model="showErrorModal" :error="errorDetails!" />
    </div>
</template>

<script setup lang="ts">
import { authApiService } from '@/services/authApiService';
import { ref } from 'vue';
import ErrorDialog from '@/components/ErrorDialog.vue';
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();

const registerTab = ref(0);

const email = ref('');
const password = ref('');
const confirmPassword = ref('');

const showErrorModal = ref(false);
const errorDetails = ref<Error>();

const passShow = ref(false)
const confirmPassShow = ref(false)

const passwordRules = {
    required: (value: string) => !!value || 'Required',
    hasUpper: (value: string) => /[A-Z]/.test(value) || 'Requires an uppercase letter',
    hasLower: (value: string) => /[a-z]/.test(value) || 'Requires a lowercase letter',
    hasDigit: (value: string) => /\d/.test(value) || 'Requires a digit',
    hasSymbol: (value: string) => /[^a-zA-Z0-9]/.test(value) || 'Requires a symbol',
    min: (v: string) => v.length >= 8 || 'Min 8 characters',
    confirmMatch: (v: string) => v === password.value || 'Passwords do not match'
}

const passwordRuleArray = [
    passwordRules.required,
    passwordRules.hasUpper,
    passwordRules.hasLower,
    passwordRules.hasDigit,
    passwordRules.hasSymbol,
    passwordRules.min,
    passwordRules.confirmMatch
]

const login = (email: string, pass: string) => {
    if (!email || !pass) return;
    return authApiService.login(email, pass)
        .then((responseStatusCode: number) => {
            let error = new Error()
            switch (responseStatusCode) {
                case 401:
                    error.message = "Invalid Login"
                    showError(error)
                    break;
                default:
                    authStore.checkAuthenticatedAsync().then(() => {
                        if (authStore.user) {
                            authStore.getUser()
                        }
                    })
                    break;
            }
        })
}

const register = (email: string, pass: string) => {
    if (!email || !pass) return;
    return authApiService.register(email, pass)
        .then(async (response: Response) => {
            let error = new Error()
            switch (response.status) {
                case 200:
                    authApiService.login(email, pass).then(() => {
                        authStore.checkAuthenticatedAsync()
                    })
                    break;
                case 400:
                    let data = await response.json();
                    if (data.errors) {
                        const messages = Object.values(data.errors).flat();
                        error.message = `Bad Request: ${messages.join('; ')}`
                        showError(error)
                    } else {
                        error.message = `Bad Request`
                        showError(error)
                    }
                    break;
                default:
                    error.message = `Unknown error.\n Http Status: ${response.status}\n Http Body: ${response.body ?? ''}`
                    showError(error)
                    break;
            }
        });
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