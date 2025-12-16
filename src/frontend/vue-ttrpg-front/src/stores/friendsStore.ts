// src/stores/friends.ts
import { defineStore } from 'pinia'
import { ref } from 'vue'
import axios from 'axios'
import { API_URL } from '@/stores/auth' // Załóżmy, że tam masz ten stały URL
import { resolveAvatarUrl } from '@/utils/avatar'

export interface UserInfoDto {
  id: number
  userName: string
  email: string
  avatarUrl: string
}

export const useFriendsStore = defineStore('friends', () => {
  // Stan (State)
  const friends = ref<UserInfoDto[]>([])
  const friendsPending = ref<UserInfoDto[]>([])

  const isLoading = ref(false)
  const isFriendsLoading = ref(false)
  const isPendingLoading = ref(false)

  // Akcje (Actions)
  const sendFriendRequest = async (friendId: string) => {
    if (!friendId) return
    isLoading.value = true
    try {
      await axios.post(`${API_URL}/api/friend/request/${friendId}`)
      // Opcjonalnie: odśwież listę oczekujących wysłanych, jeśli taką masz
    } catch (e) {
      console.error(e)
      throw e // Rzuć błąd dalej, by komponent mógł wyświetlić np. toast
    } finally {
      isLoading.value = false
    }
  }

  const fetchFriends = async () => {
    if (isFriendsLoading.value) return
    isFriendsLoading.value = true
    try {
      const response = await axios.get(`${API_URL}/api/friend/all`)
      friends.value = response.data.map((f: any) => ({
        ...f,
        avatarUrl: resolveAvatarUrl(f.avatarUrl, f.userName),
      }))
    } catch (e) {
      console.error(e)
    } finally {
      isFriendsLoading.value = false
    }
  }

  const fetchPending = async () => {
    if (isPendingLoading.value) return
    isPendingLoading.value = true
    try {
      const response = await axios.get(`${API_URL}/api/friend/pending`)
      friendsPending.value = response.data.map((f: any) => ({
        ...f,
        avatarUrl: resolveAvatarUrl(f.avatarUrl, f.userName),
      }))
    } catch (e) {
      console.error(e)
    } finally {
      isPendingLoading.value = false
    }
  }

  const resolvePending = async (friendId: number, accept: boolean) => {
    try {
      let response
      if (accept) {
        response = await axios.put(`${API_URL}/api/friend/accept/${friendId}`)
      } else {
        response = await axios.delete(`${API_URL}/api/friend/${friendId}`)
      }
      friendsPending.value = friendsPending.value.filter((f) => f.id !== friendId)
      if (accept) await fetchFriends()
    } catch (e) {
      console.error(e)
    }
  }

  return {
    friends,
    friendsPending,
    isLoading,
    isFriendsLoading,
    isPendingLoading,
    sendFriendRequest,
    fetchFriends,
    fetchPending,
    resolvePending,
  }
})
