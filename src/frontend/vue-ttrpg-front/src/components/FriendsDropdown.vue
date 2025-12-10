<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useFriendsStore } from '@/stores/friends'
import { storeToRefs } from 'pinia'
import type { UserInfoDto } from '@/stores/friends' // importuj typ ze store

import { useChatStore } from '@/stores/chat' // Import
const chatStore = useChatStore()

const store = useFriendsStore()
const { friends, friendsPending, isLoading, isFriendsLoading, isPendingLoading } =
  storeToRefs(store)

// Lokalne UI state dla formularza
const friendForm = reactive({ id: '' })

// Funkcje obsługi UI
const handleSendRequest = async () => {
  await store.sendFriendRequest(friendForm.id)
  friendForm.id = ''
}

const handleResolve = (friend: UserInfoDto, accept: boolean) => {
  store.resolvePending(friend.id, accept)
}

// Możemy załadować dane od razu po otwarciu komponentu (jeśli jest montowany warunkowo v-if)
// lub wywołać to z rodzica. Tutaj przykład ładowania przy montowaniu:
onMounted(() => {
  store.fetchFriends()
  store.fetchPending()
})

const goToProfile = (friend: UserInfoDto) => console.log('Navigating to', friend.userName)

const openChat = (friend: UserInfoDto) => {
  chatStore.openChat(friend)
}

const props = defineProps<{
  // Ta funkcja jest wywoływana, gdy użytkownik chce zamknąć okno
  // Rodzic przekazał nam funkcję 'toggleFriendsWindow'
  onClose: () => void
}>()
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
      X
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
        class="whitespace-nowrap rounded-lg bg-emerald-600 px-3 py-2 text-sm font-semibold text-white hover:bg-emerald-500 disabled:opacity-40 disabled:cursor-not-allowed"
      >
        <span v-if="isLoading">...</span>
        <span v-else>+</span>
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
      <button
        @click="goToProfile(f)"
        class="flex items-center gap-3 w-3/4 p-1 text-left focus:outline-none"
      >
        <img
          :src="f.avatarUrl || 'https://via.placeholder.com/32'"
          class="h-8 w-8 rounded-full border border-slate-700 object-cover"
        />
        <span class="text-slate-200 text-sm truncate">{{ f.userName }}</span>
      </button>
      <button
        class="text-emerald-400 hover:text-emerald-300 text-xs font-medium px-2 py-1 opacity-0 group-hover:opacity-100 transition-opacity"
        @click="openChat(f)"
      >
        Chat
      </button>
    </div>

    <h3 class="block text-sm text-slate-400 m-2 px-1 border-t border-slate-800 pt-2">
      Oczekujące zaproszenia
    </h3>

    <div v-if="isPendingLoading" class="text-slate-500 text-xs text-center p-2">Ładowanie...</div>
    <div v-else-if="friendsPending.length === 0" class="text-slate-500 text-xs text-center p-2">
      Brak oczekujących
    </div>

    <div v-else>
      <div
        v-for="f in friendsPending"
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
          @click="handleResolve(f, true)"
        >
          ✓
        </button>
        <button
          class="text-red-400 hover:text-red-300 w-1/4 hover:bg-red-200/20 rounded-md text-base font-medium px-2 py-1"
          @click="handleResolve(f, false)"
        >
          X
        </button>
      </div>
    </div>
  </div>
</template>
