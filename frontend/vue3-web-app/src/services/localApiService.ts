// apiService.js

import type { FantasyPlayer } from "@/components/Fantasy/fantasyDraft"
import type { FantasyLeague } from "@/types/FantasyLeague"
import type { League } from "@/types/League"

const baseUrl = '/api'

export const localApiService = {
  getLeagues() {
    return fetch(`${baseUrl}/league`)
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
  postLeague(newLeague: League) {
    return fetch(`${baseUrl}/League`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newLeague)
    }).then(
      function (response: Response) {
        if (!response.ok) {
          throw response.status
        } else {
          return response.json()
        }
      }.bind(this)
    )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  deleteLeague(newLeague: League) {
    if (!newLeague.id) return;
    return fetch(`${baseUrl}/League/${newLeague.id}`, {
      method: 'DELETE'
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
  },
  getFantasyLeagues() {
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
  postFantasyLeague(newFantasyLeague: FantasyLeague) {
    return fetch(`${baseUrl}/FantasyLeague`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newFantasyLeague)
    }).then(
      function (response: Response) {
        if (!response.ok) {
          throw response.status
        } else {
          return response.json()
        }
      }.bind(this)
    )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  deleteFantasyLeague(deleteFantasyLeague: League) {
    if (!deleteFantasyLeague.id) return;
    return fetch(`${baseUrl}/FantasyLeague/${deleteFantasyLeague.id}`, {
      method: 'DELETE'
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
  },
  getFantasyPlayers(fantasyLeagueId: number = 0) {
    let url = `${baseUrl}/FantasyPlayer`
    if (fantasyLeagueId != 0) {
      url = url + `?FantasyLeagueId=${fantasyLeagueId}`
    }
    return fetch(url)
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
  postFantasyPlayer(newFantasyPlayer: FantasyPlayer) {
    return fetch(`${baseUrl}/FantasyPlayer`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newFantasyPlayer)
    }).then(
      function (response: Response) {
        if (!response.ok) {
          throw response.status
        } else {
          return response.json()
        }
      }.bind(this)
    )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  deleteFantasyPlayer(deleteFantasyPlayer: FantasyPlayer) {
    if (!deleteFantasyPlayer.id) return;
    return fetch(`${baseUrl}/FantasyPlayer/${deleteFantasyPlayer.id}`, {
      method: 'DELETE'
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
  },

  getLeagueMatchHistory(leagueId: number) {
    return fetch(`${baseUrl}/league/${leagueId}/match/history`)
      .then(
        function (response: any) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .then(
        function (data: any) {
          return data.sort((a: any, b: any) => b.matchId - a.matchId)
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  getLeaguePlayerData(leagueId: number) {
    return fetch(`${baseUrl}/Match/${leagueId}/players`)
      .then(
        function (response: any) {
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
  getTeams() {
    return fetch(`${baseUrl}/Team/teams`)
      .then(
        function (response: any) {
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
  getHeroes() {
    return fetch(`${baseUrl}/Hero/heroes`)
      .then(
        function (response: any) {
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
  getAccounts() {
    return fetch(`${baseUrl}/Player/accounts`)
      .then(
        function (response: any) {
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
  getFantasyDraft(leagueId: number) {
    return fetch(`${baseUrl}/fantasy/draft/${leagueId}`)
      .then(
        function (response: any) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .then(
        function (data: any) {
          return data
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  getUserDraftPoints(leagueId: number) {
    return fetch(`${baseUrl}/fantasy/draft/${leagueId}/points`)
      .then(
        function (response: any) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .then(
        function (data: any) {
          return data
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  getPlayerFantasyStats(leagueId: number) {
    return fetch(`${baseUrl}/fantasy/players/${leagueId}/points`)
      .then(
        function (response: any) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .then(
        function (data: any) {
          return data
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  getFantasyLeagueMetadataStats(leagueId: number) {
    return fetch(`${baseUrl}/fantasy/league/${leagueId}/metadata`)
      .then(
        function (response: any) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .then(
        function (data: any) {
          return data
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  getTopTenDrafts(leagueId: number) {
    return fetch(`${baseUrl}/fantasy/players/${leagueId}/top10`)
      .then(
        function (response: any) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .then(
        function (data: any) {
          return data
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  getHighlights(leagueId: number, numberOfHighlights: number) {
    return fetch(`${baseUrl}/fantasy/${leagueId}/highlights/${numberOfHighlights}`)
      .then(
        function (response: any) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .then(
        function (data: any) {
          return data
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  getPlayerTopHeroes(fantasyPlayerId: number) {
    return fetch(`${baseUrl}/player/${fantasyPlayerId}/topheroes`)
      .then(
        function (response: any) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .then(
        function (data: any) {
          return data
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  getPlayerFantasyAverages(fantasyPlayerId: number) {
    return fetch(`${baseUrl}/player/${fantasyPlayerId}/fantasyaverages`)
      .then(
        function (response: any) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .then(
        function (data: any) {
          return data
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  saveFantasyDraft(user: any, league: any, draftPickArray: FantasyPlayer[]) {
    const updateRequest = {
      FantasyLeagueId: league.id,
      DisordAccountId: user.id,
      DraftPickPlayers: [
        {
          FantasyPlayerId: draftPickArray[1]?.id,
          DraftOrder: 1
        },
        {
          FantasyPlayerId: draftPickArray[2]?.id,
          DraftOrder: 2
        },
        {
          FantasyPlayerId: draftPickArray[3]?.id,
          DraftOrder: 3
        },
        {
          FantasyPlayerId: draftPickArray[4]?.id,
          DraftOrder: 4
        },
        {
          FantasyPlayerId: draftPickArray[5]?.id,
          DraftOrder: 5
        }
      ]
    }
    return fetch(`${baseUrl}/fantasy/draft`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updateRequest)
    })
      .then(
        function (response: any) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  }
}
