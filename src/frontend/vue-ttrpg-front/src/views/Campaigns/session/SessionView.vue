<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { ref, computed, onMounted, onBeforeUnmount, nextTick, watch } from 'vue'
import type { ChatMessagePayload, DiceRollPayload } from '@/types'
import { useRoute } from 'vue-router'
import { useSessionStore } from '@/stores/SessionStore'
import { resolveAvatarUrl } from '@/utils/avatar'

const route = useRoute()
const sessionStore = useSessionStore()
const { playersWithAvatar, events, notes, items, npcs } = storeToRefs(sessionStore)

const session = computed(() => sessionStore.session)
const isLoading = computed(() => sessionStore.isLoading)
const campaignId = computed(() => Number(route.params.campaignId))

const sessionDateLabel = computed(() => formatSessionDatePL(session.value?.scheduledDate))

function formatSessionDatePL(input?: string | null) {
  if (!input) return '—'

  // backend -> 2026-01-03T18:24:00
  const m = input.match(/^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})$/)
  if (m) {
    const [, y, mo, d, h, mi] = m
    return `${d}.${mo}.${y}, godzina ${h}.${mi}`
  }

  // inny format -> wyświetl surowo
  return input
}

const avatarByUserId = computed<Record<number, string>>(() => {
  const map: Record<number, string> = {}

  playersWithAvatar.value.forEach((player) => {
    map[player.id] = player.resolvedAvatar
  })

  return map
})

const newPlayerId = ref('')
const chatInput = ref('')
const chatContainer = ref<HTMLElement | null>(null)
const showDicePanel = ref(false)

//  kafelki akcji
const diceTypes = ['d6', 'd8', 'd12', 'd20']

const actions = [
  {
    id: 1,
    name: 'Rzut Kością',
    color: 'text-yellow-400',
    icon: `
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="#facd0a" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="h-12 w-12">
        <rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect>
        <circle cx="8" cy="8" r="1" fill="#facd0a"></circle>
        <circle cx="16" cy="16" r="1" fill="#facd0a"></circle>
        <circle cx="16" cy="8" r="1" fill="#facd0a"></circle>
        <circle cx="8" cy="16" r="1" fill="#facd0a"></circle>
        <circle cx="12" cy="12" r="1" fill="#facd0a"></circle>
      </svg>`,
  },
  {
    id: 2,
    name: 'Test Umiejętności',
    color: 'text-blue-400',
    icon: `
      <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="#60a5fa" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="h-12 w-12">
        <circle cx="12" cy="12" r="10"></circle>
        <path d="M9.09 9a3 3 0 0 1 5.83 1c0 2-3 3-3 3"></path>
        <line x1="12" y1="17" x2="12.01" y2="17"></line>
      </svg>`,
  },
]

const activeTab = ref<'notes' | 'items' | 'npcs'>('notes')

onMounted(async () => {
  const sessionId = Number(route.params.id)
  if (sessionId) {
    await sessionStore.fetchSessionById(sessionId)
    await sessionStore.fetchCampaignData(campaignId.value)
  }
  await sessionStore.fetchEvents(sessionId) // Pobierz historię
  await sessionStore.initSignalR(sessionId)
  scrollToBottom()
})

onBeforeUnmount(() => {
  sessionStore.clearSession()
})

// const currentSessionId = () => sessionStore.session?.id

watch(
  () => events.value.length,
  () => {
    scrollToBottom()
  },
)

const scrollToBottom = () => {
  nextTick(() => {
    if (chatContainer.value) {
      chatContainer.value.scrollTop = chatContainer.value.scrollHeight
    }
  })
}

const getAvatarForEvent = (userId: number) => {
  return avatarByUserId.value[userId] || resolveAvatarUrl(null, '?')
}

