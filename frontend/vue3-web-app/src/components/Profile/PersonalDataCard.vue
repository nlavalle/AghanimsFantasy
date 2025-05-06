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
                <v-btn color="error">Delete Personal Data</v-btn>
            </v-row>
        </v-card-text>
    </v-card>
</template>

<script setup lang="ts">
import { authApiService } from '@/services/authApiService';
import { VRow, VCard, VCardText } from 'vuetify/components'

const downloadPersonalData = () => {
    return authApiService.downloadPersonalData()
}

</script>