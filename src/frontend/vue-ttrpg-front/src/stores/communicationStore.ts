import { defineStore } from 'pinia'
import { ref } from 'vue'
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr'
import axios from 'axios'

// Stores
import { useAuthStore, API_URL } from '@/stores/auth'
import { useFriendsStore } from '@/stores/friendsStore'
import { useNotificationStore, type NotificationDto } from '@/stores/notificationStore'

export const useCommunicationStore = defineStore('communication', () => {
  // State
  const connection = ref<HubConnection | null>(null)
  const isConnected = ref(false)
  const unreadNotificationsCount = ref(0) // For the bell icon (e.g., total unread)
  const hasNewMessages = ref(false) // For the chat icon dot

  // Store instances
  const authStore = useAuthStore()
  const friendsStore = useFriendsStore()
  const notificationStore = useNotificationStore()

  const initSignalR = async () => {
    // Prevent double connection or connection without token
    if (isConnected.value || !authStore.token) return

    // Optional: Fetch initial counts (e.g. unread friend requests)
    await fetchInitialNotificationsState()

    const newConnection = new HubConnectionBuilder()
      .withUrl(`${API_URL}/mainHub`, {
        accessTokenFactory: () => authStore.token || '',
      })
      .withAutomaticReconnect()
      .build()

    // --- HANDLERS ---

    // 1. General Notification Handler (The "Master" Listener)
    // This handles everything: Friend Requests, Acceptances, System Alerts, etc.
    newConnection.on('ReceiveNotification', (dto: NotificationDto) => {
      console.log('New notification received:', dto)

      // A. Push the notification to the visual dropdown list immediately
      notificationStore.pushNotification(dto)

      // B. Handle specific side effects based on the TYPE of notification
      switch (dto.type) {
        case 'FriendRequest':
          // If we got a friend request, update the red counter and refresh the pending list
          unreadNotificationsCount.value++
          friendsStore.fetchPending()
          break

        case 'FriendRequestAccepted':
          // If someone accepted us, refresh the main friends list
          friendsStore.fetchFriends()
          break

        case 'NewMessage':
          // If the notification was about a message, mark chat as having activity
          hasNewMessages.value = true
          break
      }
    })

    // 2. Chat Specific Handler
    // We keep this separate because chat messages often come with a "MessageDto" payload
    // that is different from a "NotificationDto", and we might need to append it to a chat window.
    newConnection.on('ReceivePrivateMessage', (msg) => {
      hasNewMessages.value = true
      // logic to append message to active chat window store could go here
    })

    // --- START CONNECTION ---
    try {
      await newConnection.start()
      console.log('SignalR Global Connected')
      isConnected.value = true
      connection.value = newConnection
    } catch (err) {
      console.error('SignalR Connection Error:', err)
    }
  }

  const fetchInitialNotificationsState = async () => {
    if (!authStore.token) return
    try {
      // Fetch amount of pending friend requests for the red badge
      const res = await axios.get(`${API_URL}/api/friend/pending`)
      unreadNotificationsCount.value = res.data.length
    } catch (err) {
      console.error('Error fetching notification count', err)
    }
  }

  const stopSignalR = () => {
    if (connection.value) {
      connection.value.stop()
      isConnected.value = false
      connection.value = null
    }
  }

  const markNotificationsAsRead = () => {
    unreadNotificationsCount.value = 0
  }

  return {
    connection,
    isConnected,
    unreadNotificationsCount,
    hasNewMessages,
    initSignalR,
    stopSignalR,
    markNotificationsAsRead,
  }
})
