import { defineStore } from 'pinia'
import { useRouter } from 'vue-router'
import axios from 'axios'

export const API_URL =
  (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:8080'
const router = useRouter()

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
      // Logic fix: Ensure we don't double-slash or mess up the URL
      if (state.avatarUrl) {
        if (state.avatarUrl.startsWith('http')) {
          return state.avatarUrl
        }
        // Remove trailing slash from API_URL and leading slash from avatarUrl to be safe
        const baseUrl = API_URL.replace(/\/$/, '')
        const path = state.avatarUrl.replace(/^\//, '')
        return `${baseUrl}/${path}`
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
      } else {
        delete axios.defaults.headers.common['Authorization']
      }
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
          avatarUrl: generatedAvatarUrl,
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
      if (!this.token) {
        console.log('no token')
        return
      }

      try {
        const res = await axios.get<UserProfile>(`${API_URL}/api/user/me`)

        // FIX: Use camelCase here to match the JSON response
        this.username = res.data.userName
        this.email = res.data.email

        localStorage.setItem('username', this.username)
        localStorage.setItem('email', this.email)

        console.log('--- fetchCurrentUser Success ---')
        console.log('Store Username:', this.username) // Should now print "testuser"
        console.log('Store Email:', this.email)

        // FIX: Use camelCase here
        console.log('Avatar URL:', res.data.avatarUrl)

        if (res.data.avatarUrl) {
          this.avatarUrl = res.data.avatarUrl
          localStorage.setItem('avatar_url', this.avatarUrl)
        }
      } catch (error: any) {
        console.error('Error fetching user', error)
        if (error.response && error.response.status === 401) {
          this.logout()
        }
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
      localStorage.removeItem('avatarUrl')
      // usunięcie nagłówka
      delete axios.defaults.headers.common['Authorization']
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
