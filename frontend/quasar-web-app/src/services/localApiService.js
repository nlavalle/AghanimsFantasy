// apiService.js

const baseUrl = '/api';

export const localApiService = {
    getLeagues() {
        return fetch(`${baseUrl}/league`)
            .then(function (response) {
                if (response.status != 200) {
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
                if (response.status != 200) {
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
        return fetch(`${baseUrl}/league/${leagueId}/players`)
            .then(function (response) {
                if (response.status != 200) {
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
        return fetch(`${baseUrl}/league/teams`)
            .then(function (response) {
                if (response.status != 200) {
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
        return fetch(`${baseUrl}/league/heroes`)
            .then(function (response) {
                if (response.status != 200) {
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
        return fetch(`${baseUrl}/league/accounts`)
            .then(function (response) {
                if (response.status != 200) {
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
};