import { createRouter, createWebHistory } from 'vue-router'
import { isAuthenticated } from '@/services/authService'

const routes = [
  // LOGIN (be layout)
  {
    path: '/login',
    name: 'login',
    component: () => import('../pages/Login.vue')
  },

  // APP su layout
  {
    path: '/',
    component: () => import('../layouts/DefaultLayout.vue'),
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        name: 'dashboard',
        component: () => import('../pages/Dashboard.vue')
      },
      {
        path: 'assets',
        name: 'assets',
        component: () => import('../pages/Assets.vue')
      },
      {
        path: 'transactions',
        name: 'transactions',
        component: () => import('../pages/Transactions.vue')
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  if (to.meta.requiresAuth && !isAuthenticated()) {
    return { name: 'login' }
  }

  if (to.name === 'login' && isAuthenticated()) {
    return { name: 'dashboard' }
  }
})

export default router
