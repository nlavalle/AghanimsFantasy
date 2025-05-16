// apiService.js

import type { FantasyPlayer } from "@/components/Fantasy/fantasyDraft"
import type { DotaAccount, DotaTeam } from "@/types/Dota"
import type { FantasyLeague } from "@/types/FantasyLeague"
import type { FantasyLeagueWeight } from "@/types/FantasyLeagueWeight"
import type { League } from "@/types/League"
import type { PrivateFantasyPlayer } from "@/types/PrivateFantasyPlayer"

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
  putLeague(updateLeague: League) {
    if (!updateLeague.league_id) return;
    return fetch(`${baseUrl}/League/${updateLeague.league_id}`, {
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
  deleteLeague(deleteLeague: League) {
    if (!deleteLeague.league_id) return;
    return fetch(`${baseUrl}/League/${deleteLeague.league_id}`, {
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
  putFantasyLeague(updateFantasyLeague: FantasyLeague) {
    if (!updateFantasyLeague.id) return;
    return fetch(`${baseUrl}/FantasyLeague/${updateFantasyLeague.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updateFantasyLeague)
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
  deleteFantasyLeague(deleteFantasyLeague: FantasyLeague) {
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
  getFantasyLeagueWeights() {
    return fetch(`${baseUrl}/FantasyLeagueWeight`)
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
  postFantasyLeagueWeight(newFantasyLeagueWeight: FantasyLeagueWeight) {
    return fetch(`${baseUrl}/FantasyLeagueWeight`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newFantasyLeagueWeight)
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
  putFantasyLeagueWeight(updateFantasyLeagueWeight: FantasyLeagueWeight) {
    if (!updateFantasyLeagueWeight.id) return;
    return fetch(`${baseUrl}/FantasyLeagueWeight/${updateFantasyLeagueWeight.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updateFantasyLeagueWeight)
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
  deleteFantasyLeagueWeight(deleteFantasyLeagueWeight: FantasyLeagueWeight) {
    if (!deleteFantasyLeagueWeight.id) return;
    return fetch(`${baseUrl}/FantasyLeagueWeight/${deleteFantasyLeagueWeight.id}`, {
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
  getPrivateFantasyLeagueWeights() {
    return fetch(`${baseUrl}/PrivateFantasyLeague/FantasyLeagueWeight`)
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
  putPrivateFantasyLeagueWeight(updateFantasyLeagueWeight: FantasyLeagueWeight) {
    if (!updateFantasyLeagueWeight.id) return;
    return fetch(`${baseUrl}/PrivateFantasyLeague/FantasyLeagueWeight/${updateFantasyLeagueWeight.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updateFantasyLeagueWeight)
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
  getPrivateFantasyPlayers(fantasyLeagueId: number = 0) {
    return fetch(`${baseUrl}/PrivateFantasyLeague/FantasyLeague/${fantasyLeagueId}`)
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
  postPrivateFantasyPlayer(newPrivateFantasyPlayer: Partial<PrivateFantasyPlayer>) {
    return fetch(`${baseUrl}/PrivateFantasyLeague`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newPrivateFantasyPlayer)
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
  putPrivateFantasyPlayer(updatePrivateFantasyPlayer: PrivateFantasyPlayer) {
    if (!updatePrivateFantasyPlayer.id) return;
    return fetch(`${baseUrl}/PrivateFantasyLeague/${updatePrivateFantasyPlayer.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updatePrivateFantasyPlayer)
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
  deletePrivateFantasyPlayer(deletePrivateFantasyPlayer: PrivateFantasyPlayer) {
    if (!deletePrivateFantasyPlayer.id) return;
    return fetch(`${baseUrl}/PrivateFantasyLeague/${deletePrivateFantasyPlayer.id}`, {
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
  postFantasyPlayer(newFantasyPlayer: Partial<FantasyPlayer>) {
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
    return fetch(`${baseUrl}/Team`)
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
  postTeam(newTeam: DotaTeam) {
    return fetch(`${baseUrl}/Team`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newTeam)
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
  putTeam(updateTeam: DotaTeam) {
    if (!updateTeam.id) return;
    return fetch(`${baseUrl}/Team/${updateTeam.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updateTeam)
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
  deleteTeam(deleteTeam: DotaTeam) {
    if (!deleteTeam.id) return;
    return fetch(`${baseUrl}/Team/${deleteTeam.id}`, {
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
  postAccount(newAccount: DotaAccount) {
    return fetch(`${baseUrl}/Player`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(newAccount)
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
  putAccount(updateAccount: DotaAccount) {
    if (!updateAccount.id) return;
    return fetch(`${baseUrl}/Player/${updateAccount.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updateAccount)
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
  deleteAccount(deleteAccount: DotaAccount) {
    if (!deleteAccount.id) return;
    return fetch(`${baseUrl}/Player/${deleteAccount.id}`, {
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
    return fetch(`${baseUrl}/fantasydraft/${leagueId}/drafts/points`)
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
  getFantasyPlayerViewModels(fantasyLeagueId: number) {
    return fetch(`${baseUrl}/fantasyplayer/fantasyleague/${fantasyLeagueId}`)
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
  getPlayerFantasyStats(fantasyLeagueId: number) {
    return fetch(`${baseUrl}/fantasyleague/${fantasyLeagueId}/players/points`)
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
  getFantasyLeagueMetadataStats(fantasyLeagueId: number) {
    return fetch(`${baseUrl}/fantasyleague/${fantasyLeagueId}/metadata`)
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
  getPlayerFantasyMatchStats(fantasyLeagueId: number) {
    return fetch(`${baseUrl}/fantasyleague/${fantasyLeagueId}/players/matches/points?limit=100`)
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
  getLeaderboardStats(fantasyLeagueId: number) {
    return fetch(`${baseUrl}/fantasyleague/${fantasyLeagueId}/drafters/stats`)
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
  getLeagueSchedules() {
    return fetch(`${baseUrl}/league/schedule`)
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
  validateDiscordUsername(username: string) {
    return fetch(`${baseUrl}/PrivateFantasyLeague/validate/${username}`)
      .then(
        function (response: Response) {
          if (!response.ok) {
            throw response.status
          } else {
            return response.text()
          }
        }.bind(this)
      )
      .catch((error) => {
        console.error('Error fetching data:', error)
        throw error
      })
  },
  getUserBalance() {
    return fetch(`${baseUrl}/discord/balance`)
      .then(
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
  saveFantasyDraft(league: any, draftPickArray: FantasyPlayer[]) {
    let draftPicks = [];
    for (let i = 1; i < 6; i++) {
      if (draftPickArray[i]) {
        draftPicks.push({
          FantasyPlayerId: draftPickArray[i].id,
          DraftOrder: i
        })
      }
    }
    const updateRequest = {
      FantasyLeagueId: league.id,
      DraftPickPlayers: draftPicks
    }
    return fetch(`${baseUrl}/fantasydraft`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(updateRequest)
    }).then(
      function (response: any) {
        if (!response.ok) {
          return response.text().then((response: any) => { throw new Error(response) })
        } else {
          return response.json()
        }
      }.bind(this)
    ).catch((error) => {
      throw (error)
    })
  }
}
