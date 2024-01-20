import { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/IndexPage.vue') }],
  },

  {
    path: '/about',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/AboutPage.vue') }],
  },

  {
    path: '/league',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/LeaguesPage.vue') }],
  },

  {
    path: '/fantasy',
    component: () => import('layouts/MainLayout.vue'),
    children: [{ path: '', component: () => import('pages/FantasyPage.vue') }],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
];

export default routes;