const handleAddPlayer = async () => {
  if (!newPlayerId.value || !sessionStore.session) return

  const sessionId = sessionStore.session.id
  const playerId = Number(newPlayerId.value)

  if (Number.isNaN(playerId)) return

  await sessionStore.addPlayer(sessionId, playerId)
  await sessionStore.fetchSessionPlayersDetails()

  newPlayerId.value = ''
}

const handleSendMessage = async () => {
  if (!chatInput.value.trim()) return

  const payload: ChatMessagePayload = {
    message: chatInput.value,
  }

  await sessionStore.sendEvent('ChatMessage', payload)
  chatInput.value = ''
}

const rollDice = (dice: string) => {
  const max = Number(dice.replace('d', ''))
  return Math.floor(Math.random() * max) + 1
}

const triggerDiceRoll = async (dice: string) => {
  const result = rollDice(dice)

  const payload: DiceRollPayload = {
    dice: `1${dice}`,
    result,
  }

  await sessionStore.sendEvent('DiceRoll', payload)
  showDicePanel.value = false
}

const triggerAction = async (actionName: string) => {
  if (actionName === 'Rzut Kością') {
    showDicePanel.value = !showDicePanel.value
    return
  }

  if (actionName === 'Test Umiejętności') {
    // TO DO KIEDYS CLUELESS
  }
}
</script>

