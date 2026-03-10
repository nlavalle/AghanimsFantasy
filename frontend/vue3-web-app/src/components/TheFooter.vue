<template>
    <v-divider />
    <v-footer class="pa-4">
        <v-container>
            <v-row justify="center" align="start">
                <v-col cols="12" sm="4" class="text-center text-sm-start">
                    <div class="footer-heading">Quick Links</div>
                    <div class="footer-links">
                        <router-link to="/">Home</router-link>
                        <router-link to="/fantasy">Fantasy</router-link>
                        <router-link to="/stats">Stats</router-link>
                        <router-link to="/about">About</router-link>
                    </div>
                </v-col>

                <v-col cols="12" sm="4" class="text-center">
                    <div class="footer-heading">Community</div>
                    <div class="footer-links">
                        <v-btn size="small" variant="text" href="https://discord.gg/2Ru2CVgby9" target="_blank"
                            prepend-icon="fa-brands fa-discord">
                            Discord
                        </v-btn>
                        <v-btn size="small" variant="text" href="mailto:support@aghanimsfantasy.com" target="_blank">
                            Support
                        </v-btn>
                    </div>
                </v-col>

                <v-col cols="12" sm="4" class="text-center text-sm-end">
                    <div class="footer-heading">Aghanim's Fantasy</div>
                    <div class="text-medium-emphasis footer-tagline">
                        Dota 2 Fantasy Leagues
                    </div>
                </v-col>
            </v-row>

            <v-row justify="center" class="mt-4">
                <span class="text-medium-emphasis footer-disclaimer">
                    Aghanim's Fantasy is not affiliated with Valve Corporation.
                    Dota 2 is a registered trademark of Valve Corporation.
                </span>
            </v-row>
        </v-container>
    </v-footer>
</template>

<script setup lang="ts">
import { VFooter, VContainer, VRow, VCol, VBtn, VDivider } from 'vuetify/components';
</script>

<style scoped>
.footer-heading {
    font-family: var(--font-heading);
    font-weight: 600;
    font-size: 0.85rem;
    margin-bottom: var(--space-sm);
    text-transform: uppercase;
    letter-spacing: 0.05em;
}

.footer-links {
    display: flex;
    flex-direction: column;
    gap: var(--space-xs);
}

.footer-links a {
    color: inherit;
    text-decoration: none;
    font-size: 0.85rem;
    opacity: 0.7;
    transition: opacity 0.2s ease;
}

.footer-links a:hover {
    opacity: 1;
}

.footer-tagline {
    font-size: 0.85rem;
}

.footer-disclaimer {
    font-size: 0.7rem;
    text-align: center;
}
</style>
