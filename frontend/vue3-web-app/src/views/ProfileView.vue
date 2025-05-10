<template>
    <div class="profile">
        <v-container>
            <v-row>
                <v-col :sm="8" :md="4" :lg="3">
                    <v-tabs v-model="profileTab" direction="vertical">
                        <v-tab value="profile">Profile</v-tab>
                        <v-tab value="password">Change Password</v-tab>
                        <v-tab value="external-login">External Login Providers</v-tab>
                        <!-- <v-tab value="notifications">Notifications</v-tab> -->
                        <v-tab value="personal">Personal data</v-tab>
                    </v-tabs>
                </v-col>
                <v-col :cols="12" :sm="10" :md="8" :lg="8">
                    <v-tabs-window v-model="profileTab">
                        <v-tabs-window-item value="profile">
                            <UpdateProfileCard />
                        </v-tabs-window-item>
                        <v-tabs-window-item value="password">
                            <ChangePasswordCard />
                        </v-tabs-window-item>
                        <v-tabs-window-item value="external-login">
                            <ExternalLoginCard />
                        </v-tabs-window-item>
                        <!-- <v-tabs-window-item value="notifications">
                            Notifications focus
                        </v-tabs-window-item> -->
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
import ChangePasswordCard from '@/components/Profile/ChangePasswordCard.vue';
import ExternalLoginCard from '@/components/Profile/ExternalLoginCard.vue';
import PersonalDataCard from '@/components/Profile/PersonalDataCard.vue';
import UpdateProfileCard from '@/components/Profile/UpdateProfileCard.vue';
import { ref } from 'vue';
import { VContainer, VRow, VCol, VTabs, VTab, VTabsWindow, VTabsWindowItem } from 'vuetify/components'

const profileTab = ref(0)

</script>

<style scoped></style>