/**
 * Notification Store
 * Manages all notification-related state and actions
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'
import { API_URL } from '@/config/api'
import type { NotificationDto } from '@/types'

// Re-export type for backward compatibility
export type { NotificationDto } from '@/types'

export const useNotificationStore = defineStore('notifications', () => {
  // State
  const notifications = ref<NotificationDto[]>([])
  const isLoading = ref(false)
  const unreadCount = ref(0)

  // Getters
  const hasUnread = computed(() => unreadCount.value > 0)
  const unreadNotifications = computed(() => notifications.value.filter((n) => !n.isRead))
  const hasUnreadFrom = computed(() => (userId: number) => {
    return notifications.value.some(
      (n) => n.type === 'NewMessage' && n.fromUserId === userId && !n.isRead,
    )
  })

  // Actions
  const fetchNotifications = async () => {
    if (isLoading.value) return
    isLoading.value = true
    try {
      const response = await axios.get<NotificationDto[]>(`${API_URL}/api/notification/unread`)
      notifications.value = response.data
      unreadCount.value = response.data.filter((n) => !n.isRead).length
    } catch (error) {
      console.error('Error fetching notifications:', error)
    } finally {
      isLoading.value = false
    }
  }

  const pushNotification = (notification: NotificationDto) => {
    // Add new item to the top of the array
    notifications.value.unshift(notification)
    if (!notification.isRead) {
      unreadCount.value++
    }
  }

  const markAsRead = async (id: number) => {
    // Optimistic update
    const notification = notifications.value.find((n) => n.id === id)
    if (notification && !notification.isRead) {
      notification.isRead = true
      unreadCount.value = Math.max(0, unreadCount.value - 1)
    }

    try {
      await axios.post(`${API_URL}/api/notification/mark-read/${id}`)
    } catch (error) {
      console.error('Error marking notification as read:', error)
      // Revert on failure
      if (notification) {
        notification.isRead = false
        unreadCount.value++
      }
    }
  }

  const markAllAsRead = () => {
    notifications.value.forEach((n) => (n.isRead = true))
    unreadCount.value = 0
  }

  const incrementUnreadCount = () => {
    unreadCount.value++
  }

  const clearAll = () => {
    notifications.value = []
    unreadCount.value = 0
  }

  return {
    // State
    notifications,
    isLoading,
    unreadCount,
    // Getters
    hasUnread,
    unreadNotifications,
    hasUnreadFrom,
    // Actions
    fetchNotifications,
    pushNotification,
    markAsRead,
    markAllAsRead,
    incrementUnreadCount,
    clearAll,
  }
})
