import { createRouter, createWebHistory } from 'vue-router'
import { isAuthenticated } from '@/services/authService'

const routes = [
  { path: '/login', name: 'login', component: () => import('../pages/Login.vue') },

  { path: '/', name: 'dashboard', component: () => import('../pages/Dashboard.vue'), meta: { requiresAuth: true } },
  { path: '/assets', name: 'assets', component: () => import('../pages/Assets.vue'), meta: { requiresAuth: true } },
  { path: '/transactions', name: 'transactions', component: () => import('../pages/Transactions.vue'), meta: { requiresAuth: true } },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  if (to.meta.requiresAuth && !isAuthenticated()) {
    return { name: 'login' }
  }
})

export default router
