// authApiService.js

const identityBaseUrl = '/identity'
const apiBaseUrl = '/api'

export const authApiService = {
  // Auth actions
  login(email: string, pass: string) {
    return fetch(`${identityBaseUrl}/login?useCookies=true`, {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        email: email,
        password: pass
      })
    }).then(
      function (response: any) {
        return response.status
      }
    )
  },
  register(email: string, pass: string) {
    return fetch(`${identityBaseUrl}/register`, {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        email: email,
        password: pass
      })
    }).then(
      function (response: any) {
        return response
      }
    )
  },
  updateDisplayName(name: string) {
    return fetch(`${apiBaseUrl}/auth/change-display-name`, {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        display_name: name
      })
    }).then(
      function (response: any) {
        return response
      }
    )
  },
  getManageInfo() {
    return fetch(`${identityBaseUrl}/manage/info`)
      .then((response: any) => {
        if (!response.ok) {
          throw response.status
        } else {
          return response.json()
        }
      })
  },
  changePassword(currentPassword: string, newPassword: string) {
    return fetch(`${identityBaseUrl}/manage/info`, {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        newPassword: newPassword,
        oldPassword: currentPassword
      })
    })
      .then((response: any) => {
        if (!response.ok) {
          throw response.status
        } else {
          return response.json()
        }
      })
  },
  downloadPersonalData() {
    return fetch(`${apiBaseUrl}/auth/download-data`, {
      method: 'POST',
      headers: {
        'accept': '*/*'
      }
    })
      .then((response: Response) => {
        if (!response.ok) {
          throw response.status
        } else {
          return response.blob()
        }
      })
      .then((blob) => {
        // console.log(blob)
        // const blob = new Blob([data], { type: 'application/json' })
        const link = document.createElement('a')
        link.href = URL.createObjectURL(blob)
        link.download = 'PersonalData'
        link.click()
        URL.revokeObjectURL(link.href)
      })
  }
}