<template>
  <div class="min-h-screen bg-slate-950 text-slate-100 font-sans">
    <div class="mx-auto max-w-6xl px-4 py-8">
      <div v-if="isLoading || !session" class="flex justify-center items-center h-[80vh]">
        <div class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-emerald-400"></div>
      </div>

      <div v-else class="space-y-6">
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
            <span>Data: {{ sessionDateLabel }}</span>
            <span>ID: #{{ session.id }}</span>
          </div>
        </header>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 lg:h-[calc(100vh-250px)] lg:min-h-[600px]">
          <!-- Logi: dłuższe na wąskich ekranach -->
          <div
            class="lg:col-span-2 flex flex-col bg-slate-900/40 backdrop-blur-sm border border-slate-800 rounded-2xl overflow-hidden shadow-xl
                   h-[70vh] sm:h-[75vh] lg:h-full"
          >
            <div class="p-4 border-b border-slate-800 bg-slate-900/60 flex items-center justify-between">
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
              <div v-for="log in events" :key="log.id" class="animate-fade-in-up">
                <div v-if="log.type === 'ChatMessage'" class="flex gap-4">
                  <img
                    :src="getAvatarForEvent(log.userId)"
                    class="flex-shrink-0 w-8 h-8 rounded bg-slate-800 flex items-center justify-center text-xs font-bold text-emerald-400"
                  />
                  <div class="flex-1">
                    <div class="flex items-baseline gap-2">
                      <span class="text-sm font-bold text-emerald-400">{{ log.userName }}</span>
                      <span class="text-xs text-slate-600">
                        {{ log.timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }) }}
                      </span>
                    </div>
                    <p class="text-slate-300 text-sm">{{ log.data.message }}</p>
                  </div>
                </div>

                <div v-else-if="log.type === 'UserJoined'" class="flex justify-center w-full">
                  <div class="flex items-baseline gap-2">
                    <span class="text-xs font-bold text-slate-600">
                      {{ log.userName }} dołączył do sesji
                    </span>
                  </div>
                </div>

                <div v-else-if="log.type === 'UserLeft'" class="flex justify-center w-full">
                  <div class="flex items-baseline gap-2">
                    <span class="text-xs font-bold text-slate-600">
                      {{ log.userName }} opóścił sesję
                    </span>
                  </div>
                </div>

                <div
                  v-else-if="log.type === 'DiceRoll'"
                  class="flex gap-3 bg-slate-800/30 p-3 rounded-lg border-l-2 border-yellow-500/50"
                >
                  <div class="text-yellow-500 mt-1">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                      <path
                        d="M10 2a6 6 0 00-6 6v3.586l-.707.707A1 1 0 004 14h12a1 1 0 00.707-1.707L16 11.586V8a6 6 0 00-6-6zM10 18a3 3 0 01-3-3h6a3 3 0 01-3 3z"
                      />
                    </svg>
                  </div>
                  <div>
                    <span class="block text-xs font-bold text-yellow-500">
                      {{ log.userName }} rzuca {{ log.data.dice }}
                    </span>
                    <p class="text-lg font-bold text-slate-100">{{ log.data.result }}</p>
                  </div>
                </div>

                <div v-else class="flex gap-4 group">
                  <img
                    :src="getAvatarForEvent(log.userId)"
                    class="flex-shrink-0 w-8 h-8 rounded bg-slate-800 flex items-center justify-center text-xs font-bold text-emerald-400"
                  />
                  <div class="flex-1">
                    <div class="flex items-baseline gap-2 mb-1">
                      <span class="text-sm font-bold text-emerald-400">{{ log.userName }}</span>
                      <span class="text-xs text-slate-600">{{ log.type }}</span>
                    </div>
                    <p class="text-slate-300 text-sm leading-relaxed">{{ log.data }}</p>
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
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
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
                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
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
                  <span v-html="action.icon"></span>
                  <span
                    class="text-xs font-medium text-slate-400 group-hover:text-emerald-400 transition-colors"
                  >
                    {{ action.name }}
                  </span>
                </button>
              </div>

              <div v-if="showDicePanel" class="mt-4 grid grid-cols-4 gap-3">
                <button
                  v-for="dice in diceTypes"
                  :key="dice"
                  @click="triggerDiceRoll(dice)"
                  class="p-3 bg-slate-950 border border-slate-800 rounded-xl hover:border-yellow-400/50 hover:bg-yellow-400/10 transition"
                >
                  <span class="text-yellow-400 font-bold">{{ dice }}</span>
                </button>
              </div>
            </section>

            <section
              class="bg-slate-900/40 backdrop-blur-sm border border-slate-800 rounded-2xl p-4 flex-1 flex flex-col"
            >
              <h3
                class="text-sm font-semibold text-slate-400 uppercase tracking-wider mb-4 flex items-center gap-2"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z"
                  />
                </svg>
                Użytkownicy ({{ playersWithAvatar.length }})
              </h3>

              <div class="flex-1 overflow-y-auto space-y-3 mb-4 pr-1 scrollbar-thin scrollbar-thumb-slate-700">
                <div
                  v-for="player in playersWithAvatar"
                  :key="player.id"
                  class="flex items-center justify-between p-2 rounded-lg bg-slate-900/50 hover:bg-slate-800 transition-colors border border-transparent hover:border-slate-700"
                >
                  <div class="flex items-center gap-3">
                    <img
                      :src="player.resolvedAvatar"
                      class="w-8 h-8 rounded-full bg-slate-800 border border-slate-600 flex items-center justify-center"
                    />
                    <div>
                      <p class="text-sm font-medium text-slate-200">{{ player.userName }}</p>
                      <p class="text-[10px] text-slate-500 font-mono">ID: {{ player.id }}</p>
                    </div>
                  </div>
                </div>

                <div
                  v-if="session.players.length === 0"
                  class="text-center py-4 text-slate-500 text-sm"
                >
                  Brak graczy w tej sesji.
                </div>
              </div>

              <div class="mt-auto pt-4 border-t border-slate-800">
                <label class="block text-xs font-medium text-slate-500 mb-2">Dodaj nowego gracza</label>
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
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
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

        <!-- Tabs -->
        <div class="bg-slate-900/40 backdrop-blur-sm border border-slate-800 rounded-2xl overflow-hidden shadow-xl mt-6">
          <div class="flex border-b border-slate-800">
            <button
              @click="activeTab = 'notes'"
              :class="[
                'px-6 py-3 text-sm font-medium transition-colors',
                activeTab === 'notes'
                  ? 'text-emerald-400 border-b-2 border-emerald-400 bg-slate-800/50'
                  : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/30',
              ]"
            >
              Notatki
            </button>
            <button
              @click="activeTab = 'items'"
              :class="[
                'px-6 py-3 text-sm font-medium transition-colors',
                activeTab === 'items'
                  ? 'text-emerald-400 border-b-2 border-emerald-400 bg-slate-800/50'
                  : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/30',
              ]"
            >
              Przedmioty
            </button>
            <button
              @click="activeTab = 'npcs'"
              :class="[
                'px-6 py-3 text-sm font-medium transition-colors',
                activeTab === 'npcs'
                  ? 'text-emerald-400 border-b-2 border-emerald-400 bg-slate-800/50'
                  : 'text-slate-400 hover:text-slate-200 hover:bg-slate-800/30',
              ]"
            >
              NPC
            </button>
          </div>

          <div class="p-6 h-[400px] overflow-y-auto scrollbar-thin scrollbar-thumb-slate-700">
            <div v-if="activeTab === 'notes'" class="space-y-4">
              <div v-if="notes.length === 0" class="text-slate-500 text-center py-8">
                Brak notatek w tej kampanii.
              </div>
              <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div
                  v-for="note in notes"
                  :key="note.id"
                  class="bg-slate-950 p-4 rounded-xl border border-slate-800"
                >
                  <h4 class="font-bold text-emerald-400 mb-2">{{ note.name }}</h4>
                  <p class="text-slate-300 text-sm whitespace-pre-wrap">{{ note.contentMarkdown }}</p>
                </div>
              </div>
            </div>

            <div v-if="activeTab === 'items'">
              <div v-if="items.length === 0" class="text-slate-500 text-center py-8">
                Brak przedmiotów.
              </div>
              <table v-else class="w-full text-left text-sm text-slate-300">
                <thead class="text-xs uppercase bg-slate-950 text-slate-500 sticky top-0">
                  <tr>
                    <th class="px-4 py-3 rounded-tl-lg">Nazwa</th>
                    <th class="px-4 py-3">Typ</th>
                    <th class="px-4 py-3 rounded-tr-lg">Opis</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-800">
                  <tr
                    v-for="item in items"
                    :key="item.id"
                    class="hover:bg-slate-800/50 transition-colors"
                  >
                    <td class="px-4 py-3 font-medium text-slate-100">{{ item.name }}</td>
                    <td class="px-4 py-3 text-yellow-500">{{ item.type }}</td>
                    <td class="px-4 py-3 text-slate-400">{{ item.description }}</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div v-if="activeTab === 'npcs'">
              <div v-if="npcs.length === 0" class="text-slate-500 text-center py-8">Brak NPC.</div>
              <table v-else class="w-full text-left text-sm text-slate-300">
                <thead class="text-xs uppercase bg-slate-950 text-slate-500 sticky top-0">
                  <tr>
                    <th class="px-4 py-3 rounded-tl-lg">Imie</th>
                    <th class="px-4 py-3">Rasa</th>
                    <th class="px-4 py-3">Klasa</th>
                    <th class="px-4 py-3 rounded-tr-lg">Opis</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-800">
                  <tr
                    v-for="npc in npcs"
                    :key="npc.id"
                    class="hover:bg-slate-800/50 transition-colors group"
                  >
                    <td class="px-4 py-3 font-medium text-slate-100">{{ npc.name }}</td>
                    <td class="px-4 py-3 text-blue-400">{{ npc.race }}</td>
                    <td class="px-4 py-3 text-purple-400">{{ npc.class }} (Lvl {{ npc.level }})</td>
                    <td class="px-4 py-3 text-slate-400">
                      {{ npc.description }}
                      <div
                        class="hidden group-hover:block absolute bg-slate-900 border border-slate-700 p-2 rounded shadow-xl mt-1 z-10 text-xs"
                      >
                        STR: {{ npc.strength }} | DEX: {{ npc.dexterity }} | INT:
                        {{ npc.intelligence }}
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
