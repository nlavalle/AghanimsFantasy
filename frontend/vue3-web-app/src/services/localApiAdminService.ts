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
  },
  addFantasyPlayersByTeam(teamId: number, fantasyLeagueId: number) {
    if (!teamId || !fantasyLeagueId) return;
    return fetch(`${baseUrl}/fantasyleague/${fantasyLeagueId}/team/${teamId}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      }
    }).then(
      function (response: Response) {
        if (!response.ok) {
          throw response.status
        } else {
          return response.status
        }
      }.bind(this)
    )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  }
}
