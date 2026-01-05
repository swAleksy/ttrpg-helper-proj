import { defineStore } from 'pinia'
import axios from 'axios'
import { HubConnectionBuilder, HubConnection, HubConnectionState } from '@microsoft/signalr'
import { API_URL, SIGNALR_HUB_SESSION_URL } from '@/config/api'
import { useAuthStore } from '@/stores/auth'
import type {
  SessionDto,
  UserInfoDto,
  GetSessionEventDto,
  SessionEventModel,
  ChatMessagePayload,
  DiceRollPayload,
  UserLifecyclePayload,
  CreateSessionEventDto,
  NoteDto,
  ItemDto,
  NpcDto,
} from '@/types'
import { resolveAvatarUrl } from '@/utils/avatar'

export const useSessionStore = defineStore('session', {
  state: () => ({
    session: null as SessionDto | null,
    players: [] as UserInfoDto[],
    isLoading: false,
    events: [] as SessionEventModel[],
    hubConnection: null as HubConnection | null,
    notes: [] as NoteDto[],
    items: [] as ItemDto[],
    npcs: [] as NpcDto[],
  }),

  getters: {
    playersWithAvatar(): Array<UserInfoDto & { resolvedAvatar: string }> {
      return this.players.map((player) => ({
        ...player,
        resolvedAvatar: resolveAvatarUrl(player.avatarUrl, player.userName),
      }))
    },
    isConnected(): boolean {
      return this.hubConnection?.state === HubConnectionState.Connected
    },
  },

  actions: {
    async fetchSessionById(id: number) {
      this.isLoading = true
      try {
        const response = await axios.get<SessionDto>(`${API_URL}/api/session/both/${id}`)
        this.session = response.data

        if (this.session && this.session.players.length > 0) {
          await this.fetchSessionPlayersDetails()
        } else {
          this.players = []
        }
      } catch (error) {
        console.error('Failed to fetch session', error)
      } finally {
        this.isLoading = false
      }
    },

    async fetchSessionPlayersDetails() {
      if (!this.session) return

      try {
        //   array of Axios promises
        const requests = this.session.players.map((player) =>
          axios.get<UserInfoDto>(`${API_URL}/api/user/info/${player.playerId}`),
        )
        const responses = await Promise.all(requests)
        this.players = responses.map((res) => res.data)
      } catch (error) {
        console.error('Failed to fetch player details', error)
      }
    },

    async fetchCampaignData(campaignId: number) {
      console.log(`id wysłane do sesji${campaignId}`)
      if (!this.session) {
        console.log(`this.session: ${this.session}`)
        console.warn('Brak campaignId w sesji 1 - nie można pobrać danych kampanii.')
        return
      }
      if (!campaignId) {
        console.log(`campaignId ======= : ${campaignId}`)
        console.warn('Brak campaignId w sesji 2 - nie można pobrać danych kampanii.')
        return
      }

      await Promise.all([
        this.fetchNotes(campaignId),
        this.fetchItems(campaignId),
        this.fetchNpcs(campaignId),
      ])
    },

    async fetchNotes(campaignId: number) {
      try {
        const response = await axios.get<NoteDto[]>(`${API_URL}/api/note/campaign/${campaignId}`)
        this.notes = response.data
      } catch (error) {
        console.error('Failed to fetch notes', error)
      }
    },

    async fetchItems(campaignId: number) {
      try {
        const response = await axios.get<ItemDto[]>(`${API_URL}/api/item/campaign/${campaignId}`)
        this.items = response.data
      } catch (error) {
        console.error('Failed to fetch items', error)
      }
    },

    async fetchNpcs(campaignId: number) {
      try {
        const response = await axios.get<NpcDto[]>(`${API_URL}/api/npc/campaign/${campaignId}`)
        this.npcs = response.data
      } catch (error) {
        console.error('Failed to fetch npcs', error)
      }
    },

    async addPlayer(sessionId: number, playerId: number) {
      if (!this.session) return
      try {
        await axios.post<UserInfoDto>(
          `${API_URL}/api/session/gm/addplayer/${sessionId}/${playerId}`,
        )
      } catch (error) {
        console.error('Failed to add player to session', error)
      }
    },

    async fetchEvents(sessionId: number) {
      try {
        const response = await axios.get<GetSessionEventDto[]>(
          `${API_URL}/api/sessionevent/${sessionId}`,
        )
        this.events = response.data.map((dto) => this.mapDtoToModel(dto))
      } catch (error) {
        console.error('Failed to fetch events', error)
      }
    },

    /**
     * Zakładamy, że Backend po otrzymaniu POSTa sam wyemituje zdarzenie przez SignalR
     * do innych, więc nie musimy dodawać go ręcznie do tablicy, jeśli SignalR działa.
     * Ale dla płynności UI ("optimistic UI") często dodaje się go od razu.
     */
    async sendEvent(type: string, payload: unknown) {
      if (!this.session) return

      const dto: CreateSessionEventDto = {
        sessionId: this.session.id,
        type: type,
        dataJson: JSON.stringify(payload),
      }

      try {
        const response = await axios.post<GetSessionEventDto>(`${API_URL}/api/sessionevent`, dto)

        const newEvent = this.mapDtoToModel(response.data)
        this.addEventIfMissing(newEvent)
      } catch (error) {
        console.error('Failed to send event', error)
      }
    },

    async initSignalR(sessionId: number) {
      const authStore = useAuthStore()
      if (!authStore.token) return

      if (this.isConnected && this.session?.id === sessionId) return

      this.hubConnection = new HubConnectionBuilder()
        .withUrl(SIGNALR_HUB_SESSION_URL, {
          accessTokenFactory: () => authStore.token || '',
        })
        .withAutomaticReconnect()
        .build()

      this.hubConnection.on('SessionEventCreated', (dto: GetSessionEventDto) => {
        console.log('SignalR received event:', dto)
        if (dto.type === 'UserJoined' && dto.userId === authStore.user?.id) {
          return
        }

        const eventModel = this.mapDtoToModel(dto)
        this.addEventIfMissing(eventModel)
      })

      try {
        await this.hubConnection.start()
        console.log('SignalR Connected.')

        await this.hubConnection.invoke('JoinSession', sessionId)
        console.log(`Joined SignalR group for session ${sessionId}`)
      } catch (error) {
        console.error('SignalR connection error:', error)
      }
    },

    async stopSignalR() {
      if (this.hubConnection) {
        try {
          if (this.session) {
            await this.hubConnection.invoke('LeaveSession', this.session.id)
          }
        } catch (e) {
          console.error('SignalR leave error:', e)
        }

        await this.hubConnection.stop()
        this.hubConnection = null
        console.log('SignalR Disconnected.')
      }
    },
    // Helper zapobiegający duplikatom
    addEventIfMissing(newEvent: SessionEventModel) {
      const exists = this.events.some((existingEvent) => areEventsEqual(existingEvent, newEvent))

      if (!exists) {
        this.events.push(newEvent)

        this.events.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime())
      }
    },

    mapDtoToModel(dto: GetSessionEventDto): SessionEventModel {
      let parsedData: unknown
      try {
        parsedData = dto.dataJson ? JSON.parse(dto.dataJson) : {}
      } catch (e) {
        console.error('Failed to parse event data', e)
        parsedData = {}
      }

      const dateObj = new Date(dto.timestamp)
      switch (dto.type) {
        case 'ChatMessage':
          return {
            id: dto.id,
            sessionId: dto.sessionId,
            userId: dto.userId,
            userName: dto.userName,
            timestamp: dateObj,
            type: 'ChatMessage',
            data: parsedData as ChatMessagePayload,
          }

        case 'DiceRoll':
          return {
            id: dto.id,
            sessionId: dto.sessionId,
            userId: dto.userId,
            userName: dto.userName,
            timestamp: dateObj,
            type: 'DiceRoll',
            data: parsedData as DiceRollPayload,
          }

        case 'UserJoined':
          return {
            id: dto.id,
            sessionId: dto.sessionId,
            userId: dto.userId,
            userName: dto.userName,
            timestamp: dateObj,
            type: 'UserJoined',
            data: parsedData as UserLifecyclePayload,
          }

        case 'UserLeft':
          return {
            id: dto.id,
            sessionId: dto.sessionId,
            userId: dto.userId,
            userName: dto.userName,
            timestamp: dateObj,
            type: 'UserLeft',
            data: parsedData as UserLifecyclePayload,
          }

        default:
          return {
            id: dto.id,
            sessionId: dto.sessionId,
            userId: dto.userId,
            userName: dto.userName,
            timestamp: dateObj,
            type: 'Unknown',
            data: parsedData,
          }
      }
    },

    clearSession() {
      this.stopSignalR()
      this.session = null
      this.players = []
      this.events = []
    },
  },
})

