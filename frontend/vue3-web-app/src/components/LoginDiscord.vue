<template>
  <div class="discord-login" style="width:100px;max-height:48px;display:flex;align-items:center">
    <div v-if="authenticated">
      <v-btn @click="logout" density="compact" variant="outlined" size="x-small" height="40px" style="text-align:left">
        <span>Logout<font-awesome-icon class="icon" :icon="faRightFromBracket" /></span>
      </v-btn>
    </div>
    <div v-else>
      <v-btn @click="login" density="compact" variant="outlined" size="x-small" height="40px" style="text-align:left">
        <span>Login via <br />Discord<font-awesome-icon class="icon" :icon="faDiscord" /></span>
      </v-btn>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { onMounted, computed } from 'vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faRightFromBracket } from '@fortawesome/free-solid-svg-icons';
import { faDiscord } from '@fortawesome/free-brands-svg-icons';
import { VBtn } from 'vuetify/components';


const authStore = useAuthStore()

onMounted(async () => {
  await authStore.checkAuthenticatedAsync()
})

const authenticated = computed(() => {
  return authStore.authenticated
})

function login() {
  // Open a new window or popup for OAuth login
  const loginWindow = window.open('/api/auth/login', 'Login', 'width=800,height=650')

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
