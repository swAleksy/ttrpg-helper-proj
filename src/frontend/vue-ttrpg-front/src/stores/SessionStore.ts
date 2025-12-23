import { defineStore } from 'pinia'
import axios from 'axios'
import { API_URL } from '@/config/api'
import type { SessionDto } from '@/types'

export const useSessionStore = defineStore('session', {
  state: () => ({
    session: null as SessionDto | null,
    isLoading: false,
  }),

  actions: {
    async fetchSessionById(id: number) {
      this.isLoading = true
      try {
        const response = await axios.get(`${API_URL}/api/session/both/${id}`)
        this.session = response.data
      } catch (error) {
        console.error('Failed to fetch session', error)
      } finally {
        this.isLoading = false
      }
    },

    clearSession() {
      this.session = null
    },
  },
})
