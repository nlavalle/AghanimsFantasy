<template>
    <v-select
        :label="label"
        :items="items"
        :model-value="modelValue"
        item-title="name"
        item-value="id"
        multiple
        clearable
        hide-details
        single-line
        density="compact"
        variant="outlined"
        class="filter-select"
        @update:model-value="emit('update:modelValue', $event ?? [])"
    />
</template>

<script setup lang="ts">
import { VSelect } from 'vuetify/components';

defineProps<{
    label: string;
    items: { id: number; name: string }[];
    modelValue: number[];
}>();

const emit = defineEmits<{
    (e: 'update:modelValue', value: number[]): void;
}>();
</script>

<style scoped>
.filter-select {
    width: 200px;
}

.filter-select :deep(.v-field) {
    background: var(--ot-bg-mid);
    border-radius: 6px;
    font-family: var(--font-body);
    font-size: var(--text-sm);
}

.filter-select :deep(.v-field__outline) {
    --v-field-border-opacity: 1;
    color: color-mix(in srgb, var(--ot-border) 20%, transparent);
}

.filter-select :deep(.v-label) {
    font-family: var(--font-body);
    font-size: var(--text-sm);
    color: var(--ot-text-dim);
}

.filter-select :deep(.v-field__input) {
    color: var(--ot-text);
    font-size: var(--text-sm);
}

.filter-select :deep(.v-icon) {
    color: var(--ot-text-dim);
}
</style>
