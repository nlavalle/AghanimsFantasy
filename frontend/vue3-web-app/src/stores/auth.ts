import { defineStore } from 'pinia'

export interface User {
  id: number
  name: string
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
              name: data.find(
                (claim: { type: string }) =>
                  claim.type === 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'
              ).value,
              id: data.find(
                (claim: { type: string }) =>
                  claim.type ===
                  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'
              ).value
            }
          })
      }
    }
  }
})
