<template>
  <div>
    <div class="new-pick-bar">
      <DraftPickSlot v-for="index in 5" :key="index" :index="index" :fantasy-player="fantasyDraftPicks[index]"
        :cost="fantasyDraftPicks[index] ? Number(getPlayerCost(fantasyDraftPicks[index].dotaAccount.id)) : undefined"
        :points="getPlayerPoints(index)" :is-selected="currentActiveDraftPlayerCheck(index)"
        @select="changeActiveDraftPlayer(index)" />

      <div class="bar-right">
        <div class="bar-info-cards">
          <DraftBudgetCard />
          <DraftRoundCard :current="3" :total="5" />
          <DraftTimerCard v-if="leagueStore.selectedFantasyLeague"
            :target-time="leagueStore.selectedFantasyLeague.fantasyDraftLocked" />
        </div>
        <div class="bar-actions">
          <button class="btn-save" :disabled="!canSave" :class="{ 'btn-save--disabled': !canSave }"
            @click="$emit('saveDraft')">
            <font-awesome-icon :icon="['fas', 'check-square']" />
            <span>Save Draft</span>
          </button>
          <button class="btn-clear" @click="$emit('clearDraft')">
            <font-awesome-icon :icon="['fas', 'times-circle']" />
            <span>Clear Draft</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { fantasyDraftState } from '../fantasyDraft';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import DraftPickSlot from '@/components/Fantasy/CreateDraft/DraftPickSlot.vue';
import DraftRoundCard from '@/components/Fantasy/CreateDraft/DraftRoundCard.vue';
import DraftTimerCard from '@/components/Fantasy/CreateDraft/DraftTimerCard.vue';
import DraftBudgetCard from '@/components/Fantasy/CreateDraft/DraftBudgetCard.vue';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';

defineProps<{ canSave: boolean }>()
defineEmits(['clearDraft', 'saveDraft'])

const { selectedPlayer, currentDraftSlotSelected, fantasyDraftPicks, fantasyPlayerPointsAvailable } = fantasyDraftState();
const leagueStore = useFantasyLeagueStore();

const changeActiveDraftPlayer = (activeDraftPlayerSlot: number) => {
  currentDraftSlotSelected.value = activeDraftPlayerSlot;
  selectedPlayer.value = fantasyDraftPicks.value[activeDraftPlayerSlot];
}

const currentActiveDraftPlayerCheck = (draftSlot: number) => {
  return draftSlot === currentDraftSlotSelected.value;
}

const getPlayerCost = (accountId: number) => {
  return leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.dotaAccountId === accountId)?.cost.toFixed(0) ?? 0;
}

const getPlayerPoints = (slotIndex: number) => {
  const player = fantasyDraftPicks.value[slotIndex]
  if (!player) return undefined
  return fantasyPlayerPointsAvailable.value.find(pp => pp.fantasyPlayer.id === player.id)?.totalMatchFantasyPoints
}
</script>

<style scoped>
.new-pick-bar {
  position: relative;
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 32px;
  background: linear-gradient(180deg, #22222E 0%, #171720 60%, #111118 100%);
  border-bottom: 1px solid transparent;
  border-image: linear-gradient(90deg, transparent, color-mix(in srgb, var(--sg-border) 13%, transparent), transparent) 1;
  box-shadow: 0 6px 24px rgba(0, 0, 0, 0.8), 0 12px 40px rgba(0, 0, 0, 0.5);
  overflow: hidden;
}

.bar-right {
  margin-left: auto;
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 6px;
  flex-shrink: 0;
}

.bar-info-cards {
  display: flex;
  gap: 8px;
  align-items: center;
}

.bar-actions {
  display: flex;
  flex-direction: row;
  gap: 6px;
  align-items: center;
  width: 100%;
}

.btn-clear {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  height: 28px;
  padding: 0 10px;
  border-radius: 6px;
  border: 1px solid color-mix(in srgb, var(--ot-border) 20%, transparent);
  background: transparent;
  cursor: pointer;
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 600;
  color: color-mix(in srgb, var(--ot-text-dim) 50%, transparent);
  transition: border-color 0.2s, color 0.2s;
}

.btn-clear:hover {
  border-color: color-mix(in srgb, var(--aghanims-fantasy-accent) 60%, transparent);
  color: var(--aghanims-fantasy-accent);
}

.btn-save {
  display: flex;
  flex: 1;
  align-items: center;
  justify-content: center;
  gap: 8px;
  height: 52px;
  padding: 0 20px;
  border-radius: 8px;
  border: 2px solid #A8956A;
  background: linear-gradient(135deg, #A8956A, #7A6A48);
  box-shadow: 0 0 24px rgba(168, 149, 106, 0.5);
  cursor: pointer;
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  font-weight: 800;
  color: var(--sg-text);
  transition: all 0.2s;
}

.btn-save:hover:not(:disabled) {
  background: linear-gradient(135deg, #BEA97A, #8A7A58);
  box-shadow: 0 0 36px rgba(168, 149, 106, 0.7);
}

.btn-save--disabled {
  opacity: 0.4;
  cursor: not-allowed;
  box-shadow: none;
}
</style>
