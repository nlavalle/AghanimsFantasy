<template>
    <span class="balance-span">
        Shard Stash:
    </span>
    <ShardSpan :animated="true" :font-size="1.0" :bold="true" :gold-value="userBalance.toFixed(0)" />
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useAuthStore } from "@/stores/auth";
import { localApiService } from "@/services/localApiService";
import ShardSpan from "../Dom/ShardSpan.vue";

const authStore = useAuthStore();

const userBalance = ref(0);

onMounted(() => {
    if (authStore.authenticated) {
        localApiService.getUserBalance().then((result: any) => {
            userBalance.value = result
        });
    }
});

</script>

<style scoped>
.balance-span {
    color: var(--gradient-blue-2);
    font-weight: bold;
    margin: 5px;
}
</style>