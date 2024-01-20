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
    saveFantasyDraft(
        user,
        league,
        draftPickArray
    ) {
        var updateRequest = {
            leagueId: league.id,
            disordAccountId: user.id,
            draftPickOne: draftPickArray[0].id,
            draftPickTwo: draftPickArray[1].id,
            draftPickThree: draftPickArray[2].id,
            draftPickFour: draftPickArray[3].id,
            draftPickFive: draftPickArray[4].id,
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