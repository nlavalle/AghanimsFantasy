<template>
  <div class="login-modal">
    <div v-if="authStore.isAuthenticated" style="display:flex">
      <v-btn v-if="authStore.currentUser && authStore.currentUser.name != ''" class="text-none" density="compact"
        variant="flat" height="40px" to="/profile">
        Welcome, {{ authStore.currentUser!.name }}
      </v-btn>
      <v-btn @click="logout" density="compact" variant="outlined" size="x-small" height="40px" style="text-align:left">
        <span>Logout<font-awesome-icon class="icon" :icon="faRightFromBracket" /></span>
      </v-btn>
      <v-dialog v-model="updateDisplayNameDialog" style="max-width: 600px;">
        <template v-slot:default="{ isActive }">
          <v-card title="Create your username">
            <v-card-text>
              <v-text-field v-model="displayName" name="displayName" placeholder="World's best drafter" required />
              <span>You can change this later</span>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn text="Cancel" @click="isActive.value = false"></v-btn>
              <v-btn text="Save" @click="updateDisplayName()"></v-btn>
            </v-card-actions>
          </v-card>
        </template>
      </v-dialog>
    </div>
    <div v-else>
      <v-dialog>
        <template v-slot:activator="{ props: activatorProps }">
          <v-btn v-bind="activatorProps" density="compact" variant="outlined" size="x-small" height="40px"
            style="text-align:left">
            <span>Login</span>
          </v-btn>
        </template>

        <template v-slot:default="{ isActive }">
          <v-card title="Dialog">
            <v-card-text>
              <v-row>
                <v-col>
                  <LoginForm class="login-form" />
                </v-col>
                <v-col>
                  <v-row>
                    <v-btn @click="login('Discord')" density="compact" variant="outlined" size="x-small" height="40px"
                      style="text-align:left">
                      <span>Login with Discord<font-awesome-icon class="icon" :icon="faRightFromBracket" /></span>
                    </v-btn>
                  </v-row>
                  <v-row>
                    <v-btn @click="login('Google')" density="compact" variant="outlined" size="x-small" height="40px"
                      style="text-align:left">
                      <span>Login with Google<font-awesome-icon class="icon" :icon="faRightFromBracket" /></span>
                    </v-btn>
                  </v-row>
                </v-col>
              </v-row>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn text="Close Dialog" @click="isActive.value = false"></v-btn>
            </v-card-actions>
          </v-card>
        </template>
      </v-dialog>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore, type User } from '@/stores/auth'
import { onBeforeMount, ref, watch } from 'vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faRightFromBracket } from '@fortawesome/free-solid-svg-icons';
import { VRow, VCol, VBtn, VDialog, VCard, VCardActions, VCardText } from 'vuetify/components';
import LoginForm from './LoginForm.vue';
import { useRouter } from 'vue-router';


const authStore = useAuthStore()
const router = useRouter()

const updateDisplayNameDialog = ref(false)
const displayName = ref('')

watch(() => authStore.user, (user: Partial<User>) => {
  updateDisplayNameDialog.value = user?.name == ''
})

onBeforeMount(async () => {
  await authStore.checkAuthenticatedAsync()
})

async function login(provider: String) {
  // Open a new window or popup for OAuth login
  const loginWindow = window.open(`/api/auth/login-provider?provider=${provider}`, 'Login', 'width=600,height=800')

  if (loginWindow) {
    const checkLoginStatus = setInterval(() => {
      if (authStore.isAuthenticated || loginWindow.closed) {
        loginWindow.close()
        clearInterval(checkLoginStatus)
      }
      authStore.checkAuthenticatedAsync()
    }, 1000)
  }
}

const updateDisplayName = () => {
  if (!displayName.value || displayName.value == '') return
  authStore.updateDisplayName(displayName.value)?.then(() => {
    updateDisplayNameDialog.value = false
  })
}

const logout = () => {
  authStore.logout().then(() => {
    router.push({ path: '/' })
  })
}

</script>

<style scoped>
.login-modal {
  margin-right: 10px;
  font-size: 0.8rem;
  max-height: 48px;
  display: flex;
  align-items: center
}

.login-form {
  padding: 10px;
  max-width: 600px;
}

.icon {
  /* flex-shrink: 0; */
  width: 20px;
  margin-right: 2px;
}
</style>