const safeParse = <T>(json: string): T | null => {
  try {
    return JSON.parse(json)
  } catch (e) {
    console.error('Failed to parse event data', e)
    return null
  }
}

const areEventsEqual = (a: SessionEventModel, b: SessionEventModel): boolean => {
  // 1. PRIMARY CHECK: Database IDs
  // If both have a non-zero ID, strictly compare them.
  if (a.id !== 0 && b.id !== 0) {
    return a.id === b.id
  }

  // 2. SECONDARY CHECK: For ID 0 (System Events like UserJoined/UserLeft)
  // If one has ID and the other is 0, they are definitely different events.
  if ((a.id === 0 && b.id !== 0) || (a.id !== 0 && b.id === 0)) {
    return false
  }

  // 3. COMPOSITE KEY CHECK (When both IDs are 0)
  // Check if they are the same Type and same User
  if (a.type !== b.type || a.userId !== b.userId) {
    return false
  }

  // 4. TIME BUFFER CHECK
  // Compare timestamps with a 2-second tolerance.
  // This allows for slight delays between the REST response time
  // and the SignalR broadcast time.
  const timeDiff = Math.abs(a.timestamp.getTime() - b.timestamp.getTime())

  // Consider them duplicate if they happened within 2000ms (2 seconds) of each other
  return timeDiff < 2000
}
