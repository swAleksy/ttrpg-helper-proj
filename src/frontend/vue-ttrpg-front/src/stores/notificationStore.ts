import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'
import { API_URL } from '@/stores/auth' // Załóżmy, że tam masz ten stały URL
import { resolveAvatarUrl } from '@/utils/avatar'

export type NotificationType =
  | 'AddedToGroup'
  | 'NewMessage'
  | 'FriendRequest'
  | 'FriendRequestAccepted'

export interface NotificationDto {
  id: number
  title: string
  message: string
  type: NotificationType
  isRead: boolean
  createdAt: string
  fromUserId?: number
}

export const useNotificationStore = defineStore('notifications', () => {
  const notifications = ref<NotificationDto[]>([])
  const isLoading = ref(false)
  const areNotificationLoading = ref(false)

  const fetchNotifications = async () => {
    if (areNotificationLoading.value) return
    areNotificationLoading.value = true
    try {
      const response = await axios.get(`${API_URL}/api/notification/unread`)
      areNotificationLoading.value = response.data.map((f: any) => ({
        ...f,
      }))
    } catch (e) {
      console.error(e)
    } finally {
      areNotificationLoading.value = false
    }
  }

  const pushNotification = (notification: NotificationDto) => {
    // Add new item to the top of the array
    notifications.value.unshift(notification)
  }

  const markAsRead = async (id: number) => {
    // 1. Optimistic Update (Update UI immediately)
    const notification = notifications.value.find((n) => n.id === id)
    if (notification) {
      notification.isRead = true
    }

    try {
      await axios.post(`${API_URL}/api/notification/mark-read/${id}`)
    } catch (e) {
      console.error(e)
      // Optional: Revert change if API fails
      if (notification) notification.isRead = false
    }
  }

  return {
    notifications,
    isLoading,
    fetchNotifications,
    markAsRead,
    pushNotification,
  }
})
