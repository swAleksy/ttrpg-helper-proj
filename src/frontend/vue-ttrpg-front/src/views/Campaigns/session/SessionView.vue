<script setup lang="ts">
import { onMounted, onBeforeUnmount, computed, ref, nextTick, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useSessionStore } from '@/stores/SessionStore'

const route = useRoute()
const sessionStore = useSessionStore()

// Reaktywne dane z Pinia
const session = computed(() => sessionStore.session)
const isLoading = computed(() => sessionStore.isLoading)

// --- MOCK DANE DO WIZUALIZACJI (zastąp logiką backendową w przyszłości) ---
const newPlayerId = ref('')
const chatInput = ref('')
const chatContainer = ref<HTMLElement | null>(null)

// Przykładowe logi/wiadomości w czacie
const chatLogs = ref([
  { id: 1, type: 'system', content: 'Sesja została rozpoczęta.', time: '20:00' },
  {
    id: 2,
    type: 'user',
    author: 'Mistrz Gry',
    content: 'Witacie w mrocznych lochach. Przed wami rozciąga się długi korytarz.',
    time: '20:05',
  },
  {
    id: 3,
    type: 'roll',
    author: 'System',
    content: 'Geralt rzucił na percepcję: 18 (Sukces)',
    time: '20:06',
  },
  {
    id: 4,
    type: 'user',
    author: 'Geralt',
    content: 'Czy widzę jakieś pułapki na podłodze?',
    time: '20:07',
  },
])

// Przykładowe kafelki akcji
const actions = [
  { id: 1, name: 'Rzut Kością', icon: 'd20', color: 'text-emerald-400' },
  { id: 2, name: 'Test Umiejętności', icon: 'skill', color: 'text-blue-400' },
  { id: 3, name: 'Inicjatywa', icon: 'flash', color: 'text-yellow-400' },
  { id: 4, name: 'Odpoczynek', icon: 'moon', color: 'text-slate-400' },
]
// --------------------------------------------------------------------------

onMounted(async () => {
  await sessionStore.fetchSessionById(Number(route.params.id))
  scrollToBottom()
})

onBeforeUnmount(() => {
  sessionStore.clearSession()
})

// Automatyczne przewijanie czatu w dół
const scrollToBottom = () => {
  nextTick(() => {
    if (chatContainer.value) {
      chatContainer.value.scrollTop = chatContainer.value.scrollHeight
    }
  })
}

// Funkcja obsługi dodawania gracza (mock)
const handleAddPlayer = () => {
  if (!newPlayerId.value) return
  alert(`Tu wyślesz request API dodający gracza o ID: ${newPlayerId.value}`)
  newPlayerId.value = ''
}

// Funkcja obsługi wysyłania wiadomości (mock)
const handleSendMessage = () => {
  if (!chatInput.value.trim()) return
  chatLogs.value.push({
    id: Date.now(),
    type: 'user',
    author: 'Ty',
    content: chatInput.value,
    time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
  })
  chatInput.value = ''
  scrollToBottom()
}

// Funkcja obsługi akcji (mock)
const triggerAction = (actionName: string) => {
  chatLogs.value.push({
    id: Date.now(),
    type: 'system',
    content: `Aktywowano akcję: ${actionName}`,
    time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
  })
  scrollToBottom()
}

// Helper do inicjałów avatara
const getInitials = (name: string) => name.charAt(0).toUpperCase()
</script>

