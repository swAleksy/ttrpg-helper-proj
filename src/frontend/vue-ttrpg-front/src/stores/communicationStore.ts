/**
 * Communication Store
 * Manages SignalR real-time connection and message routing
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { HubConnectionBuilder, HubConnection, HubConnectionState } from '@microsoft/signalr'

import { API_URL, SIGNALR_HUB_URL } from '@/config/api'
import { useAuthStore } from '@/stores/auth'
import { useFriendsStore } from '@/stores/friendsStore'
import { useNotificationStore } from '@/stores/notificationStore'
import type { NotificationDto } from '@/types'

export const useCommunicationStore = defineStore('communication', () => {
  const connection = ref<HubConnection | null>(null)
  const hasNewMessages = ref(false)

  const isConnected = computed(() => connection.value?.state === HubConnectionState.Connected)

  const initSignalR = async () => {
    const authStore = useAuthStore()

    // Prevent double connection or connection without token
    if (isConnected.value || !authStore.token) return

    const friendsStore = useFriendsStore()
    const notificationStore = useNotificationStore()

    await Promise.all([friendsStore.fetchPending(), notificationStore.fetchNotifications()])

    const newConnection = new HubConnectionBuilder()
      .withUrl(SIGNALR_HUB_URL, {
        accessTokenFactory: () => authStore.token || '',
      })
      .withAutomaticReconnect()
      .build()

    // === EVENT HANDLERS ===

    // General notification handler
    newConnection.on('ReceiveNotification', (dto: NotificationDto) => {
      console.log('Notification received:', dto.type)

      // Push to notification store
      notificationStore.pushNotification(dto)

      // Handle side effects based on notification type
      switch (dto.type) {
        case 'FriendRequest':
          friendsStore.fetchPending()
          break
        case 'FriendRequestAccepted':
          friendsStore.fetchFriends()
          break
        case 'NewMessage':
          hasNewMessages.value = true
          console.log('TESTSTETADF')
          break
      }
    })

    // Private message handler (for chat)
    newConnection.on('ReceivePrivateMessage', () => {
      hasNewMessages.value = true
    })

    // Start connection
    try {
      await newConnection.start()
      console.log('SignalR connected')
      connection.value = newConnection
    } catch (error) {
      console.error('SignalR connection error:', error)
    }
  }

  const stopSignalR = async () => {
    if (connection.value) {
      await connection.value.stop()
      connection.value = null
    }
  }

  const clearMessageIndicator = () => {
    hasNewMessages.value = false
  }

  return {
    // State
    connection,
    hasNewMessages,
    // Getters
    isConnected,
    // Actions
    initSignalR,
    stopSignalR,
    clearMessageIndicator,
  }
})
