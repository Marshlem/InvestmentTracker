import axios from 'axios'
import router from '@/router'
import { logout } from '@/services/authService'

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL
})

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error?.response?.status === 401) {
      logout()
      router.replace({ name: 'login' })
    }
    return Promise.reject(error)
  }
)

export default api
