<template>
  <div class="discord-login" style="width:80px">
    <div v-if="authenticated">
      <font-awesome-icon class="icon" :icon="faRightFromBracket" />
      <span @click="logout">Logout</span>
    </div>
    <div v-else>
      <span @click="login"> <font-awesome-icon class="icon" :icon="faCircleUser" />Login </span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { onMounted, computed } from 'vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faRightFromBracket, faCircleUser } from '@fortawesome/free-solid-svg-icons';


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
.discord-login {
  font-size: 0.8rem;
}

.icon {
  /* flex-shrink: 0; */
  width: 20px;
  margin-right: 2px;
}
</style>
