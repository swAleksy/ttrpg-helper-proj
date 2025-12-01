import { defineStore } from 'pinia'
import axios from 'axios'

const API_URL = (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:8080'

interface LoginResponse {
  token: string
  refreshToken: string
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: null as string | null,
    refreshToken: null as string | null,
    username: null as string | null,
    loading: false,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
  },

  actions: {
    // REJESTRACJA: /api/user/register
    async register(
      userName: string,
      email: string,
      password: string,
      isAdminRequest = false,
    ) {
      this.loading = true
      try {
        // backend przy 200 OK nie zwraca body, więc nie typujemy odpowiedzi
        await axios.post(`${API_URL}/api/user/register`, {
          userName,
          email,
          password,
          isAdminRequest,
        })
        // ewentualnie dodać np. toast z informacją "konto utworzone"
      } finally {
        this.loading = false
      }
    },

    // LOGOWANIE: /api/user/login
    async login(username: string, password: string) {
      this.loading = true
      try {
        const res = await axios.post<LoginResponse>(
          `${API_URL}/api/user/login`,
          {
            username,
            password,
          },
        )

        this.token = res.data.token
        this.refreshToken = res.data.refreshToken
        this.username = username // bierzemy z formularza, backend nie zwraca

        localStorage.setItem('auth_token', this.token)
        localStorage.setItem('refresh_token', this.refreshToken)
        localStorage.setItem('username', this.username)
      } finally {
        this.loading = false
      }
    },

    logout() {
      this.token = null
      this.refreshToken = null
      this.username = null

      localStorage.removeItem('auth_token')
      localStorage.removeItem('refresh_token')
      localStorage.removeItem('username')
    },
  },
})
