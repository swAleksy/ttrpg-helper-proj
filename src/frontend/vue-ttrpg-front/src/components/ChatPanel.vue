<script setup lang="ts">
/**
 * Chat Panel Component
 * Individual chat window for private messaging
 */
import { ref, onMounted, onUnmounted, nextTick, computed } from 'vue'
import axios from 'axios'
import { useAuthStore } from '@/stores/auth'
import { useCommunicationStore } from '@/stores/communicationStore'
import { useChatStore } from '@/stores/chatStore'
import { API_URL } from '@/config/api'
import type { UserInfoDto, MessageDto } from '@/types'

const props = defineProps<{
  user: UserInfoDto
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()

const authStore = useAuthStore()
const commStore = useCommunicationStore()
const chatStore = useChatStore()
// State
const messages = ref<MessageDto[]>([])
const newMessage = ref('')
const chatContainer = ref<HTMLElement | null>(null)

// Computed
const currentUserId = computed(() => authStore.user?.id)

// Actions
const scrollToBottom = async () => {
  await nextTick()
  if (chatContainer.value) {
    chatContainer.value.scrollTop = chatContainer.value.scrollHeight
  }
}

const loadHistory = async () => {
  if (!authStore.token) return

  try {
    const response = await axios.get<MessageDto[]>(`${API_URL}/api/chat/history/${props.user.id}`)
    messages.value = response.data
    scrollToBottom()
  } catch (error) {
    console.error('Error loading chat history:', error)
  }
}

const startListeningToChat = () => {
  if (!commStore.connection) {
    console.warn('SignalR connection not ready')
    return
  }

  const handler = (msg: MessageDto) => {
    const isRelevant = msg.senderId === props.user.id || msg.senderId === currentUserId.value
    const exists = messages.value.some((m) => m.id === msg.id)

    if (!exists && isRelevant) {
      messages.value.push(msg)
      scrollToBottom()
    }
  }

  commStore.connection.on('ReceivePrivateMessage', handler)

  return () => {
    commStore.connection?.off('ReceivePrivateMessage', handler)
  }
}

const sendMessage = async () => {
  if (!newMessage.value.trim() || !commStore.connection || !currentUserId.value) return

  const contentToSend = newMessage.value
  newMessage.value = ''

  try {
    await commStore.connection.invoke('SendPrivateMessage', props.user.id, contentToSend)
  } catch (error) {
    console.error('Error sending message:', error)
    newMessage.value = contentToSend
  }
}

const formatTime = (dateStr: string) => {
  const date = new Date(dateStr)
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const openChat = (friend: UserInfoDto) => {
  chatStore.openChat(friend)
}

// Lifecycle
let stopListening: (() => void) | undefined

onMounted(async () => {
  if (!authStore.user) {
    await authStore.fetchCurrentUser()
  }
  await loadHistory()
  stopListening = startListeningToChat()
})

onUnmounted(() => {
  stopListening?.()
})
</script>

<template>
  <div
    class="chat-popup fixed bottom-0 right-4 w-80 bg-slate-900 border border-slate-700 shadow-xl rounded-t-xl z-50 flex flex-col h-96"
  >
    <div
      class="flex justify-between items-center p-3 bg-slate-800 border-b border-slate-700 rounded-t-xl shrink-0"
    >
      <div class="flex items-center gap-2">
        <img
          :src="props.user.avatarUrl || 'https://via.placeholder.com/32'"
          class="h-8 w-8 rounded-full border border-slate-700 object-cover"
        />
        <span class="text-slate-200 font-semibold truncate max-w-[120px]">{{
          props.user.userName
        }}</span>
      </div>
      <button @click="emit('close')" class="text-slate-400 hover:text-white transition-colors p-1">
        ✕
      </button>
    </div>

    <div ref="chatContainer" class="chat-content flex-1 p-3 overflow-y-auto bg-slate-900 space-y-3">
      <div
        v-for="(msg, index) in messages"
        :key="msg.id || index"
        class="flex flex-col"
        :class="msg.senderId === currentUserId ? 'items-end' : 'items-start'"
      >
        <div
          class="max-w-[85%] px-3 py-2 rounded-lg text-sm wrap-break-word"
          :class="[
            msg.senderId === currentUserId
              ? 'bg-emerald-600 text-white rounded-br-none'
              : 'bg-slate-700 text-slate-200 rounded-bl-none',
          ]"
        >
          {{ msg.content }}
        </div>
        <span class="text-[10px] text-slate-500 mt-1 px-1">
          {{ formatTime(msg.sentAt) }}
        </span>
      </div>
    </div>

    <div class="p-3 bg-slate-800 border-t border-slate-700 shrink-0">
      <div class="flex gap-2">
        <input
          v-model="newMessage"
          @keyup.enter="sendMessage"
          type="text"
          placeholder="Napisz wiadomość..."
          class="flex-1 bg-slate-900 text-slate-200 text-sm rounded-md px-3 py-2 border border-slate-600 focus:border-emerald-500 focus:outline-none placeholder-slate-500"
        />
        <button
          @click="sendMessage"
          class="bg-emerald-600 hover:bg-emerald-500 text-white rounded-md px-3 py-2 transition-colors"
        >
          ➤
        </button>
      </div>
    </div>
  </div>
</template>
