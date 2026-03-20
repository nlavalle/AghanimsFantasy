<template>
    <div class="filter-bar">
        <div class="filter-search">
            <font-awesome-icon :icon="['fas', 'magnifying-glass']" class="filter-search-icon" />
            <input
                :value="search"
                class="filter-input"
                placeholder="Search"
                @input="emit('update:search', ($event.target as HTMLInputElement).value)"
            />
        </div>
        <FilterSelect
            label="Filter Role"
            :items="roleList"
            :model-value="roleFilter"
            @update:model-value="emit('update:roleFilter', $event)"
        />
        <FilterSelect
            label="Filter Team"
            :items="teams"
            :model-value="teamFilter"
            @update:model-value="emit('update:teamFilter', $event)"
        />
    </div>
</template>

<script setup lang="ts">
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import FilterSelect from '@/components/Stats/FilterSelect.vue';

defineProps<{
    roleFilter: number[];
    teamFilter: number[];
    teams: { id: number; name: string }[];
    search: string;
}>();

const emit = defineEmits<{
    (e: 'update:roleFilter', value: number[]): void;
    (e: 'update:teamFilter', value: number[]): void;
    (e: 'update:search', value: string): void;
}>();

const roleList = [
    { id: 1, name: 'Carry' },
    { id: 2, name: 'Mid' },
    { id: 3, name: 'Offlane' },
    { id: 4, name: 'Soft Support' },
    { id: 5, name: 'Hard Support' },
];
</script>

<style scoped>
.filter-bar {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 8px 0 12px;
}

.filter-search {
  display: flex;
  align-items: center;
  gap: 8px;
  height: 40px;
  padding: 0 14px;
  border-radius: 6px;
  background: var(--ot-bg-mid);
  border: 1px solid color-mix(in srgb, var(--ot-border) 20%, transparent);
  width: 320px;
}

.filter-search-icon {
  color: var(--ot-text-dim);
  font-size: var(--text-sm);
  flex-shrink: 0;
}

.filter-input {
  flex: 1;
  background: transparent;
  border: none;
  outline: none;
  font-family: var(--font-body);
  font-size: var(--text-sm);
  color: var(--ot-text);
}

.filter-input::placeholder {
  color: color-mix(in srgb, var(--ot-text-muted) 40%, transparent);
}
</style>
