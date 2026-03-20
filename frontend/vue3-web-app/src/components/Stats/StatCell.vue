<template>
    <div class="stat-cell">
        <span class="stat-pts" :class="`tier-${tier}`">{{ pts }}</span>
        <span class="stat-raw">{{ raw }}</span>
    </div>
</template>

<script setup lang="ts">
defineProps<{
    pts: string
    raw: string
    tier: 0 | 1 | 2 | 3
}>()
</script>

<style scoped>
.stat-cell {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1px;
    padding: 2px 0;
}

.stat-pts {
    font-family: var(--font-body);
    font-size: var(--text-sm);
    font-weight: 600;
    line-height: 1.3;
}

.stat-raw {
    font-family: var(--font-body);
    font-size: var(--text-xs);
    color: var(--ot-text-dim);
    line-height: 1.3;
}

/* Tier colors from design: gold → cream → muted → dim */
.tier-3 { color: var(--rune-gold); }
.tier-2 { color: var(--sg-text-muted); }
.tier-1 { color: var(--ot-text-dim); }
.tier-0 { color: color-mix(in srgb, var(--ot-text-muted) 50%, transparent); }
</style>
