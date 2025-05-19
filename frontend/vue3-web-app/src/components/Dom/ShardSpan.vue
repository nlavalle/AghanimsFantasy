<template>
    <div class="d-flex align-center">
        <span :class="{ 'invalid': props.validation }" :style="{
            'font-weight': props.bold ? 'bold' : 'normal',
            'font-size': props.fontSize + 'rem'
        }">{{ props.goldValue }}</span>
        <img v-if="props.animated" class="shard-img" :src="shardAnimated" />
        <img v-else class="shard-img" :src="shardStatic" />
    </div>
</template>

<script setup lang="ts">
import shardStatic from '@/assets/fantasy/shard/single-shard.gif'
import shardAnimated from '@/assets/fantasy/shard/single-shard.gif'
// import shardAnimated from '@/assets/fantasy/shard/multiple-shard.gif'

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
/* .shard-span {
    color: rgb(26, 41, 231);
} */

.invalid {
    color: var(--aghanims-fantasy-accent)
}

.shard-img {
    height: 30px;
    width: 30px;
    display: inline-block;
    vertical-align: middle;
}
</style>