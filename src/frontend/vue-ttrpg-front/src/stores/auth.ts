import { defineStore } from 'pinia'
import axios from 'axios'

const API_URL = (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:8080'

interface LoginResponse {
  token: string
  refreshToken: string
}

interface UserProfile {
  username: string
  email: string
  // backend może zwracać null, jeśli user nie ustawił awatara
  avatarPath?: string | null
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    // Inicjalizacja od razu z localStorage, żeby nie tracić sesji po odświeżeniu
    token: localStorage.getItem('auth_token') as string | null,
    refreshToken: localStorage.getItem('refresh_token') as string | null,
    username: localStorage.getItem('username') as string | null,

    email: null as string | null,
    avatar: null as string | null,
    loading: false,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,

    userAvatarUrl: (state) => {
      // 1. Jeśli mamy awatar pobrany z bazy (np. Blob URL lub Base64), zwróć go
      if (state.avatar) {
        return state.avatar
      }
      // 2. Jeśli nie ma awatara, użyj UI Avatars (fallback)
      const name = state.username || 'User'
      return `https://ui-avatars.com/api/?name=${name}&background=10b981&color=fff`
    },
  },

  actions: {
    // REJESTRACJA: /api/user/register
    async register(userName: string, email: string, password: string, isAdminRequest = false) {
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
        const res = await axios.post<LoginResponse>(`${API_URL}/api/user/login`, {
          username,
          password,
        })

        this.token = res.data.token
        this.refreshToken = res.data.refreshToken
        this.username = username // bierzemy z formularza, backend nie zwraca

        localStorage.setItem('auth_token', this.token)
        localStorage.setItem('refresh_token', this.refreshToken)
        localStorage.setItem('username', this.username)

        // Ustawiamy header autoryzacji dla przyszłych zapytań (ważne, jeśli nie używasz interceptorów)
        axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
      } finally {
        this.loading = false
      }
    },

    // ODNOWIENIE ACCESS TOKENA PRZEZ REFRESH TOKEN
    async refreshAccessToken() {
      if (!this.refreshToken) {
        // nie ma czym odświeżyć -> wyloguj usera
        this.logout()
        return
      }

      try {
        const res = await axios.post<LoginResponse>(`${API_URL}/api/user/refresh-token`, {
          refreshToken: this.refreshToken,
        })

        this.token = res.data.token
        this.refreshToken = res.data.refreshToken

        localStorage.setItem('auth_token', this.token)
        localStorage.setItem('refresh_token', this.refreshToken)

        // refresh token ustawiamy ponownie w axiosie
        axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
      } catch (error) {
        // jeśli coś nie tak -> logout
        this.logout()
        throw error
      }
    },

    // GET /api/user/me
    async fetchCurrentUser() {
      if (!this.token) return

      try {
        const res = await axios.get<UserProfile>(`${API_URL}/api/user/me`, {
          headers: { Authorization: `Bearer ${this.token}` },
        })

        this.username = res.data.username
        localStorage.setItem('username', this.username)

        this.email = res.data.email
        localStorage.setItem('email', this.email)

        // Jeśli backend mówi, że user ma awatar, spróbuj go pobrać
        if (res.data.avatarPath) {
          await this.fetchUserAvatarBlob(res.data.avatarPath)
        }
      } catch (error) {
        console.error('Nie udało się pobrać danych użytkownika', error)
        // Opcjonalnie: jeśli 401, wyloguj usera
      }
    },
    // PLACEHOLDER: POBIERANIE OBRAZKA Z BAZY
    async fetchUserAvatarBlob(pathOrId: string) {
      console.log('TODO: Pobieranie obrazka z bazy dla:', pathOrId)

      // Na razie zostawiamy this.avatar jako null, więc zadziała getter z ui-avatars
      this.avatar = null
    },
    logout() {
      this.token = null
      this.refreshToken = null
      this.username = null
      this.email = null
      this.avatar = null

      localStorage.removeItem('auth_token')
      localStorage.removeItem('refresh_token')
      localStorage.removeItem('username')
      // usunięcie nagłówka
      delete axios.defaults.headers.common['Authorization']
    },
  },
})
