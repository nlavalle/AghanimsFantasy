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
          <v-icon :size="36" :color="isSelected ? '#9D4EDDCC' : '#7B2FBE44'">mdi-account</v-icon>
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

const props = defineProps<{
  index: number
  isSelected: boolean
  // Real data path
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
  width: 240px;
  height: 130px;
  cursor: pointer;
  flex-shrink: 0;
  z-index: 1;
}

.slot-parallelogram {
  width: 100%;
  height: 100%;
  overflow: hidden;
  transform: skew(20deg);
  background: rgba(14, 9, 26, 0.45);
  border: 1px solid rgba(138, 138, 158, 0.2);
  transition: border-color 0.2s, box-shadow 0.2s;
}

.slot-parallelogram:hover {
  border-color: rgba(123, 47, 190, 0.4);
}

.slot-parallelogram.filled {
  border-color: rgba(123, 47, 190, 0.53);
  background: rgba(7, 5, 15, 0.5);
}

.slot-parallelogram.selected {
  border-color: #C084FC;
  box-shadow: 0 0 8px rgba(157, 78, 221, 0.7), 0 0 20px rgba(157, 78, 221, 0.5), 0 0 40px rgba(157, 78, 221, 0.25);
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
  /* width: 100%; */
  height: 100%;
}

.slot-portrait-placeholder {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
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
  background: linear-gradient(to top, rgba(5, 3, 12, 0.92) 0%, rgba(5, 3, 12, 0.6) 60%, transparent 100%);
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
  color: rgba(192, 132, 252, 0.9);
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
