<template>
    <v-card title="Personal Data">
        <v-card-text class="ma-5">
            <v-row>
                <p>
                    Download a json file of your user data in the database for your personal records.
                </p>
            </v-row>
            <v-row class="mb-10">
                <v-btn color="primary" @click="downloadPersonalData()">Download Personal Data</v-btn>
            </v-row>
            <v-row>
                <p>
                    <b>
                        This will delete everything from your user, this action cannot be undone.
                    </b>
                </p>
            </v-row>
            <v-row>
                <v-btn color="error" @click="showDeleteConfirm = !showDeleteConfirm">Delete Personal Data</v-btn>
            </v-row>
        </v-card-text>
        <v-dialog v-model="showDeleteConfirm" class="confirm-dialog">
            <template v-slot:default="{ isActive }">
                <v-card>
                    <v-card-text>
                        <span class="text-h6">Delete Personal Data?</span>
                    </v-card-text>

                    <v-card-text>
                        <p>Are you sure you want to delete all of your data? This cannot be undone.</p>
                    </v-card-text>

                    <v-card-text
                        v-if="authStore.currentUser.loginProviders?.some(provider => provider.loginProvider == 'Email')">
                        <v-text-field v-model="password" name="password" placeholder="Password" required
                            :append-icon="passShow ? 'eye' : 'eye-slash'" :type="passShow ? 'text' : 'password'"
                            @click:append="passShow = !passShow" />
                    </v-card-text>

                    <v-card-actions align="right">
                        <v-spacer></v-spacer>
                        <v-btn @click="onCancelClick()">Cancel</v-btn>
                        <v-btn @click="onOKClick()">OK</v-btn>
                    </v-card-actions>
                </v-card>
            </template>
        </v-dialog>
        <v-snackbar v-model="showError" timeout="5000" location="top right" color="error" elevation="4">
            {{ showErrorMessage }}
        </v-snackbar>
    </v-card>
</template>

<script setup lang="ts">
import { authApiService } from '@/services/authApiService';
import { useAuthStore } from '@/stores/auth';
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { VRow, VCard, VCardText } from 'vuetify/components'

const authStore = useAuthStore()
const router = useRouter()

const showDeleteConfirm = ref(false);
const password = ref('');
const passShow = ref(false);

const showError = ref(false);
const showErrorMessage = ref('');

const downloadPersonalData = () => {
    return authApiService.downloadPersonalData()
}

const onOKClick = () => {
    showDeleteConfirm.value = false;
    authApiService.deletePersonalData(password.value).then(() => {
        authStore.checkAuthenticatedAsync().then(() => {
            scrollAfterAlertDialog()
            router.push({ path: '/' })
        })
    }).catch((error) => {
        showError.value = true
        showErrorMessage.value = error
    })
}

const onCancelClick = () => {
    showDeleteConfirm.value = false;
    scrollAfterAlertDialog()
}

const scrollAfterAlertDialog = () => {
    setTimeout(function () {
        window.scrollTo({
            top: 0,
            left: 0,
            behavior: 'smooth'
        });
    }, 200);
}

</script>