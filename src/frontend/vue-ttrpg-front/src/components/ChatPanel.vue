<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick, computed } from 'vue'
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr'
import axios from 'axios'
import { useAuthStore, API_URL } from '@/stores/auth' // Importujemy Twój store
import { useCommunicationStore } from '@/stores/communicationStore'
import type { UserInfoDto } from '@/stores/friendsStore' // Zakładam, że tu masz ten typ

// --- TYPY ---
interface MessageDto {
  id: number
  senderId: number
  senderName: string
  receiverId: number
  content: string
  sentAt: string
  isRead: boolean
}

const props = defineProps<{
  user: UserInfoDto // Rozmówca (Friend)
}>()

const emit = defineEmits<{
  (e: 'close'): void
}>()

const authStore = useAuthStore()
const commStore = useCommunicationStore()

const messages = ref<MessageDto[]>([])
const newMessage = ref('')
const chatContainer = ref<HTMLElement | null>(null)

const currentUserId = computed(() => authStore.user?.id)
const token = computed(() => authStore.token)

// 1. Scrollowanie do dołu
const scrollToBottom = async () => {
  await nextTick()
  if (chatContainer.value) {
    chatContainer.value.scrollTop = chatContainer.value.scrollHeight
  }
}

// 2. Pobieranie historii wiadomości
const loadHistory = async () => {
  if (!token.value) return // Zabezpieczenie

  try {
    const response = await axios.get<MessageDto[]>(`${API_URL}/api/chat/history/${props.user.id}`, {
      headers: { Authorization: `Bearer ${token.value}` },
    })
    messages.value = response.data
    scrollToBottom()
  } catch (err) {
    console.error('Błąd pobierania historii:', err)
  }
}

// 3. Nasłuchiwanie na wiadomości z centralnego Huba
const startListeningToChat = () => {
  if (!commStore.connection) {
    console.warn('SignalR connection not ready.')
    return
  }

  // Funkcja, która dodaje wiadomość TYLKO jeśli dotyczy tej rozmowy
  const handler = (msg: MessageDto) => {
    // Musimy sprawdzić, czy ta wiadomość dotyczy TEJ konkretnej rozmowy.
    const isRelevant =
      (msg.senderId === props.user.id && msg.receiverId === currentUserId.value) ||
      (msg.senderId === currentUserId.value && msg.receiverId === props.user.id)

    // W Twoim przypadku backend wysyła tylko do obu użytkowników w rozmowie,
    // więc wystarczy sprawdzenie, czy ID nadawcy to nasz rozmówca lub my sami.
    const isRelevantSimple = msg.senderId === props.user.id || msg.senderId === currentUserId.value

    const exists = messages.value.some((m) => m.id === msg.id)

    if (!exists && isRelevantSimple) {
      messages.value.push(msg)
      scrollToBottom()
    }
  }

  // Używamy metody 'on' na centralnym połączeniu
  // Ten handler jest aktywowany tylko, gdy ten komponent jest zamontowany.
  commStore.connection.on('ReceivePrivateMessage', handler)

  // Zwracamy funkcję do usunięcia handlera, gdy komponent się odmontowuje
  return () => {
    commStore.connection?.off('ReceivePrivateMessage', handler)
  }
}

// 4. Wysyłanie wiadomości - Zmienione
const sendMessage = async () => {
  // Sprawdzamy, czy połączenie ze Store jest aktywne
  if (!newMessage.value.trim() || !commStore.connection || !currentUserId.value) return

  const contentToSend = newMessage.value
  newMessage.value = ''

  try {
    // Używamy connection ze store: commStore.connection!
    await commStore.connection.invoke('SendPrivateMessage', props.user.id, contentToSend)
  } catch (err) {
    console.error('Błąd wysyłania:', err)
    newMessage.value = contentToSend
  }
}

// 5. Formatowanie daty (np. 12:45)
const formatTime = (dateStr: string) => {
  const date = new Date(dateStr)
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

// --- LIFECYCLE ---
let stopListening: (() => void) | null | undefined = undefined

onMounted(async () => {
  if (!authStore.user) {
    await authStore.fetchCurrentUser()
  }

  await loadHistory()

  // Zamiast initSignalR(), zaczynamy nasłuchiwać
  // W idealnym świecie, commStore.connection powinien już być aktywy z Navbara
  stopListening = startListeningToChat()
})

onUnmounted(() => {
  // Czyścimy subskrypcję, aby inne otwarte okna czatu nie dostawały wiadomości
  // przeznaczonych dla tego okna, które się zamyka.
  if (stopListening) {
    stopListening()
  }
})
</script>

<template>
  <div
    class="chat-popup fixed bottom-0 right-4 w-80 bg-slate-900 border border-slate-700 shadow-xl rounded-t-xl z-50 flex flex-col h-96"
  >
    <div
      class="flex justify-between items-center p-3 bg-slate-800 border-b border-slate-700 rounded-t-xl flex-shrink-0"
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
          class="max-w-[85%] px-3 py-2 rounded-lg text-sm break-words"
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

    <div class="p-3 bg-slate-800 border-t border-slate-700 flex-shrink-0">
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
