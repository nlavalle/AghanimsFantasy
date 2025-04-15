<template>
    <div class="d-flex align-center">
        <span :class="{ 'gold-span': true, 'invalid': props.validation }" :style="{
            'font-weight': props.bold ? 'bold' : 'normal',
            'font-size': props.fontSize + 'rem'
        }">{{ props.goldValue }}</span>
        <img v-if="props.animated" class="gold-coin" :src="coinAnimated" />
        <img v-else class="gold-coin" :src="coinStatic" />
    </div>
</template>

<script setup lang="ts">
import coinStatic from '@/assets/fantasy/coin/golden-coin.png'
import coinAnimated from '@/assets/fantasy/coin/golden-coin.gif'

const props = defineProps({
    animated: {
        type: Boolean,
        required: false,
        default: false
    },
    bold: {
        type: Boolean,
        required: false,
        default: false
    },
    fontSize: {
        type: Number,
        required: false,
        default: 0.9
    },
    validation: {
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
    color: rgb(249, 194, 43);
}

.invalid {
    color: var(--aghanims-fantasy-accent)
}

.gold-coin {
    display: inline-block;
    vertical-align: middle;
}
</style>