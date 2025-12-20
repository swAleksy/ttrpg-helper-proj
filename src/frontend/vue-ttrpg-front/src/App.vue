<script setup lang="ts">
/**
 * Root Application Component
 * Contains Navbar, main content area, and chat panels
 */
import { storeToRefs } from 'pinia'
import Navbar from '@/components/Navbar.vue'
import ChatPanel from '@/components/ChatPanel.vue'
import { useChatStore } from '@/stores/chatStore'

const chatStore = useChatStore()
const { activeChats } = storeToRefs(chatStore)
</script>

<template>
  <div class="min-h-screen flex flex-col bg-slate-950 text-slate-50">
    <Navbar />

    <main class="flex-1">
      <RouterView />
    </main>
  </div>

  <!-- Chat Windows -->
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
