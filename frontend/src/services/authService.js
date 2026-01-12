import api from './api'

export function register(username, password) {
  return api.post('/auth/register', { username, password })
}

export async function login(username, password) {
  const res = await api.post('/auth/login', { username, password })
  const token = res?.data?.token
  if (token) localStorage.setItem('token', token)
  return res.data
}

export function logout() {
  localStorage.removeItem('token')
}

export function isAuthenticated() {
  return !!localStorage.getItem('token')
}
