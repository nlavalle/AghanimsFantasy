<template>
  <div class="pick-slot" :class="{ filled: isFilled, selected: isSelected, empty: !isFilled }" @click="$emit('select')">
    <div class="slot-parallelogram" :class="{ selected: isSelected, filled: isFilled }">
      <StarField />
      <!-- Filled slot content (counter-skewed) -->
      <div v-if="isFilled" class="slot-content">
        <!-- Role icon watermark (centered, portrait overlaps it) -->
        <img :src="`icons/pos_${index}.png`" class="slot-role-icon slot-role-watermark" />

        <!-- Portrait fills the slot, counter-skewed like CreateDraftPicks -->
        <img v-if="avatarUrl" :src="avatarUrl" :alt="displayName" class="slot-portrait" />
        <div v-else class="slot-portrait-placeholder">
          <font-awesome-icon :icon="['far', 'user']" class="placeholder-icon"
            :class="{ 'placeholder-icon--selected': isSelected }" />
        </div>

        <!-- Gold cost top-left -->
        <GoldSpan v-if="displayCost != null" class="slot-gold-top" :gold-value="displayCost.toString()" />

        <!-- Gradient scrim + name/pts row at bottom -->
        <div class="slot-overlay">
          <div class="slot-bottom-row">
            <span class="slot-name">{{ displayName }}</span>
            <span v-if="displayPoints != null" class="slot-pts">{{ displayPoints.toFixed(1) }} pts</span>
          </div>
        </div>
      </div>

      <!-- Empty slot content (counter-skewed) -->
      <div v-else class="slot-content slot-empty-content">
        <img :src="`icons/pos_${index}.png`" class="slot-role-icon" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { FantasyPlayer } from '@/components/Fantasy/fantasyDraft'
import GoldSpan from '@/components/Dom/GoldSpan.vue'
import StarField from '@/components/Dom/StarField.vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const props = defineProps<{
  index: number
  isSelected: boolean
  fantasyPlayer?: FantasyPlayer
  cost?: number
  points?: number
}>()

defineEmits<{
  select: []
}>()

const isFilled = computed(() => !!(props.fantasyPlayer))

const displayName = computed(() =>
  props.fantasyPlayer?.dotaAccount?.name ?? ''
)

const avatarUrl = computed(() =>
  props.fantasyPlayer?.dotaAccount?.steamProfilePicture ?? null
)

const displayPoints = computed(() =>
  props.points ?? null
)

const displayCost = computed(() =>
  props.cost != null ? Math.round(props.cost) : null
)
</script>

<style scoped>
.pick-slot {
  position: relative;
  width: clamp(120px, 17vw, 240px);
  height: clamp(65px, 9vw, 130px);
  cursor: pointer;
  flex-shrink: 1;
  z-index: 1;
  margin-right: 0px;
}

.slot-parallelogram {
  width: 100%;
  height: 100%;
  overflow: hidden;
  transform: skew(20deg);
  background: color-mix(in srgb, var(--rune-purple-deep) 15%, var(--ot-bg-deep));
  border: 1px solid color-mix(in srgb, var(--ot-border) 20%, transparent);
  transition: border-color 0.2s, box-shadow 0.2s;
}

.slot-parallelogram:hover {
  border-color: color-mix(in srgb, var(--rune-purple-dark) 40%, transparent);
}

.slot-parallelogram.filled {
  border-color: color-mix(in srgb, var(--rune-purple-dark) 53%, transparent);
  background: color-mix(in srgb, var(--rune-purple-deep) 8%, var(--sg-bg-deep));
}

.slot-parallelogram.selected {
  border-color: var(--rune-purple-light);
  box-shadow:
    0 0 8px color-mix(in srgb, var(--rune-purple) 70%, transparent),
    0 0 20px color-mix(in srgb, var(--rune-purple) 50%, transparent),
    0 0 40px color-mix(in srgb, var(--rune-purple) 25%, transparent);
}

/* Counter-skew the filled content layer */
.slot-content {
  position: absolute;
  inset: 0;
  transform: skew(-20deg);
}

/* Portrait fills the slot exactly like CreateDraftPicks img */
.slot-portrait {
  position: absolute;
  left: 25%;
  height: 100%;
}

.slot-portrait-placeholder {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.placeholder-icon {
  font-size: 36px;
  color: color-mix(in srgb, var(--rune-purple-dark) 27%, transparent);
}

.placeholder-icon--selected {
  color: color-mix(in srgb, var(--rune-purple) 80%, transparent);
}

/* GoldSpan pinned top-left, counter-skewed to read upright */
.slot-gold-top {
  position: absolute;
  top: 5px;
  z-index: 2;
}

/* Dark gradient scrim at bottom so text is always readable */
.slot-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 4px 10px 6px;
  background: linear-gradient(to top,
      color-mix(in srgb, var(--sg-bg-deep) 92%, transparent) 0%,
      color-mix(in srgb, var(--sg-bg-deep) 60%, transparent) 60%,
      transparent 100%);
}

.slot-bottom-row {
  position: relative;
  display: flex;
  justify-content: center;
  align-items: baseline;
}

.slot-name {
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  font-weight: 700;
  color: var(--aghanims-fantasy-white);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  text-shadow: 0 1px 3px rgba(0, 0, 0, 0.8);
  text-align: center;
}

.slot-pts {
  position: absolute;
  right: -5px;
  bottom: 0px;
  font-family: var(--font-body);
  font-size: var(--text-sm);
  font-weight: 600;
  color: color-mix(in srgb, var(--rune-purple-light) 90%, transparent);
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.8);
  white-space: nowrap;
}

.slot-empty-content {
  justify-content: center;
  align-items: center;
}

/* Role icon — always centered, portrait covers it when filled */
.slot-role-icon {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 120px;
  height: 120px;
  object-fit: contain;
  opacity: 0.6;
}

.slot-role-watermark {
  opacity: 0.15;
}
</style>
