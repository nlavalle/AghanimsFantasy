<template>
    <span class="balance-span">
        Winnings:
    </span>
    <GoldSpan :animated="true" :font-size="1.0" :gold-value="userBalance.toFixed(0)" />
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import GoldSpan from "../Dom/GoldSpan.vue";
import { useAuthStore } from "@/stores/auth";
import { localApiService } from "@/services/localApiService";

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
    color: rgb(249, 194, 43);
    margin: 5px;
}
</style>