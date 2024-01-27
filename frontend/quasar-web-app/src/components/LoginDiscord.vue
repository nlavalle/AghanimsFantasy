<template>
    <div class="login-container">
        <div v-if="authenticated">
            <span class="welcome">Welcome {{ user.name }}</span>
            <q-btn class="log-btn" @click="logout" label="Log out" />
        </div>
        <div v-else>
            <q-btn class="log-btn" @click="login" label="Login" />
        </div>
    </div>
</template>
<script>
import { useAuthStore } from 'stores/auth';
import { onMounted } from 'vue';

export default {
    name: 'LoginDiscord',
    setup() {
        const authStore = useAuthStore();

        onMounted(async () => {
            await authStore.checkAuthenticatedAsync();
        });

        return { authStore };
    },
    computed: {
        authenticated() { return this.authStore.authenticated; },
        user() { return this.authStore.user ?? {}; }
    },
    methods: {
        login() {
            // Open a new window or popup for OAuth login
            const loginWindow = window.open('/api/auth/login', 'Login', 'width=500,height=650');

            const checkLoginStatus = setInterval(() => {
                if (this.authenticated || loginWindow.closed) {
                    loginWindow.close();
                    clearInterval(checkLoginStatus);
                    this.authStore.getUser();
                    ;
                }
                this.authStore.checkAuthenticatedAsync();
            }, 1000);
        },
        logout() {
            fetch('/api/auth/signout', {
                credentials: "include" // fetch won't send cookies unless you set credentials
            })
                .then(response => {
                    if (response.ok) {
                        this.authStore.setAuthenticated(false);
                    }
                })
        }
    }
};
</script>
<style>
.login-container {
    box-sizing: border-box;
    border-radius: 5px;
    margin: auto;
    align-items: center;
}

.log-btn {
    margin: 2px;
    border-radius: 8px;
    border: 1px solid var(--nadcl-white);
}

.welcome {
    color: white;
    margin: 5px;
}
</style>