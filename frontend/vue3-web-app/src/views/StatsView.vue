<template>
  <Tabs>
            <Tab title="FANTASY">FANTASY</Tab>
            <Tab title="LEAGUE">LEAGUE</Tab>
            <Tab title="MATCHES">MATCHES</Tab>
        </Tabs>
</template>

<script setup lang="ts">
import Tabs from '../components/Tabs.vue'
import Tab from '../components/Tab.vue'
</script>

<style scoped>
@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    align-items: center;
  }
}
</style>
