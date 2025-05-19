import { authApiService } from '@/services/authApiService'
import { localApiService } from '@/services/localApiService'
import { defineStore } from 'pinia'

export interface User {
  id: string
  name: string
  email: string
  emailConfirmed: boolean
  discordName: string
  isAdmin: boolean
  isPrivateFantasyAdmin: boolean
  loginProviders: LoginProvider[]
  stashBalance: number | undefined
}

interface LoginProvider {
  loginProvider: string
  providerKey: string
  providerDisplayName: string
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    authenticated: false as boolean,
    user: {} as Partial<User>
  }),

  actions: {
    setAuthenticated(authenticated: boolean) {
      this.authenticated = authenticated
    },

    async checkAuthenticatedAsync() {
      await fetch('/api/auth/authenticated', {
        credentials: 'include' // fetch won't send cookies unless you set credentials
      })
        .then((response) => response.json())
        .then((data) => {
          this.authenticated = data.authenticated
          // If this is the first check and we haven't gotten user details, get that now
          if (this.authenticated && this.user.id == null) {
            this.loadUser()
          }

          // If we logged out clear the user
          if (!this.authenticated && this.user) {
            this.clearUser()
          }
        })
    },

    setUser(user: User) {
      this.user = user
    },

    clearUser() {
      this.user = {}
    },

    loadUser() {
      if (!this.authenticated) return
      this.loadUserAuthInfo()?.then(() => {
        Promise.all([
          this.loadUserManageInfo(),
          this.loadUserExternalLogins(),
          this.loadUserBalance()
        ])
      })
    },

    loadUserAuthInfo() {
      if (!this.authenticated) return
      return authApiService.getAuthInfo()
        .then((authData) => {
          this.user.id = authData.id,
            this.user.name = authData.displayName,
            this.user.discordName = authData.discordHandle,
            this.user.isAdmin = authData.roles.some((role: string) => role == "Admin"),
            this.user.isPrivateFantasyAdmin = authData.roles.some((role: string) => role == "PrivateFantasyLeagueAdmin")
        })
    },

    loadUserManageInfo() {
      return authApiService.getManageInfo().then((manageInfo) => {
        if (manageInfo) {
          this.user.email = manageInfo.email
          this.user.emailConfirmed = manageInfo.isEmailConfirmed
        }
      })
    },

    loadUserExternalLogins() {
      return authApiService.getExternalLogins().then((response) => {
        if (response) {
          this.user.loginProviders = response
        }
      })
    },

    loadUserBalance() {
      return localApiService.getUserBalance().then((response: any) => {
        this.user.stashBalance = response
      })
    },

    updateDisplayName(newDisplayName: string | undefined) {
      if (!newDisplayName) return
      return authApiService.updateDisplayName(newDisplayName).then(() => {
        this.loadUserAuthInfo()
      })
    },

    changePassword(currentPassword: string, newPassword: string) {
      return authApiService.changePassword(currentPassword, newPassword).then((manageInfo) => {
        if (manageInfo) {
          this.user.email = manageInfo.email
          this.user.emailConfirmed = manageInfo.isEmailConfirmed
        }
      })
    },

    changeEmail(newEmail: string) {
      return authApiService.changeEmail(newEmail).then((manageInfo) => {
        if (manageInfo) {
          this.user.email = manageInfo.email
          this.user.emailConfirmed = manageInfo.isEmailConfirmed
        }
      })
    },

    login(email: string, password: string) {
      if (!email || !password) return;
      return authApiService.login(email, password)
        .then((responseStatusCode: number) => {
          switch (responseStatusCode) {
            case 401:
              throw new Error("Invalid Login")
            default:
              this.checkAuthenticatedAsync()
          }
        })
    },

    register(email: string, password: string) {
      if (!email || !password) return;
      return authApiService.register(email, password)
        .then(async (response: Response) => {
          switch (response.status) {
            case 200:
              this.login(email, password)
              break;
            case 400:
              let data = await response.json();
              if (data.errors) {
                const messages = Object.values(data.errors).flat();
                throw new Error(`Bad Request: ${messages.join('; ')}`)
              } else {
                throw new Error("Bad Request")
              }
            default:
              throw new Error(`Unknown error.\n Http Status: ${response.status}\n Http Body: ${response.body ?? ''}`)
          }
        })
    },

    logout() {
      return authApiService.logout().then(async (response: Response) => {
        if (!response.ok) throw new Error(`Http Error: ${response}`)
        await this.checkAuthenticatedAsync()
      })
    }
  },
  getters: {
    isAuthenticated(): boolean {
      return this.authenticated
    },
    currentUser(): Partial<User> {
      return this.user
    }
  }
})
