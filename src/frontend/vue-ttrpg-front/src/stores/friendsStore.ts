/**
 * Friends Store
 * Manages friend list, pending requests, and friend-related actions
 */
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import axios from 'axios'
import { API_URL } from '@/config/api'
import { resolveAvatarUrl } from '@/utils/avatar'
import type { UserInfoDto } from '@/types'

export type { UserInfoDto } from '@/types'

export const useFriendsStore = defineStore('friends', () => {
  const friends = ref<UserInfoDto[]>([])
  const pendingRequests = ref<UserInfoDto[]>([])
  const isLoading = ref(false)
  const isFriendsLoading = ref(false)
  const isPendingLoading = ref(false)

  const friendCount = computed(() => friends.value.length)
  const pendingCount = computed(() => pendingRequests.value.length)
  const hasPendingRequests = computed(() => pendingRequests.value.length > 0)

  const sendFriendRequest = async (friendId: string) => {
    if (!friendId) return
    isLoading.value = true
    try {
      await axios.post(`${API_URL}/api/friend/request/${friendId}`)
    } catch (error) {
      console.error('Error sending friend request:', error)
      throw error
    } finally {
      isLoading.value = false
    }
  }

  const fetchFriends = async () => {
    if (isFriendsLoading.value) return
    isFriendsLoading.value = true
    try {
      const response = await axios.get<UserInfoDto[]>(`${API_URL}/api/friend/all`)
      friends.value = response.data.map((friend) => ({
        ...friend,
        avatarUrl: resolveAvatarUrl(friend.avatarUrl, friend.userName),
      }))
    } catch (error) {
      console.error('Error fetching friends:', error)
    } finally {
      isFriendsLoading.value = false
    }
  }

  const fetchPending = async () => {
    if (isPendingLoading.value) return
    isPendingLoading.value = true
    try {
      const response = await axios.get<UserInfoDto[]>(`${API_URL}/api/friend/pending`)
      pendingRequests.value = response.data.map((friend) => ({
        ...friend,
        avatarUrl: resolveAvatarUrl(friend.avatarUrl, friend.userName),
      }))
    } catch (error) {
      console.error('Error fetching pending requests:', error)
    } finally {
      isPendingLoading.value = false
    }
  }

  const acceptRequest = async (friendId: number) => {
    try {
      await axios.put(`${API_URL}/api/friend/accept/${friendId}`)
      pendingRequests.value = pendingRequests.value.filter((f) => f.id !== friendId)
      await fetchFriends()
    } catch (error) {
      console.error('Error accepting friend request:', error)
      throw error
    }
  }

  const rejectRequest = async (friendId: number) => {
    try {
      await axios.delete(`${API_URL}/api/friend/${friendId}`)
      pendingRequests.value = pendingRequests.value.filter((f) => f.id !== friendId)
    } catch (error) {
      console.error('Error rejecting friend request:', error)
      throw error
    }
  }

  const removeFriend = async (friendId: number) => {
    try {
      await axios.delete(`${API_URL}/api/friend/${friendId}`)
      friends.value = friends.value.filter((f) => f.id !== friendId)
    } catch (error) {
      console.error('Error removing friend:', error)
      throw error
    }
  }

  const resolvePending = async (friendId: number, accept: boolean) => {
    if (accept) {
      await acceptRequest(friendId)
    } else {
      await rejectRequest(friendId)
    }
  }

  return {
    // State
    friends,
    pendingRequests,
    // Legacy alias
    friendsPending: pendingRequests,
    isLoading,
    isFriendsLoading,
    isPendingLoading,
    // Getters
    friendCount,
    pendingCount,
    hasPendingRequests,
    // Actions
    sendFriendRequest,
    fetchFriends,
    fetchPending,
    acceptRequest,
    rejectRequest,
    removeFriend,
    resolvePending,
  }
})
