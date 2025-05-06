import { defineStore } from 'pinia'

export interface User {
  id: string
  name: string
  discordName: string | undefined
  isAdmin: boolean
  isPrivateFantasyAdmin: boolean
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    authenticated: false as boolean,
    user: null as User | null
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
          if (this.authenticated && this.user == null) {
            this.getUser()
          }

          // If we logged out clear the user
          if (!this.authenticated && this.user) {
            this.user == null
          }
        })
    },

    setUser(user: User | null) {
      this.user = user
    },

    clearUser() {
      this.user = null
    },

    getUser() {
      if (this.authenticated) {
        fetch('/api/auth/authorization', {
          credentials: 'include' // fetch won't send cookies unless you set credentials
        })
          .then((response) => response.json())
          .then((data) => {
            this.user = {
              id: data.id,
              name: data.displayName,
              discordName: data.discordHandle,
              isAdmin: data.roles.some((role: string) => role == "Admin"),
              isPrivateFantasyAdmin: data.roles.some((role: string) => role == "PrivateFantasyLeagueAdmin")
            }
          })
      }
    }
  },
  getters: {
    isAuthenticated(): boolean {
      return this.authenticated
    }
  }
})
