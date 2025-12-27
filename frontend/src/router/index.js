import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  { path: '/', name: 'dashboard', component: () => import('../pages/Dashboard.vue') },
  { path: '/assets', name: 'assets', component: () => import('../pages/Assets.vue') },
  { path: '/transactions', name: 'transactions', component: () => import('../pages/Transactions.vue') },
  { path: '/login', name: 'login', component: () => import('../pages/Login.vue') },
  { path: '/settings', name: 'settings', component: () => import('../pages/Settings.vue') }
]

export default createRouter({
  history: createWebHistory(),
  routes
})