<template>
  <div class="min-h-screen bg-slate-950 text-slate-100 p-4 md:p-6 lg:p-8 font-sans">
    <div v-if="isLoading || !session" class="flex justify-center items-center h-[80vh]">
      <div
        class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-emerald-400"
      ></div>
    </div>

    <div v-else class="max-w-7xl mx-auto space-y-6">
      <header
        class="flex flex-col md:flex-row md:items-center justify-between gap-4 border-b border-slate-800 pb-6"
      >
        <div>
          <div class="flex items-center gap-3">
            <h1 class="text-3xl font-bold tracking-tight text-slate-100">{{ session.name }}</h1>
            <span
              class="px-3 py-1 rounded-full text-xs font-medium bg-emerald-500/10 text-emerald-400 border border-emerald-500/20"
            >
              {{ session.status || 'AKTYWNA' }}
            </span>
          </div>
          <p class="text-slate-400 mt-2 max-w-2xl text-sm">
            {{ session.description || 'Brak opisu sesji.' }}
          </p>
        </div>
        <div class="flex flex-col items-end text-sm text-slate-500">
          <span>Data: {{ session.scheduledDate }}</span>
          <span>ID: #{{ session.id }}</span>
        </div>
      </header>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 h-[calc(100vh-250px)] min-h-[600px]">
        <div
          class="lg:col-span-2 flex flex-col bg-slate-900/40 backdrop-blur-sm border border-slate-800 rounded-2xl overflow-hidden shadow-xl"
        >
          <div
            class="p-4 border-b border-slate-800 bg-slate-900/60 flex items-center justify-between"
          >
            <span class="font-semibold text-slate-300 flex items-center gap-2">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-5 w-5 text-emerald-400"
                viewBox="0 0 20 20"
                fill="currentColor"
              >
                <path
                  fill-rule="evenodd"
                  d="M18 10c0 3.866-3.582 7-8 7a8.841 8.841 0 01-4.083-.98L2 17l1.338-3.123C2.493 12.767 2 11.434 2 10c0-3.866 3.582-7 8-7s8 3.134 8 7zM7 9H5v2h2V9zm8 0h-2v2h2V9zM9 9h2v2H9V9z"
                  clip-rule="evenodd"
                />
              </svg>
              Logi Sesji
            </span>
            <span class="text-xs text-slate-500">Live feed</span>
          </div>

          <div
            ref="chatContainer"
            class="flex-1 overflow-y-auto p-4 space-y-4 scrollbar-thin scrollbar-thumb-slate-700 scrollbar-track-transparent"
          >
            <div v-for="log in chatLogs" :key="log.id" class="animate-fade-in-up">
              <div v-if="log.type === 'system'" class="flex justify-center my-4">
                <span
                  class="text-xs font-mono text-slate-500 bg-slate-900/80 px-3 py-1 rounded-full border border-slate-800"
                >
                  {{ log.content }}
                </span>
              </div>

              <div
                v-else-if="log.type === 'roll'"
                class="flex gap-3 bg-slate-800/30 p-3 rounded-lg border-l-2 border-yellow-500/50"
              >
                <div class="text-yellow-500 mt-1">
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-5 w-5"
                    viewBox="0 0 20 20"
                    fill="currentColor"
                  >
                    <path
                      d="M10 2a6 6 0 00-6 6v3.586l-.707.707A1 1 0 004 14h12a1 1 0 00.707-1.707L16 11.586V8a6 6 0 00-6-6zM10 18a3 3 0 01-3-3h6a3 3 0 01-3 3z"
                    />
                  </svg>
                </div>
                <div>
                  <span class="block text-xs font-bold text-yellow-500 mb-0.5">{{
                    log.author
                  }}</span>
                  <p class="text-sm text-slate-300">{{ log.content }}</p>
                </div>
              </div>

              <div v-else class="flex gap-4 group">
                <div
                  class="flex-shrink-0 w-8 h-8 rounded bg-slate-800 flex items-center justify-center text-xs font-bold text-emerald-400 border border-slate-700"
                >
                  {{ log.author ? getInitials(log.author) : '?' }}
                </div>
                <div class="flex-1">
                  <div class="flex items-baseline gap-2 mb-1">
                    <span class="text-sm font-bold text-emerald-400">{{ log.author }}</span>
                    <span class="text-xs text-slate-600">{{ log.time }}</span>
                  </div>
                  <p class="text-slate-300 text-sm leading-relaxed">{{ log.content }}</p>
                </div>
              </div>
            </div>
          </div>

          <div class="p-4 bg-slate-900 border-t border-slate-800">
            <form @submit.prevent="handleSendMessage" class="relative">
              <input
                v-model="chatInput"
                type="text"
                placeholder="Napisz wiadomość lub komendę..."
                class="w-full bg-slate-950 text-slate-100 rounded-xl border border-slate-700 py-3 pl-4 pr-12 focus:outline-none focus:border-emerald-500 focus:ring-1 focus:ring-emerald-500 transition-all placeholder:text-slate-600"
              />
              <button
                type="submit"
                class="absolute right-2 top-1/2 -translate-y-1/2 p-2 text-slate-400 hover:text-emerald-400 transition-colors"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    d="M10.894 2.553a1 1 0 00-1.788 0l-7 14a1 1 0 001.169 1.409l5-1.429A1 1 0 009 15.571V11a1 1 0 112 0v4.571a1 1 0 00.725.962l5 1.428a1 1 0 001.17-1.408l-7-14z"
                  />
                </svg>
              </button>
            </form>
          </div>
        </div>

        <div class="flex flex-col gap-6">
          <section
            class="bg-slate-900/40 backdrop-blur-sm border border-slate-800 rounded-2xl p-4 flex-1 max-h-[50%] overflow-y-auto"
          >
            <h3
              class="text-sm font-semibold text-slate-400 uppercase tracking-wider mb-4 flex items-center gap-2"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-4 w-4"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M13 10V3L4 14h7v7l9-11h-7z"
                />
              </svg>
              Dostępne Akcje
            </h3>
            <div class="grid grid-cols-2 gap-3">
              <button
                v-for="action in actions"
                :key="action.id"
                @click="triggerAction(action.name)"
                class="group flex flex-col items-center justify-center p-4 bg-slate-950 border border-slate-800 rounded-xl hover:border-emerald-500/50 hover:bg-emerald-500/5 transition-all duration-300"
              >
                <div class="mb-2 transition-transform group-hover:scale-110" :class="action.color">
                  <svg
                    v-if="action.icon === 'd20'"
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-6 w-6"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    stroke-width="2"
                  >
                    <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5" />
                  </svg>
                  <svg
                    v-if="action.icon === 'skill'"
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-6 w-6"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"
                    />
                  </svg>
                  <svg
                    v-if="action.icon === 'flash'"
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-6 w-6"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M13 10V3L4 14h7v7l9-11h-7z"
                    />
                  </svg>
                  <svg
                    v-if="action.icon === 'moon'"
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-6 w-6"
                    fill="none"
                    viewBox="0 0 24 24"
                    stroke="currentColor"
                  >
                    <path
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      stroke-width="2"
                      d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"
                    />
                  </svg>
                </div>
                <span
                  class="text-xs font-medium text-slate-400 group-hover:text-emerald-400 transition-colors"
                  >{{ action.name }}</span
                >
              </button>
            </div>
          </section>

          <section
            class="bg-slate-900/40 backdrop-blur-sm border border-slate-800 rounded-2xl p-4 flex-1 flex flex-col"
          >
            <h3
              class="text-sm font-semibold text-slate-400 uppercase tracking-wider mb-4 flex items-center gap-2"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-4 w-4"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"
                />
              </svg>
              Uczestnicy ({{ session.players.length }})
            </h3>

            <div
              class="flex-1 overflow-y-auto space-y-3 mb-4 pr-1 scrollbar-thin scrollbar-thumb-slate-700"
            >
              <div
                v-for="player in session.players"
                :key="player.playerId"
                class="flex items-center justify-between p-2 rounded-lg bg-slate-900/50 hover:bg-slate-800 transition-colors border border-transparent hover:border-slate-700"
              >
                <div class="flex items-center gap-3">
                  <div
                    class="w-8 h-8 rounded-full bg-slate-800 border border-slate-600 flex items-center justify-center text-xs font-bold text-blue-400"
                  >
                    {{ getInitials(player.playerName) }}
                  </div>
                  <div>
                    <p class="text-sm font-medium text-slate-200">{{ player.playerName }}</p>
                    <p class="text-[10px] text-slate-500 font-mono">ID: {{ player.playerId }}</p>
                  </div>
                </div>
                <div
                  class="h-2 w-2 rounded-full bg-emerald-500 shadow-[0_0_8px_rgba(16,185,129,0.5)]"
                ></div>
              </div>

              <div
                v-if="session.players.length === 0"
                class="text-center py-4 text-slate-500 text-sm"
              >
                Brak graczy w tej sesji.
              </div>
            </div>

            <div class="mt-auto pt-4 border-t border-slate-800">
              <label class="block text-xs font-medium text-slate-500 mb-2"
                >Dodaj nowego gracza</label
              >
              <div class="flex gap-2">
                <input
                  v-model="newPlayerId"
                  type="text"
                  placeholder="ID Gracza"
                  class="flex-1 bg-slate-950 text-sm text-slate-100 rounded-lg border border-slate-700 px-3 py-2 focus:outline-none focus:border-emerald-500 transition-colors"
                  @keyup.enter="handleAddPlayer"
                />
                <button
                  @click="handleAddPlayer"
                  class="bg-emerald-600 hover:bg-emerald-500 text-white rounded-lg px-3 py-2 transition-colors flex items-center justify-center"
                  title="Dodaj gracza"
                >
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    class="h-5 w-5"
                    viewBox="0 0 20 20"
                    fill="currentColor"
                  >
                    <path
                      fill-rule="evenodd"
                      d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z"
                      clip-rule="evenodd"
                    />
                  </svg>
                </button>
              </div>
            </div>
          </section>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Opcjonalnie: animacja pojawiania się wiadomości */
</style>
