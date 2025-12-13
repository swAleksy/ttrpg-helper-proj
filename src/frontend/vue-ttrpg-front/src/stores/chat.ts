// stores/chat.ts
import { defineStore } from 'pinia'
import type { UserInfoDto } from '@/stores/friends'

interface ChatState {
  activeChats: UserInfoDto[]
}

export const useChatStore = defineStore('chat', {
  state: (): ChatState => ({
    activeChats: [],
  }),

  actions: {
    openChat(user: UserInfoDto) {
      // Sprawdź czy już nie jest otwarte
      if (!this.activeChats.some((chat) => chat.id === user.id)) {
        this.activeChats.push(user)
      }
    },
    // ZMIANA: userId powinien mieć taki typ jak user.id.
    // Bezpieczniej użyć string | number lub po prostu UserInfoDto['id']
    closeChat(userId: string | number) {
      this.activeChats = this.activeChats.filter((chat) => chat.id !== userId)
    },
  },
})
