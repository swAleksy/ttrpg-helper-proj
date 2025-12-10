import { defineStore } from 'pinia'
// import { useRouter } from 'vue-router'
import axios from 'axios'
import router from '@/router'

export const API_URL =
  (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:8080'
// const router = useRouter()

interface LoginResponse {
  token: string
  refreshToken: string
}

interface UserProfile {
  id: number
  userName: string
  email: string
  avatarUrl?: string | null
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    // W LocalStorage trzymamy TYLKO tokeny
    token: localStorage.getItem('auth_token') as string | null,
    refreshToken: localStorage.getItem('refresh_token') as string | null,

    // Dane użytkownika trzymamy w pamięci (reaktywne)
    // Nie pobieramy ich z LS, bo mogą być nieaktualne.
    // Pobrane zostaną przez fetchCurrentUser() przy starcie.
    user: null as UserProfile | null,
    loading: false,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,

    username: (state) => state.user?.userName || '',

    userAvatarUrl: (state) => {
      const avatar = state.user?.avatarUrl

      if (avatar) {
        if (avatar.startsWith('http')) return avatar

        // Czyste łączenie URLi
        const baseUrl = API_URL.replace(/\/$/, '')
        const path = avatar.replace(/^\//, '')
        return `${baseUrl}/${path}`
      }

      // Fallback
      const name = state.user?.userName || 'User'
      return generateUiAvatar(name)
    },
  },

  actions: {
    // zeby przy odswiezaniu zachowac token w headerze
    initializeAuth() {
      if (this.token) {
        axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
        this.fetchCurrentUser()
      } else {
        delete axios.defaults.headers.common['Authorization']
      }
    },
    async _handleAuthResponse(response: { token: string; refreshToken: string }) {
      this.token = response.token
      this.refreshToken = response.refreshToken

      localStorage.setItem('auth_token', this.token)
      localStorage.setItem('refresh_token', this.refreshToken)

      axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`

      await this.fetchCurrentUser()
    },
    // REJESTRACJA: /api/user/register
    async register(userName: string, email: string, password: string, isAdminRequest = false) {
      this.loading = true
      try {
        const generatedAvatarUrl = generateUiAvatar(userName)
        // backend przy 200 OK nie zwraca body, więc nie typujemy odpowiedzi
        const res = await axios.post<LoginResponse>(`${API_URL}/api/user/register`, {
          userName,
          email,
          password,
          isAdminRequest,
          generatedAvatarUrl,
        })

        await this._handleAuthResponse(res.data)
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

        await this._handleAuthResponse(res.data)
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
        return this.token
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
        this.user = res.data // Zapisujemy cały obiekt usera
      } catch (error: any) {
        console.error('Error fetching user', error)
        // Jeśli 401, wyloguj
        if (error.response?.status === 401) {
          this.logout()
        }
      }
    },

    async uploadAvatar(file: File) {
      this.loading = true

      const formData = new FormData()
      formData.append('file', file) // Must match C# controller parameter name
      try {
        const res = await axios.post<{ avatarUrl: string }>(
          `${API_URL}/api/upload/uploadavatar`,
          formData,
          {
            headers: { 'Content-Type': 'multipart/form-data' },
          },
        )

        // Update state immediately with the new URL returned by backend
        if (this.user) {
          this.user.avatarUrl = res.data.avatarUrl
        }
        return true
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
      this.user = null

      localStorage.removeItem('auth_token')
      localStorage.removeItem('refresh_token')

      delete axios.defaults.headers.common['Authorization']

      router.push('/')
    },
  },
})

const stringToColor = (str: string) => {
  let hash = 0
  for (let i = 0; i < str.length; i++) {
    hash = str.charCodeAt(i) + ((hash << 5) - hash)
  }
  let color = ''
  for (let i = 0; i < 3; i++) {
    const value = (hash >> (i * 8)) & 0xff
    color += ('00' + value.toString(16)).substr(-2)
  }
  return color
}

// Helper: Generate the full URL
const generateUiAvatar = (name: string) => {
  const bgColor = stringToColor(name)
  const encodedName = encodeURIComponent(name)
  // Settings: size 128, white text, custom background based on name hash
  return `https://ui-avatars.com/api/?name=${encodedName}&background=${bgColor}&color=fff&size=128`
}
