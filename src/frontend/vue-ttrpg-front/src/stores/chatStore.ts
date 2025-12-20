/**
 * Chat Store
 * Manages active chat windows state
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { UserInfoDto } from '@/types'

export const useChatStore = defineStore('chat', () => {
  // State
  const activeChats = ref<UserInfoDto[]>([])

  // Getters
  const activeChatCount = computed(() => activeChats.value.length)
  const hasActiveChats = computed(() => activeChats.value.length > 0)

  // Actions
  const openChat = (user: UserInfoDto) => {
    const exists = activeChats.value.some((chat) => chat.id === user.id)
    if (!exists) {
      activeChats.value.push(user)
    }
  }

  const closeChat = (userId: number) => {
    activeChats.value = activeChats.value.filter((chat) => chat.id !== userId)
  }

  const closeAllChats = () => {
    activeChats.value = []
  }

  const isChatOpen = (userId: number) => {
    return activeChats.value.some((chat) => chat.id === userId)
  }

  return {
    // State
    activeChats,
    // Getters
    activeChatCount,
    hasActiveChats,
    // Actions
    openChat,
    closeChat,
    closeAllChats,
    isChatOpen,
  }
})
