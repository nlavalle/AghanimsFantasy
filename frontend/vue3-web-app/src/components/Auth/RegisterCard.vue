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
    <v-dialog v-model="showInfoModal" class="info-dialog">
        <template v-slot:default="{ isActive }">
            <v-card>
                <v-card-text>
                    <span class="text-h6">Please confirm your Email</span>
                </v-card-text>
                <v-card-text>
                    <p>Registration was successful. A confirmation email has been sent to {{ email }}.</p>
                </v-card-text>
                <v-card-actions align="right">
                    <v-spacer></v-spacer>
                    <v-btn @click="onOKClick(isActive)">OK</v-btn>
                </v-card-actions>
            </v-card>
        </template>
    </v-dialog>
    <ErrorDialog v-model="showErrorModal" :error="errorDetails!" />
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { ref, type Ref } from 'vue';
import { VForm, VTextField, VBtn } from 'vuetify/components'
import ErrorDialog from '@/components/ErrorDialog.vue';
import { passwordRules } from '@/utilities/PasswordRules';

const authStore = useAuthStore()
const tab = defineModel('tab')
const email = defineModel<string>('email')

const showErrorModal = ref(false);
const errorDetails = ref<Error>();

const showInfoModal = ref(false);

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

const register = (email: string | undefined, pass: string) => {
    if (!email || !pass) return;
    return authStore.register(email, pass)
        ?.then(() => {
            showInfoModal.value = true;
        })
        ?.catch((error: Error) => {
            showError(error)
        })
}

const onOKClick = (isActive: Ref<boolean>) => {
    isActive.value = false;
    tab.value = "login"
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

<style scoped>
.info-dialog {
    z-index: 1000;
    max-width: 500px;
}
</style>