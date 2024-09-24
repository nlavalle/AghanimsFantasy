// apiService.js

import type { FantasyPlayer } from "@/components/Fantasy/fantasyDraft"
import type { FantasyLeague } from "@/types/FantasyLeague"
import type { League } from "@/types/League"

const baseUrl = '/api'

export const localApiService = {
  getLeagues(include_inactive = 'false') {
    let url = `${baseUrl}/league?` + new URLSearchParams({
      include_inactive: include_inactive
    })
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
  postLeague(newLeague: League) {
    return fetch(`${baseUrl}/league`, {
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
  putLeague(newLeague: League) {
    if (!newLeague.id) return;
    return fetch(`${baseUrl}/League/${newLeague.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newLeague)
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
  putFantasyLeague(updateLeague: FantasyLeague) {
    if (!updateLeague.id) return;
    return fetch(`${baseUrl}/FantasyLeague/${updateLeague.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updateLeague)
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
    return fetch(`${baseUrl}/FantasyLeague/${fantasyLeagueId}/players`)
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
  putFantasyPlayer(updateFantasyPlayer: FantasyPlayer) {
    if (!updateFantasyPlayer.id) return;
    return fetch(`${baseUrl}/FantasyPlayer/${updateFantasyPlayer.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updateFantasyPlayer)
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
  getUserDraftPoints(fantasyLeagueId: number) {
    return fetch(`${baseUrl}/fantasydraft/${fantasyLeagueId}/points`)
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
  getPlayerFantasyMatchStats(leagueId: number) {
    return fetch(`${baseUrl}/fantasy/players/${leagueId}/matches/points?limit=100`)
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
  getDraftPlayerFantasyMatchStats(fantasyLeagueId: number) {
    return fetch(`${baseUrl}/fantasydraft/${fantasyLeagueId}/matches/points?limit=100`)
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
  getTopTenDrafts(fantasyLeagueId: number) {
    return fetch(`${baseUrl}/fantasyleague/${fantasyLeagueId}/drafters/top10`)
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
    let draftPicks = [];
    for (let i = 1; i < 6; i++) {
      if (draftPickArray[i]) {
        draftPicks.push({
          FantasyPlayerId: draftPickArray[1].id,
          DraftOrder: i
        })
      }
    }
    const updateRequest = {
      FantasyLeagueId: league.id,
      DisordAccountId: user.id,
      DraftPickPlayers: draftPicks
    }
    return fetch(`${baseUrl}/fantasydraft`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updateRequest)
    })
      .then(
        function (response: any) {
          if (!response.ok) {
            return response.text().then((response: any) => { throw new Error(response) })
          } else {
            return response.json()
          }
        }.bind(this)
      )
      .catch((error) => {
        throw (error)
      })
  }
}
