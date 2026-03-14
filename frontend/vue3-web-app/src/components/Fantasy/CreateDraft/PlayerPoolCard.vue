<template>
  <div class="pool-card"
    :class="{ 'pool-card--selected': isSelected, 'pool-card--disabled': isDisabled, 'pool-card--drafted': isDrafted }">
    <div class="card-avatar">
      <img v-if="fantasyPlayer?.dotaAccount?.steamProfilePicture" :src="fantasyPlayer.dotaAccount.steamProfilePicture"
        :alt="fantasyPlayer.dotaAccount.name" class="avatar-img" />
      <div v-else class="avatar-placeholder">
        <font-awesome-icon :icon="['far', 'user']" class="avatar-icon" />
      </div>
      <img v-if="fantasyPlayer?.teamId" :src="`logos/teams_logo_${fantasyPlayer.teamId}.png`" class="team-badge" />
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
import type { FantasyPlayer } from '../fantasyDraft'
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
  display: flex;
  flex-direction: column;
  border-radius: 6px;
  background: var(--ot-bg-deep);
  border: 1px solid color-mix(in srgb, var(--ot-border) 25%, transparent);
  overflow: hidden;
  cursor: pointer;
  transition: border-color 0.2s, box-shadow 0.2s, background 0.2s;
}

.pool-card:hover {
  border-color: color-mix(in srgb, var(--ot-border) 50%, transparent);
}

.pool-card--selected {
  background: var(--sg-bg-deep);
  border-color: color-mix(in srgb, var(--sg-border) 60%, transparent);
  box-shadow:
    0 0 6px color-mix(in srgb, var(--sg-border) 25%, transparent),
    0 0 16px color-mix(in srgb, var(--sg-border) 12%, transparent);
}

.pool-card--drafted {
  border-color: color-mix(in srgb, var(--rune-purple) 90%, transparent);
  box-shadow:
    0 0 6px color-mix(in srgb, var(--rune-purple) 60%, transparent),
    0 0 16px color-mix(in srgb, var(--rune-purple) 30%, transparent);
}

.pool-card--drafted .card-info {
  background: color-mix(in srgb, var(--rune-purple) 15%, transparent);
  border-top-color: color-mix(in srgb, var(--rune-purple) 45%, transparent);
}

.pool-card--drafted .card-name {
  color: var(--rune-purple-light);
  text-shadow: 0 0 8px color-mix(in srgb, var(--rune-purple-light) 40%, transparent);
}

.pool-card--drafted .card-pts {
  color: var(--rune-purple-muted);
}

.pool-card--disabled {
  opacity: 0.6;
  /* cursor: not-allowed; */
  filter: grayscale(0.8);
}

.pool-card--drafted.pool-card--disabled {
  filter: none;
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

/* Info area */
.card-info {
  display: flex;
  flex-direction: column;
  gap: 1px;
  padding: 4px 6px;
  background: color-mix(in srgb, var(--ot-bg-mid) 80%, transparent);
  border-top: 1px solid color-mix(in srgb, var(--ot-border) 15%, transparent);
}

.pool-card--selected .card-info {
  background: color-mix(in srgb, var(--sg-bg-mid) 80%, transparent);
  border-top-color: color-mix(in srgb, var(--sg-border) 25%, transparent);
}

.card-name {
  font-family: var(--font-heading);
  font-size: var(--text-xs);
  font-weight: 700;
  color: color-mix(in srgb, var(--ot-text) 70%, transparent);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.pool-card--selected .card-name {
  color: var(--sg-text);
}

.card-pts {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 400;
  color: color-mix(in srgb, var(--ot-text-dim) 50%, transparent);
}

.pool-card--selected .card-pts {
  color: color-mix(in srgb, var(--sg-text-dim) 70%, transparent);
}

.card-stats {
  display: flex;
  align-items: center;
}
</style>
