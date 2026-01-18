import { createRouter, createWebHistory } from 'vue-router'
import { isAuthenticated } from '@/services/authService'

const routes = [
  // LOGIN (be layout)
  {
    path: '/login',
    name: 'login',
    component: () => import('../pages/Login.vue')
  },

  // VISI puslapiai su header + footer
  {
    path: '/',
    component: () => import('../layouts/DefaultLayout.vue'),
    children: [
      // PUBLIC
      {
        path: 'privacy',
        name: 'privacy',
        component: () => import('../pages/Privacy.vue')
      },
      {
        path: 'terms',
        name: 'terms',
        component: () => import('../pages/Terms.vue')
      },
      {
        path: 'contact',
        name: 'contact',
        component: () => import('../pages/Contact.vue')
      },

      // PROTECTED
      {
        path: '',
        name: 'dashboard',
        meta: { requiresAuth: true },
        component: () => import('../pages/Dashboard.vue')
      },
      {
        path: 'assets',
        name: 'assets',
        meta: { requiresAuth: true },
        component: () => import('../pages/Assets.vue')
      },
      {
        path: 'transactions',
        name: 'transactions',
        meta: { requiresAuth: true },
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
