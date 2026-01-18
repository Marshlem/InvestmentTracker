import api from './api'
import router from '@/router'

const TOKEN_KEY = 'token'

export function register(username, password) {
  return api.post('/auth/register', { username, password })
}

export async function login(username, password) {
  const res = await api.post('/auth/login', { username, password })
  const token = res?.data?.token
  if (token) localStorage.setItem(TOKEN_KEY, token)
  return res.data
}

export function logout() {
  localStorage.removeItem(TOKEN_KEY)
}

export function logoutAndRedirect() {
  logout()
  router.push({ name: 'login' })
}

export function isAuthenticated() {
  return !!localStorage.getItem(TOKEN_KEY)
}
