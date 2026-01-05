import { defineStore } from 'pinia'
import axios from 'axios'
import { API_URL } from '@/config/api'
import { useAuthStore } from './auth'

import type { SessionDto, GameCampaignDto } from '@/types/index'

export const useCampaignStore = defineStore('campaign', {
  state: () => ({
    gmCampaigns: [] as GameCampaignDto[],
    playerCampaigns: [] as GameCampaignDto[],
    session: null as SessionDto | null,
    isLoading: false,
  }),

  getters: {
    getUniquePlayerCampaigns: (state) => {
      const gmCampaignIds = state.gmCampaigns.map((c) => c.id)
      return state.playerCampaigns.filter((c) => !gmCampaignIds.includes(c.id))
    },

    getCampaignById: (state) => {
      return (id: number) => {
        const foundInGm = state.gmCampaigns.find((c) => c.id === id)
        if (foundInGm) return foundInGm

        const foundInPlayer = state.playerCampaigns.find((c) => c.id === id)
        return foundInPlayer || undefined
      }
    },
  },

  actions: {
    async fetchGmCampaigns() {
      const authStore = useAuthStore()
      const playerId = authStore.user?.id
      console.log(`Fetching campaigns for player ID: ${playerId}`)
      if (!playerId) {
        console.warn('Cannot fetch campaigns: User is not logged in or ID is missing.')
        return
      }

      this.isLoading = true
      try {
        const response = await axios.get(`${API_URL}/api/campaign/gm`)
        this.gmCampaigns = response.data
      } catch (error) {
        console.error('Failed to fetch campaigns:', error)
      } finally {
        this.isLoading = false
      }
    },

    async fetchPlayerCampaigns() {
      const authStore = useAuthStore()
      const playerId = authStore.user?.id
      console.log(`Fetching campaigns for player ID: ${playerId}`)
      if (!playerId) {
        console.warn('Cannot fetch campaigns: User is not logged in or ID is missing.')
        return
      }

      this.isLoading = true
      try {
        const response = await axios.get(`${API_URL}/api/campaign/player`)
        this.playerCampaigns = response.data
      } catch (error) {
        console.error('Failed to fetch campaigns:', error)
      } finally {
        this.isLoading = false
      }
    },

    async fetchCampaignGmById(id: number) {
      const authStore = useAuthStore()
      const playerId = authStore.user?.id
      console.log(`Fetching campaigns for player ID: ${playerId}`)
      if (!playerId) {
        console.warn('Cannot fetch campaigns: User is not logged in or ID is missing.')
        return
      }

      this.isLoading = true
      try {
        const response = await axios.get(`${API_URL}/api/campaign/gm/${id}`)

        const index = this.gmCampaigns.findIndex((c) => c.id === id)
        if (index !== -1) {
          this.gmCampaigns[index] = response.data
        } else {
          this.gmCampaigns.push(response.data)
        }
      } finally {
        this.isLoading = false
      }
    },

    async fetchCampaignPlayerById(id: number) {
      const authStore = useAuthStore()
      const playerId = authStore.user?.id
      console.log(`Fetching campaigns for player ID: ${playerId}`)
      if (!playerId) {
        console.warn('Cannot fetch campaigns: User is not logged in or ID is missing.')
        return
      }

      this.isLoading = true
      try {
        const response = await axios.get(`${API_URL}/api/campaign/player/${id}`)

        const index = this.playerCampaigns.findIndex((c) => c.id === id)
        if (index !== -1) {
          this.playerCampaigns[index] = response.data
        } else {
          this.playerCampaigns.push(response.data)
        }
      } finally {
        this.isLoading = false
      }
    },

    async createCampaign(name: string, desc: string) {
      const authStore = useAuthStore()
      const gameMasterId = authStore.user?.id

      try {
        const response = await axios.post(`${API_URL}/api/campaign/gm/create`, {
          gameMasterId: gameMasterId,
          name: name,
          description: desc,
        })

        this.gmCampaigns.push(response.data)
        return true
      } catch (error) {
        console.error('Failed to create campaign', error)
        return false
      }
    },

    async deleteCampaign(id: number) {
      try {
        await axios.delete(`${API_URL}/api/campaign/gm/delete/${id}`)
        this.gmCampaigns = this.gmCampaigns.filter((c) => c.id !== id)

        return true
      } catch (error) {
        console.error('Failed to delete campaign', error)
        return false
      }
    },

    async createSession(name: string, desc: string, scheduledDate: string, campaignId: number) {
      try {
        const response = await axios.post(`${API_URL}/api/session/gm/create`, {
          name: name,
          description: desc,
          scheduledDate: scheduledDate,
          campaignId: campaignId,
        })

        // aktualizacja listy sessji
        const campaign = this.gmCampaigns.find((c) => c.id === campaignId)
        if (campaign && campaign.sessions) {
          campaign.sessions.push(response.data)
        }

        this.session = response.data
        return true
      } catch (error) {
        console.error('Failed to create session', error)
        return false
      }
    },

    async deleteSession(id: number, campaignId: number) {
      try {
        await axios.delete(`${API_URL}/api/session/gm/delete/${id}`)

        const campaign = this.gmCampaigns.find((c) => c.id === campaignId)
        if (campaign && campaign.sessions) {
          campaign.sessions = campaign.sessions.filter((s) => s.id !== id)
        }

        return true
      } catch (error) {
        console.error('Failed to delete session', error)
        return false
      }
    },
  },
})
