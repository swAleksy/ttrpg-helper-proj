import { defineStore } from 'pinia'
import { useRouter } from 'vue-router'
import axios from 'axios'

const API_URL = (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:8080'
const router = useRouter()

interface LoginResponse {
  token: string
  refreshToken: string
}

interface UserProfile {
  username: string
  email: string
  // backend może zwracać null, jeśli user nie ustawił awatara
  avatarUrl?: string | null
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    // Inicjalizacja od razu z localStorage, żeby nie tracić sesji po odświeżeniu
    token: localStorage.getItem('auth_token') as string | null,
    refreshToken: localStorage.getItem('refresh_token') as string | null,
    username: localStorage.getItem('username') as string | null,

    email: localStorage.getItem('email') as string | null,
    avatarUrl: localStorage.getItem('avatar_url') as string | null,
    loading: false,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,

    userAvatarUrl: (state) => {
      // 1. Jeśli mamy awatar pobrany z bazy (np. Blob URL lub Base64), zwróć go
      if (state.avatarUrl) {
        if (state.avatarUrl.startsWith('http')) return state.avatarUrl
        return `${API_URL}${state.avatarUrl}`
      }
      const name = state.username || 'User'
      return `https://ui-avatars.com/api/?name=${name}&background=10b981&color=fff`
    },
  },

  actions: {
    // zeby przy odswiezaniu zachowac token w headerze
    initializeAuth() {
      if (this.token) {
        axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
      }
    },

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

        localStorage.setItem('auth_token', this.token)
        localStorage.setItem('refresh_token', this.refreshToken)

        // Ustawiamy header autoryzacji dla przyszłych zapytań
        axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
        console.log(this.token)
        await this.fetchCurrentUser()
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
        const res = await axios.get<UserProfile>(`${API_URL}/api/user/me`)

        this.username = res.data.username
        this.email = res.data.email
        localStorage.setItem('username', this.username)
        localStorage.setItem('email', this.email)

        console.log(res.data.avatarUrl)

        if (res.data.avatarUrl) {
          this.avatarUrl = res.data.avatarUrl
          localStorage.setItem('avatar_url', this.avatarUrl)
        }
        // else {
        //   this.avatarUrl = null
        //   localStorage.removeItem('avatar_url')
        // }
      } catch (error) {
        console.error('Error fetching user', error)
      }
    },

    async uploadAvatar(file: File) {
      this.loading = true

      const formData = new FormData()
      formData.append('file', file) // Must match C# controller parameter name
      console.log(`${API_URL}/api/upload/uploadavatar`)
      try {
        // Post to the specific upload endpoint
        const res = await axios.post<{ avatarUrl: string }>(
          `${API_URL}/api/upload/uploadavatar`,
          formData,
          {
            headers: { 'Content-Type': 'multipart/form-data' },
          },
        )

        // Update state immediately with the new URL returned by backend
        this.avatarUrl = res.data.avatarUrl
        localStorage.setItem('avatar_url', this.avatarUrl)

        return true // Success
      } catch (error) {
        console.error('Upload failed', error)
        throw error
      } finally {
        this.loading = false
      }
    },

    async updateProfile(payload: { UserName?: string; Email?: string }) {
      this.loading = true
      try {
        await axios.put(`${API_URL}/api/user/profile`, payload)
      } finally {
        this.loading = false
        this.fetchCurrentUser()
      }
    },

    async updatePassword(CurrentPassword: string, NewPassword: string) {
      this.loading = true
      try {
        await axios.post(`${API_URL}/api/user/change-password`, {
          CurrentPassword,
          NewPassword,
        })
      } finally {
        this.loading = false
      }
    },

    async deleteUser() {
      try {
        await axios.delete(`${API_URL}/api/user/delete-user`)
      } finally {
        this.loading = false
        this.logout()
        router.push('/')
      }
    },

    logout() {
      this.token = null
      this.refreshToken = null
      this.username = null
      this.email = null
      this.avatarUrl = null

      localStorage.removeItem('auth_token')
      localStorage.removeItem('refresh_token')
      localStorage.removeItem('username')
      localStorage.removeItem('email')
      // usunięcie nagłówka
      delete axios.defaults.headers.common['Authorization']
    },
  },
})
