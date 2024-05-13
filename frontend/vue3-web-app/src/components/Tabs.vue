<template>
  <div class="tabs">
    <ul class="tabs__header">
      <li v-for="title in tabTitles" :key="title" :class="{ selected: title == selectedTitle }" @click="selectedTitle = title">
        {{ title }}
      </li>
    </ul>
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import { ref, provide, useSlots } from 'vue'

const slots = useSlots()

const tabTitles = ref(slots.default!().map((tab) => tab.props?.title))
const selectedTitle = ref(tabTitles.value[0])
provide("selectedTitle", selectedTitle)

</script>

<style scoped lang="css">
  .tabs {
    max-width: 400px;
    margin: 0 auto;
  }
  
  .tabs__header {
    margin-bottom: 10px;
    list-style: none;
    padding: 0;
    display: flex;
    color: black;
  }

  .tabs__header li {
    width: 80px;
    text-align: center;
    padding: 10px 20px;
    margin-right: 10px;
    background-color: #ddd;
    border-radius: 5px;
    cursor: pointer;
    transition: 0.4s all ease-out;
  }

  .tabs__header li.selected {
    background-color: #0984e3;
    color: white;
  }
</style>