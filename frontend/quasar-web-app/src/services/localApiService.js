// apiService.js

const baseUrl = '/api';

export const localApiService = {
    getLeagues() {
        return fetch(`${baseUrl}/league`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data.sort((a, b) => b.id - a.id);
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    },
    getLeagueMatchHistory(leagueId) {
        return fetch(`${baseUrl}/league/${leagueId}/match/history`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data.sort((a, b) => b.matchId - a.matchId);
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    },
    getLeaguePlayerData(leagueId) {
        return fetch(`${baseUrl}/Match/${leagueId}/players`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data.sort((a, b) => b.id - a.id);
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    },
    getTeams() {
        return fetch(`${baseUrl}/Team/teams`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data.sort((a, b) => b.id - a.id);
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    },
    getHeroes() {
        return fetch(`${baseUrl}/Hero/heroes`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data.sort((a, b) => b.id - a.id);
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    },
    getAccounts() {
        return fetch(`${baseUrl}/Player/accounts`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data.sort((a, b) => b.id - a.id);
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });

    },
    getFantasyPlayers(leagueId) {
        return fetch(`${baseUrl}/fantasy/players/${leagueId}`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data;
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    },
    getFantasyDraft(leagueId) {
        return fetch(`${baseUrl}/fantasy/draft/${leagueId}`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data;
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    },
    getUserDraftPoints(leagueId) {
        return fetch(`${baseUrl}/fantasy/draft/${leagueId}/points`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data;
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    },
    getPlayerFantasyStats(leagueId) {
        return fetch(`${baseUrl}/fantasy/players/${leagueId}/points`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data;
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    },
    getTopTenDrafts(leagueId) {
        return fetch(`${baseUrl}/fantasy/players/${leagueId}/top10`)
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .then(function (data) {
                return data;
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    },
    saveFantasyDraft(
        user,
        league,
        draftPickArray
    ) {
        // var updateRequest = {
        //     leagueId: league.id,
        //     disordAccountId: user.id,
        //     draftPickOne: draftPickArray[0]?.id ?? 0,
        //     draftPickTwo: draftPickArray[1]?.id ?? 0,
        //     draftPickThree: draftPickArray[2]?.id ?? 0,
        //     draftPickFour: draftPickArray[3]?.id ?? 0,
        //     draftPickFive: draftPickArray[4]?.id ?? 0,
        // }
        var updateRequest = {
            LeagueId: league.id,
            DisordAccountId: user.id,
            DraftPickPlayers: [
                {
                    FantasyPlayerId: draftPickArray[0]?.id,
                    DraftOrder: 1,
                },
                {
                    FantasyPlayerId: draftPickArray[1]?.id,
                    DraftOrder: 2,
                },
                {
                    FantasyPlayerId: draftPickArray[2]?.id,
                    DraftOrder: 3,
                },
                {
                    FantasyPlayerId: draftPickArray[3]?.id,
                    DraftOrder: 4,
                },
                {
                    FantasyPlayerId: draftPickArray[4]?.id,
                    DraftOrder: 5,
                },
            ]
        }
        return fetch(`${baseUrl}/fantasy/draft`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(updateRequest),
        })
            .then(function (response) {
                if (!response.ok) {
                    throw response.status;
                } else {
                    return response.json();
                }
            }.bind(this))
            .catch(error => {
                console.error('Error fetching data:', error);
                throw error;
            });
    }
};