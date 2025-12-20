/**
 * Application Entry Point
 * Initializes Vue app with Pinia, Router, and Axios interceptors
 */
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import axios from 'axios'

import './assets/main.css'

import App from './App.vue'
import router from './router'
import { useAuthStore } from './stores/auth'

// Create Vue app
const app = createApp(App)

// Initialize Pinia store
app.use(createPinia())

// Initialize auth
const authStore = useAuthStore()
authStore.initializeAuth()

// Setup Axios interceptor for token refresh
axios.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config

    // If 401 and we haven't retried yet, try to refresh token
    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      authStore.refreshToken
    ) {
      originalRequest._retry = true

      try {
        await authStore.refreshAccessToken()
        return axios(originalRequest)
      } catch {
        // refreshAccessToken handles logout on failure
      }
    }

    return Promise.reject(error)
  },
)

// Initialize router and mount app
app.use(router)
app.mount('#app')
