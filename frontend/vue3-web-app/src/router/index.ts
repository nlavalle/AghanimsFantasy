import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import { useAuthStore } from '@/stores/auth';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue')
    },
    {
      path: '/stats',
      name: 'stats',
      component: () => import('../views/StatsView.vue')
    },
    {
      path: '/fantasy',
      name: 'fantasy',
      component: () => import('../views/FantasyView.vue')
    },
    {
      path: '/profile',
      name: 'profile',
      component: () => import('../views/ProfileView.vue')
    },

    {
      path: '/admin',
      name: 'admin',
      component: () => import('../views/AdminView.vue'),
      meta: { requiresAdmin: true }
    },

    {
      path: '/privatefantasy',
      name: 'privatefantasyadmin',
      component: () => import('../views/PrivateFantasyAdminView.vue'),
      meta: { requiresPrivateFantasyAdmin: true }
    },
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();

  authStore.loadUserAuthInfo();

  if (to.meta.requiresAdmin && !authStore.user?.isAdmin) {
    next('/');
  } else if (to.meta.requiresPrivateFantasyAdmin && !authStore.user?.isPrivateFantasyAdmin) {
    next('/');
  } else {
    next();
  }
});

export default router
