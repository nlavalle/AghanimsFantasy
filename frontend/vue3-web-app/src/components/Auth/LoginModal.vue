<template>
  <div class="login-modal">
    <div v-if="authStore.isAuthenticated" style="display:flex">
      <v-btn v-if="!display.mobile.value" class="text-none" density="compact" variant="flat" height="40px"
        to="/profile">
        Welcome, {{ isCurrentNameBlank ? 'Unknown User' : authStore.currentUser.name }}
      </v-btn>
      <v-btn v-else class="text-none mr-2" density="compact" variant="flat" height="40px" width="40px" to="/profile"
        icon="fa-regular fa-circle-user">
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
              <br />
              <span style="font-size:0.8rem">* If it's offensive I'm choosing it</span>
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
          <v-card title="Sign In">
            <v-card-text>
              <v-row justify="center" align="center">
                <v-col :cols="12" :sm="10" :md="8" :lg="6">
                  <LoginForm class="login-form" />
                  <v-divider class="my-6" />
                  <div class="text-center text-subtitle-2 mb-4">
                    <span>Or sign in with one of these providers:</span>
                  </div>
                  <v-row dense>
                    <v-col class="d-flex justify-center mb-2" :cols="12" :sm="6">
                      <v-btn @click="login('Discord')" density="compact" variant="outlined" size="x-small" height="40px"
                        style="text-align:left; background-color: #7289DA;">
                        <font-awesome-icon class="icon" style="height: 20px; width: 20px;"
                          :icon="faDiscord" /><span>Sign In with Discord</span>
                      </v-btn>
                    </v-col>
                    <v-col class="d-flex justify-center mb-2" :cols="12" :sm="6">
                      <img :src="GoogleSignIn" alt="Google logo" style="height: 40px; width: 175px; cursor: pointer;"
                        @click="login('Google')" />
                    </v-col>
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
import { computed, onBeforeMount, ref, watch } from 'vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faRightFromBracket } from '@fortawesome/free-solid-svg-icons';
import { VRow, VCol, VDivider, VBtn, VDialog, VCard, VCardActions, VCardText, VImg } from 'vuetify/components';
import LoginForm from './LoginForm.vue';
import { useRouter } from 'vue-router';
import { useDisplay } from 'vuetify';
import { faDiscord } from '@fortawesome/free-brands-svg-icons';
import GoogleSignIn from '@/assets/icons/google-sign-in.png'


const authStore = useAuthStore()
const router = useRouter()
const display = useDisplay()

const updateDisplayNameDialog = ref(false)
const displayName = ref('')

watch(() => authStore.currentUser, (user: Partial<User>) => {
  updateDisplayNameDialog.value = !user.name || user.name == ''
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

const isCurrentNameBlank = computed(() => {
  return !authStore.currentUser.name || authStore.currentUser.name == ''
})

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
