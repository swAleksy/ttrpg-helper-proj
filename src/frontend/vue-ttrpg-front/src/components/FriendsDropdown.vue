<script setup lang="ts">
/**
 * Friends Dropdown Component
 * Displays friends list, pending requests, and add friend form
 */
import { reactive, onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useRouter } from 'vue-router'
import { useFriendsStore } from '@/stores/friendsStore'
import { useChatStore } from '@/stores/chatStore'
import { useNotificationStore } from '@/stores/notificationStore'

import type { UserInfoDto } from '@/types'

const props = defineProps<{
  onClose: () => void
}>()

const router = useRouter()
const friendsStore = useFriendsStore()
const chatStore = useChatStore()
const notificationStore = useNotificationStore()

const { friends, pendingRequests, isLoading, isFriendsLoading, isPendingLoading } =
  storeToRefs(friendsStore)

// Form state
const friendForm = reactive({ id: '' })

// Actions
const handleSendRequest = async () => {
  if (!friendForm.id) return
  await friendsStore.sendFriendRequest(friendForm.id)
  friendForm.id = ''
}

const handleAccept = (friend: UserInfoDto) => {
  friendsStore.acceptRequest(friend.id)
}

const handleReject = (friend: UserInfoDto) => {
  friendsStore.rejectRequest(friend.id)
}

const goToProfile = (friend: UserInfoDto) => {
  router.push(`/profile/${friend.id}`)
  props.onClose()
}

const openChat = (friend: UserInfoDto) => {
  chatStore.openChat(friend)
  props.onClose()
}

onMounted(() => {
  friendsStore.fetchFriends()
  friendsStore.fetchPending()
})
</script>

<template>
  <div
    class="absolute right-0 mt-2 w-64 bg-slate-900 border border-slate-700 rounded-xl shadow-xl p-2 z-50"
  >
    <button
      @click="props.onClose"
      class="absolute top-2 right-2 text-slate-500 hover:text-white transition-colors p-1"
      aria-label="Zamknij menu"
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        fill="none"
        viewBox="0 0 24 24"
        stroke-width="1.5"
        stroke="currentColor"
        class="h-5 w-5"
      >
        <path stroke-linecap="round" stroke-linejoin="round" d="M6 18 18 6M6 6l12 12" />
      </svg>
    </button>
    <label class="block text-sm text-slate-400 m-2 px-1">Dodaj Znajomego</label>
    <div class="flex gap-2 px-1 mb-4">
      <input
        v-model="friendForm.id"
        placeholder="ID użytkownika"
        type="text"
        class="w-full rounded-lg border border-slate-700 bg-slate-950 px-3 py-2 text-sm text-white focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
      />
      <button
        @click="handleSendRequest"
        :disabled="isLoading || !friendForm.id"
        class="flex items-center justify-center whitespace-nowrap rounded-lg bg-emerald-600 px-3 py-2 text-sm font-semibold text-white hover:bg-emerald-500 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
      >
        <span v-if="isLoading" class="flex gap-1">
          <span class="animate-pulse">.</span
          ><span class="animate-pulse [animation-delay:0.2s]">.</span
          ><span class="animate-pulse [animation-delay:0.4s]">.</span>
        </span>

        <svg
          v-else
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          stroke-width="2"
          stroke="currentColor"
          class="h-5 w-5"
        >
          <path stroke-linecap="round" stroke-linejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
        </svg>
      </button>
    </div>

    <h3 class="block text-sm text-slate-400 m-2 px-1 border-t border-slate-800 pt-2">Znajomi</h3>

    <div v-if="isFriendsLoading" class="text-slate-500 text-xs text-center p-2">Ładowanie...</div>
    <div v-else-if="friends.length === 0" class="text-slate-500 text-xs text-center p-2">
      Brak znajomych
    </div>

    <div
      v-else
      v-for="f in friends"
      :key="f.id"
      class="flex items-center justify-between p-1 rounded hover:bg-slate-800 group"
    >
      <button @click="goToProfile(f)" class="flex items-center gap-3 w-3/4 p-1 text-left relative">
        <img
          :src="f.avatarUrl || '...'"
          class="h-8 w-8 rounded-full border border-slate-700 object-cover"
        />

        <span
          v-if="notificationStore.hasUnreadFrom(f.id)"
          class="absolute left-7 top-1 flex h-3 w-3"
        >
          <span
            class="animate-ping absolute inline-flex h-full w-full rounded-full bg-red-400 opacity-75"
          ></span>
          <span class="relative inline-flex rounded-full h-3 w-3 bg-red-500"></span>
        </span>

        <span class="text-slate-200 text-sm truncate">{{ f.userName }}</span>
      </button>

      <button @click="openChat(f)" class="text-emerald-400 hover:text-emerald-300 text-xs">
        Chat
      </button>
    </div>

    <h3 class="block text-sm text-slate-400 m-2 px-1 border-t border-slate-800 pt-2">
      Oczekujące zaproszenia
    </h3>

    <div v-if="isPendingLoading" class="text-slate-500 text-xs text-center p-2">Ładowanie...</div>
    <div v-else-if="pendingRequests.length === 0" class="text-slate-500 text-xs text-center p-2">
      Brak oczekujących
    </div>

    <div v-else>
      <div
        v-for="f in pendingRequests"
        :key="f.id"
        class="flex items-center justify-between p-1 rounded hover:bg-slate-800 group"
      >
        <button
          @click="goToProfile(f)"
          class="flex items-center gap-3 w-2/4 p-1 text-left focus:outline-none"
        >
          <img
            :src="f.avatarUrl || 'https://via.placeholder.com/32'"
            class="h-8 w-8 rounded-full border border-slate-700 object-cover"
          />
          <span class="text-slate-200 text-sm truncate">{{ f.userName }}</span>
        </button>
        <button
          class="text-emerald-400 hover:text-emerald-300 hover:bg-emerald-200/20 rounded-md w-1/4 text-base font-medium px-2 py-1"
          @click="handleAccept(f)"
        >
          ✓
        </button>
        <button
          class="text-red-400 hover:text-red-300 w-1/4 hover:bg-red-200/20 rounded-md text-base font-medium px-2 py-1"
          @click="handleReject(f)"
        >
          ✕
        </button>
      </div>
    </div>
  </div>
</template>
