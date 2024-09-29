<template>
  <v-container>
    <v-row>
      <h1><i>Welcome to Aghanim's Fantasy League for Dota 2</i></h1>
    </v-row>
    <v-row>
      <v-card class="ma-2 pa-1" variant="elevated" color="primary" to="/fantasy">
        <v-card-title>
          <h2>Start Drafting!</h2>
        </v-card-title>
      </v-card>
    </v-row>
    <v-row>
      <v-card class="ma-2 pa-1 bg-surface" variant="flat">
        <v-card-title>
          <h2 class="attentionTitle">How to Play</h2>
        </v-card-title>
      </v-card>
    </v-row>
    <!-- <v-row class="pa-2" justify="start">
      <div>
        <v-card class="mx-auto" elevation="24" :width="isDesktop ? '768' : '352'" :height="isDesktop ? '432' : '198'">
          <v-carousel height="100%" width="100%" progress="primary" hide-delimiters show-arrows="hover"
            next-icon="fas fa-chevron-right">
            <v-carousel-item key="1" :src="loginDiscordGif" cover>
              <div class="d-flex fill-height justify-center align-end">
                <span v-if="isDesktop" style="font-size:1.3rem; padding-bottom:20px; z-index:2">
                  1. Connect via your Discord, no sign-up needed
                </span>
                <span v-else style="font-size:0.8rem; padding-bottom:5px; z-index:2">
                  1. Connect via your Discord, no sign-up needed
                </span>
              </div>
            </v-carousel-item>
            <v-carousel-item key="2" :src="chooseFantasyLeagueGif" cover>
              <div class="d-flex fill-height justify-center align-end">
                <span v-if="isDesktop" style="font-size:1.3rem; padding-bottom:20px; z-index:2">
                  2. Select a Fantasy League
                </span>
                <span v-else style="font-size:0.8rem; padding-bottom:5px; z-index:2">
                  2. Select a Fantasy League
                </span>
              </div>
            </v-carousel-item>
            <v-carousel-item key="3" :src="draftPlayersGif" cover>
              <div class="d-flex fill-height justify-center align-end">
                <span v-if="isDesktop" style="font-size:1.3rem; padding-bottom:20px; z-index:2">
                  3. Draft your favorite pros
                </span>
                <span v-else style="font-size:0.8rem; padding-bottom:5px; z-index:2">
                  3. Draft your favorite pros
                </span>
              </div>
            </v-carousel-item>
          </v-carousel>
        </v-card>
      </div>
    </v-row> -->
  </v-container>
</template>

<script setup lang="ts">
import loginDiscordGif from '@/assets/howto/login_discord.gif'
import chooseFantasyLeagueGif from '@/assets/howto/choose_fantasy_league.gif'
import draftPlayersGif from '@/assets/howto/draft_players.gif'
import { ref } from 'vue';
import { VContainer, VRow, VCard, VCardTitle, VCarousel, VCarouselItem } from 'vuetify/components';

const isDesktop = ref(window.outerWidth >= 600);

</script>

<style scoped>
.attentionTitle {
  color: var(--aghanims-fantasy-main-2)
}
</style>