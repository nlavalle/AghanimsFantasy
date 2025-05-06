<template>
    <div class="profile">
        <v-container>
            <v-row>
                <v-col cols="2">
                    <v-tabs v-model="profileTab" direction="vertical">
                        <v-tab value="profile">Profile</v-tab>
                        <v-tab value="password">Change Password</v-tab>
                        <v-tab value="external-login">External Login Providers</v-tab>
                        <v-tab value="notifications">Notifications</v-tab>
                        <v-tab value="personal">Personal data</v-tab>
                    </v-tabs>
                </v-col>
                <v-col>
                    <v-tabs-window v-model="profileTab">
                        <v-tabs-window-item value="profile">
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
                                        <v-btn @click="updateDisplayName(displayName)">Save</v-btn>
                                    </v-col>
                                </v-row>
                                <v-row>
                                    <v-text-field v-model="email" label="Email" name="email" disabled />
                                </v-row>
                                <v-row>
                                    <v-col>
                                        <v-text-field v-model="isEmailConfirmed" label="Confirmed Email?"
                                            name="emailConfirmed" disabled />
                                    </v-col>
                                    <v-col cols="3">
                                        <v-btn>Resend email confirmation</v-btn>
                                    </v-col>
                                </v-row>
                                <v-row>
                                    <v-text-field v-model="discordName" label="Discord Name" name="discordName"
                                        disabled />
                                </v-row>
                            </v-card>
                        </v-tabs-window-item>
                        <v-tabs-window-item value="password">
                            <v-text-field v-model="currentPassword" label="Current Password" required
                                :append-icon="currentPasswordShow ? 'eye' : 'eye-slash'"
                                :rules="currentPasswordRuleArray" :type="currentPasswordShow ? 'text' : 'password'"
                                @click:append="currentPasswordShow = !currentPasswordShow" />
                            <v-text-field v-model="newPassword" label="New Password" required
                                :append-icon="newPasswordShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
                                :type="newPasswordShow ? 'text' : 'password'"
                                @click:append="newPasswordShow = !newPasswordShow" />
                            <v-text-field v-model="confirmNewPassword" label="Confirm New Password" required
                                :append-icon="confirmNewPasswordShow ? 'eye' : 'eye-slash'" :rules="passwordRuleArray"
                                :type="confirmNewPasswordShow ? 'text' : 'password'"
                                @click:append="confirmNewPasswordShow = !confirmNewPasswordShow" />
                            <v-btn variant="outlined" @click="changePassword(currentPassword, newPassword)">
                                Update Password
                            </v-btn>
                        </v-tabs-window-item>
                        <v-tabs-window-item value="external-login">
                            External Login Providers focus
                        </v-tabs-window-item>
                        <v-tabs-window-item value="notifications">
                            Notifications focus
                        </v-tabs-window-item>
                        <v-tabs-window-item value="personal">
                            <PersonalDataCard />
                        </v-tabs-window-item>
                    </v-tabs-window>
                </v-col>
            </v-row>
        </v-container>
    </div>
</template>

<script setup lang="ts">
import PersonalDataCard from '@/components/Profile/PersonalDataCard.vue';
import { authApiService } from '@/services/authApiService';
import { useAuthStore } from '@/stores/auth';
import { onBeforeMount, ref } from 'vue';
import { VContainer, VRow, VCol, VTabs, VTab, VTabsWindow, VTabsWindowItem, VCard } from 'vuetify/components'

const profileTab = ref(0)

const authStore = useAuthStore()
const userId = ref(authStore.user?.id)
const displayName = ref(authStore.user?.name)
const email = ref('')
const isEmailConfirmed = ref(false)
const discordName = ref(authStore.user?.discordName)

const currentPassword = ref('');
const newPassword = ref('');
const confirmNewPassword = ref('');
const currentPasswordShow = ref(false)
const newPasswordShow = ref(false)
const confirmNewPasswordShow = ref(false)

onBeforeMount(() => {
    authApiService.getManageInfo().then((manageInfo) => {
        if (manageInfo) {
            email.value = manageInfo.email
            isEmailConfirmed.value = manageInfo.isEmailConfirmed
        }
    })
})

const updateDisplayName = (newDisplayName: string | undefined) => {
    if (!newDisplayName) return
    authApiService.updateDisplayName(newDisplayName).then(() => {
        authStore.getUser()
    })
}

const changePassword = (currentPassword: string, newPassword: string) => {
    authApiService.changePassword(currentPassword, newPassword).then((manageInfo) => {
        if (manageInfo) {
            email.value = manageInfo.email
            isEmailConfirmed.value = manageInfo.isEmailConfirmed
        }
    })
}

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

<style scoped></style>