<template>
  <div class="pool-card" :class="{ 'pool-card--selected': isSelected, 'pool-card--disabled': isDisabled }">
    <div class="card-avatar">
      <img v-if="fantasyPlayer?.dotaAccount?.steamProfilePicture" :src="fantasyPlayer.dotaAccount.steamProfilePicture"
        :alt="fantasyPlayer.dotaAccount.name" class="avatar-img" />
      <div v-else class="avatar-placeholder">
        <font-awesome-icon :icon="['far', 'user']" class="avatar-icon" />
      </div>
      <img v-if="fantasyPlayer?.teamId" :src="`logos/teams_logo_${fantasyPlayer.teamId}.png`" class="team-badge" />
      <div v-if="isDrafted" class="drafted-overlay">
        <font-awesome-icon :icon="['fas', 'check']" class="drafted-icon" />
      </div>
    </div>
    <div class="card-info">
      <span class="card-name">{{ fantasyPlayer?.dotaAccount?.name ?? '' }}</span>
      <div class="card-stats">
        <template v-if="showCost">
          <GoldSpan :font-size="0.65" :validation="overBudget" :gold-value="costDisplay" />
        </template>
        <span v-else class="card-pts">{{ fantasyPoints?.toFixed(0) ?? '0' }} pts</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { FantasyPlayer } from '@/components/Fantasy/fantasyDraft'
import GoldSpan from '@/components/Dom/GoldSpan.vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const props = defineProps<{
  fantasyPlayer?: FantasyPlayer
  fantasyPoints?: number
  fantasyPlayerCost?: number
  fantasyPlayerBudget?: number
  showCost?: boolean
  isSelected?: boolean
  isDisabled?: boolean
  isDrafted?: boolean
}>()

const costDisplay = computed(() => props.fantasyPlayerCost?.toFixed(0) ?? '0')
const overBudget = computed(() => (props.fantasyPlayerBudget ?? 0) - (props.fantasyPlayerCost ?? 0) < 0)
</script>

<style scoped>
.pool-card {
  --bg: var(--ot-bg-deep);
  --border: color-mix(in srgb, var(--rune-purple) 90%, transparent);
  --text: var(--rune-purple-light);
  --text-dim: var(--rune-purple-muted);
  --info-bg: color-mix(in srgb, var(--rune-purple) 15%, transparent);
  --info-border: color-mix(in srgb, var(--rune-purple) 45%, transparent);
  --glow:
    0 0 6px color-mix(in srgb, var(--rune-purple) 60%, transparent),
    0 0 16px color-mix(in srgb, var(--rune-purple) 30%, transparent);

  display: flex;
  flex-direction: column;
  border-radius: 6px;
  background: var(--bg);
  border: 1px solid var(--border);
  overflow: hidden;
  cursor: pointer;
  transition: border-color 0.2s, box-shadow 0.2s, background 0.2s;
  box-shadow: var(--glow);
}

.pool-card .card-name {
  text-shadow: 0 0 8px color-mix(in srgb, var(--rune-purple-light) 40%, transparent);
}

.pool-card--selected {
  --bg: color-mix(in srgb, var(--sg-glow) 80%, var(--sg-bg-deep));
  --border: color-mix(in srgb, var(--sg-border) 80%, transparent);
  --text: var(--sg-text);
  --text-dim: color-mix(in srgb, var(--sg-text-dim) 70%, transparent);
  --info-bg: color-mix(in srgb, var(--sg-bg-mid) 90%, transparent);
  --info-border: color-mix(in srgb, var(--sg-border) 40%, transparent);
  --glow:
    0 0 8px color-mix(in srgb, var(--sg-border) 60%, transparent),
    0 0 18px color-mix(in srgb, var(--sg-border) 35%, transparent),
    0 0 28px color-mix(in srgb, var(--sg-border) 18%, transparent);
}

.pool-card--selected .card-name {
  text-shadow: 0 0 8px color-mix(in srgb, var(--sg-text) 50%, transparent);
}

.pool-card--disabled {
  opacity: 0.6;
  filter: grayscale(0.8);
}

/* Avatar area */
.card-avatar {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  aspect-ratio: 1;
  background: color-mix(in srgb, var(--ot-border) 4%, transparent);
  overflow: hidden;
}

.avatar-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: top center;
}

.avatar-placeholder {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  color: color-mix(in srgb, var(--ot-border) 25%, transparent);
  font-size: var(--text-xl);
}

.avatar-icon {
  opacity: 0.5;
}

.team-badge {
  position: absolute;
  bottom: 2px;
  right: 2px;
  width: 20px;
  height: 20px;
  border-radius: 2px;
  background: rgba(0, 0, 0, 0.6);
  object-fit: contain;
}

.drafted-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.45);
}

.drafted-icon {
  width: 80px;
  height: 80px;
  color: var(--sg-text);
  filter: drop-shadow(0 0 6px color-mix(in srgb, var(--sg-border) 80%, transparent));
}

/* Info area */
.card-info {
  display: flex;
  flex-direction: column;
  gap: 1px;
  padding: 4px 6px;
  background: var(--info-bg);
  border-top: 1px solid var(--info-border);
}

.card-name {
  font-family: var(--font-heading);
  font-size: var(--text-xs);
  font-weight: 700;
  color: var(--text);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.card-pts {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 400;
  color: var(--text-dim);
}

.card-stats {
  display: flex;
  align-items: center;
}
</style>
