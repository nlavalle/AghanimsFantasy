<template>
    <div class="d-flex align-center">
        <span class="gold-span" :style="{
            'font-weight': props.bold ? 'bold' : 'normal'
        }">{{ props.goldValue }}</span>
        <img v-if="props.animated" class="gold-coin" :src="coinAnimated" />
        <img v-else class="gold-coin" :src="coinStatic" />
    </div>
</template>

<script setup lang="ts">
import coinStatic from '@/assets/fantasy/coin/golden-coin.png'
import coinAnimated from '@/assets/fantasy/coin/golden-coin.gif'

const props = defineProps({
    bold: {
        type: Boolean,
        required: false,
        default: false
    },
    animated: {
        type: Boolean,
        required: false,
        default: false
    },
    goldValue: {
        type: String,
        required: true
    },
})
</script>

<style lang="css" scoped>
.gold-span {
    font-size: 0.9rem;
    color: rgb(249, 194, 43);
}

.gold-coin {
    display: inline-block;
    vertical-align: middle;
}
</style>