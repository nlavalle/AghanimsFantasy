<template>
    <div class="countdown-container">
        <v-chip class="countdown-text" size="large">
            Time until Fantasy Locks: {{ formattedTime }}
        </v-chip>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from "vue";
import { VChip } from "vuetify/components";

const props = defineProps({
    targetTime: {
        type: Number,
        required: true
    }
});

const timeLeft = ref(0);
const interval = ref(0);

const updateTimer = () => {
    const now = Math.floor(Date.now() / 1000); // Current time in seconds
    timeLeft.value = Math.max(props.targetTime - now, 0); // Prevent negative values
};

// Format time as HH:MM:SS
const formattedTime = computed(() => {
    const h = Math.floor(timeLeft.value / 3600);
    const m = Math.floor((timeLeft.value % 3600) / 60);
    const s = timeLeft.value % 60;
    return `${String(h).padStart(2, "0")}:${String(m).padStart(2, "0")}:${String(s).padStart(2, "0")}`;
});

onMounted(() => {
    updateTimer();
    interval.value = setInterval(updateTimer, 1000);
});

onUnmounted(() => {
    clearInterval(interval.value);
});
</script>

<style scoped>
.countdown-text {
    color: var(--aghanims-fantasy-white)
}
</style>