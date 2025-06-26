<template>
    <div>
        <v-tabs v-model="registerTab">
            <v-tab value="login">Login</v-tab>
            <v-tab value="register">Sign Up</v-tab>
        </v-tabs>
        <v-tabs-window v-model="registerTab">
            <v-tabs-window-item value="login">
                <LoginCard v-model:tab="registerTab" v-model:email="email" />
            </v-tabs-window-item>
            <v-tabs-window-item value="register">
                <RegisterCard v-model:tab="registerTab" v-model:email="email" />
            </v-tabs-window-item>
            <v-tabs-window-item value="resetPassword">
                <ResetPasswordCard v-model:email="email" />
            </v-tabs-window-item>
        </v-tabs-window>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { VTabs, VTab, VTabsWindow, VTabsWindowItem } from 'vuetify/components';
import RegisterCard from './RegisterCard.vue';
import LoginCard from './LoginCard.vue';
import ResetPasswordCard from './ResetPasswordCard.vue';

const registerTab = ref('login');
const email = ref('');

</script>