import { createApp } from 'vue'
import { createPinia } from 'pinia'
import axios from 'axios'

import './assets/main.css'

import App from './App.vue'
import router from './router'
import { useAuthStore } from './stores/auth'

const app = createApp(App)

app.use(createPinia())

const authStore = useAuthStore()

authStore.initializeAuth()
authStore.fetchCurrentUser()

// po odświeżeniu strony requesty axiosa będą widzieć token
if (authStore.token) {
  axios.defaults.headers.common['Authorization'] = `Bearer ${authStore.token}`
}

// interceptor odświeżania tokena
axios.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config as any

    if (error.response?.status === 401 && !originalRequest._retry && authStore.refreshToken) {
      originalRequest._retry = true

      try {
        await authStore.refreshAccessToken()
        return axios(originalRequest)
      } catch (err) {
        // refreshAccessToken zrobi logout, jeśli się nie uda
      }
    }
    return Promise.reject(error)
  },
)

app.use(router)

app.mount('#app')
