<template>
    <router-link :to="to" :class="{ active: isActive }">
        <span>
            <i class="icon" :class="icon"></i>
            <slot></slot>
        </span>
    </router-link>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const props = defineProps({
    to: { type: String, required: true },
    icon: { type: String, required: true }
})

const route = useRoute()
const isActive = computed(() => route.path == props.to)
</script>

<style scoped>
.icon {
    flex-shrink: 0;
    width: 25px;
    margin-right: 10px;
}
</style>