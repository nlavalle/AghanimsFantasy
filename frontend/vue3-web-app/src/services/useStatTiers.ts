import { computed } from 'vue'
import type { ComputedRef } from 'vue'

const PT_COLS = [
    ['totalKills', (r: any) => r.totalKillsPoints],
    ['totalDeaths', (r: any) => r.totalDeathsPoints],
    ['totalAssists', (r: any) => r.totalAssistsPoints],
    ['totalLastHits', (r: any) => r.totalLastHitsPoints],
    ['totalGoldPerMin', (r: any) => r.totalGoldPerMinPoints],
    ['totalXpPerMin', (r: any) => r.totalXpPerMinPoints],
    ['totalSupportGoldSpent', (r: any) => r.totalSupportGoldSpentPoints],
    ['totalObsPlaced', (r: any) => r.totalObserverWardsPlacedPoints],
    ['totalSentriesPlaced', (r: any) => r.totalSentryWardsPlacedPoints],
    ['totalWardsDewarded', (r: any) => r.totalWardsDewardedPoints],
    ['totalCampsStacked', (r: any) => r.totalCampsStackedPoints],
    ['totalHeroDamage', (r: any) => r.totalHeroDamagePoints],
    ['totalTowerDamage', (r: any) => r.totalTowerDamagePoints],
    ['totalHeroHealing', (r: any) => r.totalHeroHealingPoints],
    ['totalStunDuration', (r: any) => r.totalStunDurationPoints],
] as const

export function useStatTiers(rows: ComputedRef<any[]>) {
    const colRanges = computed(() => {
        const ranges: Record<string, { min: number; max: number }> = {}
        for (const [key, getter] of PT_COLS) {
            const vals = rows.value.map(getter as (r: any) => number)
            ranges[key] = { min: Math.min(...vals), max: Math.max(...vals) }
        }
        return ranges
    })

    const getTier = (pts: number, colKey: string): 0 | 1 | 2 | 3 => {
        const range = colRanges.value[colKey]
        if (!range || range.max === range.min) return 2
        const norm = (pts - range.min) / (range.max - range.min)
        if (norm >= 0.75) return 3
        if (norm >= 0.5) return 2
        if (norm >= 0.25) return 1
        return 0
    }

    return { getTier }
}
