<template>
  <div v-if="authenticated">
    <span @click="logout"> <i class="icon fa-solid fa-right-from-bracket"></i>Logout </span>
  </div>
  <div v-else>
    <span @click="login"> <i class="icon fa-regular fa-circle-user"></i>Login </span>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { onMounted, computed } from 'vue'

const authStore = useAuthStore()

onMounted(async () => {
  await authStore.checkAuthenticatedAsync()
})

const authenticated = computed(() => {
  return authStore.authenticated
})

function login() {
  // Open a new window or popup for OAuth login
  const loginWindow = window.open('/api/auth/login', 'Login', 'width=500,height=650')

  if (loginWindow) {
    const checkLoginStatus = setInterval(() => {
      if (authenticated.value || loginWindow.closed) {
        loginWindow.close()
        clearInterval(checkLoginStatus)
        authStore.getUser()
      }
      authStore.checkAuthenticatedAsync()
    }, 1000)
  }
}
function logout() {
  fetch('/api/auth/signout', {
    credentials: 'include' // fetch won't send cookies unless you set credentials
  }).then((response) => {
    if (response.ok) {
      authStore.setAuthenticated(false)
    }
  })
}
</script>

<style scoped>
.icon {
  flex-shrink: 0;
  width: 25px;
  margin-right: 10px;
}
</style>
