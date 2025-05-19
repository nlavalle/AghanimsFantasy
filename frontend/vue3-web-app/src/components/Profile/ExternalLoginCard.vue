<template>
    <v-card title="Sync Login Methods">
        <v-card-text>
            Use this if you'd like to log into your account with alternative methods (Email, Discord, Google)
        </v-card-text>
        <v-card-text v-if="!currentLoginProviders.some(login => login.loginProvider == 'Email')">
            <v-text-field v-model="email" name="email" placeholder="Email" required />
            <v-text-field v-model="password" name="password" placeholder="Password" required
                :append-icon="passShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
                :type="passShow ? 'text' : 'password'" @click:append="passShow = !passShow" />
            <v-text-field v-model="confirmPassword" name="confirmPassword" placeholder="Confirm Password" required
                :append-icon="confirmPassShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
                :type="confirmPassShow ? 'text' : 'password'" @click:append="confirmPassShow = !confirmPassShow" />
            <v-btn variant="outlined" @click="addEmailLogin(email, password)">
                Register Email
            </v-btn>
        </v-card-text>
        <v-divider />
        <v-card-text v-if="!currentLoginProviders.some(login => login.loginProvider == 'Discord')">
            <v-btn @click="login('Discord')" density="compact" variant="outlined" size="x-small" height="40px"
                style="text-align:left; background-color: #7289DA;">
                <font-awesome-icon class="icon" style="height: 20px; width: 20px;" :icon="faDiscord" /><span>Sign In
                    with Discord</span>
            </v-btn>
        </v-card-text>
        <v-divider />
        <v-card-text v-if="!currentLoginProviders.some(login => login.loginProvider == 'Google')">
            <img :src="GoogleSignIn" alt="Google logo" style="height: 40px; width: 175px; cursor: pointer;"
                @click="login('Google')" />
        </v-card-text>
        <v-snackbar v-model="showSuccess" timeout="5000" location="top right" color="success" elevation="4">
            Successfully added login
        </v-snackbar>
        <v-snackbar v-model="showError" timeout="5000" location="top right" color="error" elevation="4">
            {{ showErrorMessage }}
        </v-snackbar>
    </v-card>
</template>

<script setup lang="ts">
import { authApiService } from '@/services/authApiService';
import { useAuthStore } from '@/stores/auth';
import { computed, onBeforeMount, ref } from 'vue';
import { VCard, VCardText, VDivider, VSnackbar } from 'vuetify/components'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { passwordRules } from '@/utilities/PasswordRules';
import { faDiscord } from '@fortawesome/free-brands-svg-icons';
import GoogleSignIn from '@/assets/icons/google-sign-in.png'



const authStore = useAuthStore()

const email = ref('');
const password = ref('');
const confirmPassword = ref('');

const passShow = ref(false)
const confirmPassShow = ref(false)

const showSuccess = ref(false);
const showError = ref(false);
const showErrorMessage = ref('');

const passwordRuleArray = [
    passwordRules.required,
    passwordRules.hasUpper,
    passwordRules.hasLower,
    passwordRules.hasDigit,
    passwordRules.hasSymbol,
    passwordRules.min,
    passwordRules.confirmMatch(password.value, confirmPassword.value)
]

const currentLoginProviders = computed(() => {
    return authStore.user.loginProviders ?? []
})

onBeforeMount(() => {
    if (authStore.currentUser && currentLoginProviders.value.length == 0) {
        authStore.loadUserExternalLogins()
    }
})

const addEmailLogin = (email: string, pass: string) => {
    if (!email || !pass) return;
    authApiService.addEmailLogin(email, pass)
        .then(() => {
            authStore.loadUser()
            showSuccess.value = true
        })
        .catch((error) => {
            showError.value = true
            showErrorMessage.value = error
        });
}

async function login(provider: String) {
    // Open a new window or popup for OAuth login
    const loginWindow = window.open(`/api/auth/login-provider?provider=${provider}`, 'Login', 'width=600,height=800')

    if (loginWindow) {
        const checkLoginStatus = setInterval(() => {
            if (currentLoginProviders.value.some(login => login.loginProvider == provider) || loginWindow.closed) {
                loginWindow.close()
                showSuccess.value = true
                clearInterval(checkLoginStatus)
            }
            authStore.loadUserExternalLogins()
        }, 1000)
    }
}

</script>