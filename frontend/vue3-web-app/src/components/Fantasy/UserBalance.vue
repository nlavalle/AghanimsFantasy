<template>
    <span class="balance-span">
        Shard Stash:
    </span>
    <ShardSpan :animated="true" :font-size="1.0" :bold="true" :gold-value="userBalance.toFixed(0)" />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useAuthStore } from "@/stores/auth";
import ShardSpan from "../Dom/ShardSpan.vue";

const authStore = useAuthStore();

const userBalance = computed(() => {
    return authStore.currentUser.stashBalance ?? 0
});

</script>

<style scoped>
.balance-span {
    color: var(--gradient-blue-2);
    font-weight: bold;
    margin: 5px;
}
</style>