<template>
    <v-footer border class="footerbar">
        <v-container class="ma-0 pa-0">
            <v-row justify="center" align="center" no-gutters>
                <v-spacer />
                <v-col align="center">
                    <v-btn size="small" variant="text" href="mailto:support@aghanimsfantasy.com"
                        target="_blank">Support</v-btn>
                </v-col>
                <v-col align="center">
                    <v-btn size="small" variant="text" href="https://discord.gg/2Ru2CVgby9" target="_blank"
                        icon="fa-brands fa-discord"></v-btn>
                </v-col>
                <!-- <v-col align="center">
                    <v-btn size="small" variant="text" href="https://github.com/nlavalle/AghanimsFantasy"
                        target="_blank" icon="fa-brands fa-github"></v-btn>
                </v-col> -->
                <v-spacer />
            </v-row>
            <v-row justify="center" align="center" no-gutters>
                <span style="font-size:0.7rem;text-align:center">Aghanim's Fantasy is not affiliated with Valve
                    Corporation</span>
            </v-row>
        </v-container>
    </v-footer>
</template>

<script setup lang="ts">
import { VFooter, VContainer, VRow, VCol, VBtn, VSpacer } from 'vuetify/components';

</script>

<style scoped>
.footerbar {
    opacity: 50%;
}

.icon {
    height: 15px;
}
</style>
