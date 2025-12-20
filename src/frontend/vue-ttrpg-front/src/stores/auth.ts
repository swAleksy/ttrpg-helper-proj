/**
 * Authentication Store
 * Manages user authentication, tokens, and profile data
 */
import { defineStore } from 'pinia'
import axios from 'axios'
import router from '@/router'
import { API_URL } from '@/config/api'
import { resolveAvatarUrl, generateUiAvatar } from '@/utils/avatar'
import type { UserProfile, LoginResponse } from '@/types'

// Re-export for backward compatibility
export { API_URL } from '@/config/api'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('auth_token') as string | null,
    refreshToken: localStorage.getItem('refresh_token') as string | null,
    user: null as UserProfile | null,
    isLoading: false,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    username: (state) => state.user?.userName ?? '',
    userAvatarUrl: (state) => resolveAvatarUrl(state.user?.avatarUrl, state.user?.userName),
  },

  actions: {
    initializeAuth() {
      if (this.token) {
        axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
        this.fetchCurrentUser()
      } else {
        delete axios.defaults.headers.common['Authorization']
      }
    },

    async _handleAuthResponse(response: LoginResponse) {
      this.token = response.token
      this.refreshToken = response.refreshToken

      localStorage.setItem('auth_token', this.token)
      localStorage.setItem('refresh_token', this.refreshToken)

      axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`

      await this.fetchCurrentUser()
    },

    async register(userName: string, email: string, password: string, isAdminRequest = false) {
      this.isLoading = true
      try {
        const avatarUrl = generateUiAvatar(userName)
        const res = await axios.post<LoginResponse>(`${API_URL}/api/user/register`, {
          userName,
          email,
          password,
          isAdminRequest,
          avatarUrl,
        })
        await this._handleAuthResponse(res.data)
      } finally {
        this.isLoading = false
      }
    },

    async login(username: string, password: string) {
      this.isLoading = true
      try {
        const res = await axios.post<LoginResponse>(`${API_URL}/api/user/login`, {
          username,
          password,
        })
        await this._handleAuthResponse(res.data)
      } finally {
        this.isLoading = false
      }
    },

    async refreshAccessToken() {
      if (!this.refreshToken) {
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

        axios.defaults.headers.common['Authorization'] = `Bearer ${this.token}`
        return this.token
      } catch (error) {
        this.logout()
        throw error
      }
    },

    async fetchCurrentUser() {
      if (!this.token) return

      try {
        const res = await axios.get<UserProfile>(`${API_URL}/api/user/me`)
        this.user = res.data
      } catch (error: unknown) {
        console.error('Error fetching user', error)
        if (axios.isAxiosError(error) && error.response?.status === 401) {
          this.logout()
        }
      }
    },

    async uploadAvatar(file: File) {
      this.isLoading = true
      try {
        const formData = new FormData()
        formData.append('file', file)

        const res = await axios.post<{ avatarUrl: string }>(
          `${API_URL}/api/upload/uploadavatar`,
          formData,
          { headers: { 'Content-Type': 'multipart/form-data' } },
        )

        if (this.user) {
          this.user.avatarUrl = res.data.avatarUrl
        }
        return true
      } catch (error) {
        console.error('Upload failed', error)
        throw error
      } finally {
        this.isLoading = false
      }
    },

    async updateProfile(payload: { UserName?: string; Email?: string }) {
      this.isLoading = true
      try {
        await axios.put(`${API_URL}/api/user/profile`, payload)
        await this.fetchCurrentUser()
      } finally {
        this.isLoading = false
      }
    },

    async updatePassword(currentPassword: string, newPassword: string) {
      this.isLoading = true
      try {
        await axios.post(`${API_URL}/api/user/change-password`, {
          CurrentPassword: currentPassword,
          NewPassword: newPassword,
        })
      } finally {
        this.isLoading = false
      }
    },

    async deleteUser() {
      this.isLoading = true
      try {
        await axios.delete(`${API_URL}/api/user/delete-user`)
        this.logout()
        router.push('/')
      } catch (error) {
        console.error('Failed to delete user', error)
        throw error
      } finally {
        this.isLoading = false
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
