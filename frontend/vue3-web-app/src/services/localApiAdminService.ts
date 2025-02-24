// apiAdminService.js

const baseUrl = '/api/admin'

export const localApiAdminService = {
  getAdminFantasyLeagues() {
    return fetch(`${baseUrl}/FantasyLeague`)
      .then(
        function (response: Response) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .then(
        function (data: any) {
          return data.sort((a: any, b: any) => b.id - a.id)
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  }
}
