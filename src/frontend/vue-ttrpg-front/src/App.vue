<script setup lang="ts">
import Navbar from '@/components/Navbar.vue'
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import ChatPanel from './components/ChatPanel.vue'

// 1. Importujemy store czatu i helper z Pinia
import { useChatStore } from '@/stores/chat'
import { storeToRefs } from 'pinia'

const auth = useAuthStore()

// 2. Inicjalizujemy store
const chatStore = useChatStore()

// 3. Wyciągamy activeChats jako reaktywny ref
// Dzięki temu App.vue będzie wiedział, kiedy lista się zmieni
const { activeChats } = storeToRefs(chatStore)

onMounted(() => {
  auth.initializeAuth()
})
</script>

<template>
  <div class="min-h-screen flex flex-col bg-slate-950 text-slate-50">
    <Navbar />

    <main class="flex-1">
      <RouterView />
    </main>
  </div>

  <Teleport to="body">
    <div
      v-for="(user, index) in activeChats"
      :key="user.id"
      class="fixed bottom-0 z-50"
      :style="{ right: `${index * 320 + 20}px` }"
    >
      <ChatPanel :user="user" @close="chatStore.closeChat(user.id)" />
    </div>
  </Teleport>
</template>
