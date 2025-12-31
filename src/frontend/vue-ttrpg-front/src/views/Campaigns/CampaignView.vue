<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useCampaignStore } from '@/stores/CampaignStore'
import { useAuthStore } from '@/stores/auth' // Potrzebne do sprawdzenia czy jesteś GMem
import { useRouter } from 'vue-router'

const props = defineProps<{
  id: string | number
}>()

const router = useRouter()
const campaignStore = useCampaignStore()
const authStore = useAuthStore()

// 1. Pobieramy kampanię z gettera
const campaign = computed(() => {
  return campaignStore.getCampaignById(Number(props.id))
})

// 2. Sprawdzamy czy aktualny user jest Mistrzem Gry tej kampanii
const isGameMaster = computed(() => {
  if (!campaign.value || !authStore.user) return false
  return campaign.value.gameMasterId === authStore.user.id
})

// 3. Formatowanie daty (pomocnicza funkcja)
const formatDate = (dateString: string) => {
  if (!dateString) return 'TBA'
  return new Date(dateString).toLocaleDateString('pl-PL', {
    day: 'numeric',
    month: 'long',
    hour: '2-digit',
    minute: '2-digit',
  })
}

// 4. Akcje
const openSession = (sessionId: number) => {
  router.push(`/session/${sessionId}`)
}

const handleCreateSession = () => {
  router.push({
    name: 'SessionCreate',
    params: { campaignId: String(props.id) },
  })
}

const handleDeleteSession = async (sessionId: number) => {
  if (confirm('Czy na pewno chcesz usunąć tę sesję?')) {
    // Tu musisz dodać akcję w store, np. campaignStore.deleteSession(sessionId)
    console.log('Delete session', sessionId)
  }
}

onMounted(async () => {
  // Jeśli nie ma kampanii w cache LUB (ważne!) jeśli w cache nie ma załadowanych sesji
  if (!campaign.value || !campaign.value.sessions) {
    try {
      await campaignStore.fetchCampaignGmById(Number(props.id))
    } catch {
      // jeśli nie GM → spróbuj PLAYER
      await campaignStore.fetchCampaignPlayerById(Number(props.id))
    }
  }
})
</script>

<template>
  <div class="min-h-screen bg-slate-950 text-slate-100 p-6 md:p-10">
    <div v-if="campaignStore.isLoading && !campaign" class="flex justify-center py-20">
      <div
        class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-emerald-400"
      ></div>
    </div>

    <div v-else-if="campaign">
      <header
        class="mb-8 border-b border-slate-800 pb-6 flex flex-col md:flex-row justify-between items-start md:items-center gap-4"
      >
        <div class="flex items-center gap-4">
          <div
            class="w-16 h-16 rounded-2xl bg-slate-800 flex items-center justify-center border border-slate-700 text-emerald-400 text-2xl font-bold"
          >
            {{ campaign.name.charAt(0).toUpperCase() }}
          </div>
          <div>
            <h1 class="text-3xl font-bold tracking-tight text-slate-100">{{ campaign.name }}</h1>
            <p class="text-slate-400 mt-1">
              Mistrz Gry:
              <span class="text-emerald-400 font-medium">{{
                campaign.gameMasterName || 'Nieznany'
              }}</span>
            </p>
          </div>
        </div>

        <button
          v-if="isGameMaster"
          @click="handleCreateSession"
          class="px-4 py-2 bg-emerald-600 hover:bg-emerald-500 text-white rounded-lg transition-colors flex items-center gap-2 font-medium"
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
          Nowa Sesja
        </button>
      </header>

      <section class="bg-slate-900/40 p-6 rounded-2xl border border-slate-800 mb-10">
        <h2 class="text-xl font-bold mb-4 text-emerald-400">Opis Kampanii</h2>
        <p class="text-slate-300 leading-relaxed whitespace-pre-wrap">
          {{ campaign.description || 'Brak opisu.' }}
        </p>
      </section>

      <h2 class="text-2xl font-bold mb-6 flex items-center gap-2">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-7 w-7 text-slate-400"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            stroke-width="2"
            d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
          />
        </svg>
        Lista Sesji
      </h2>

      <div
        v-if="campaign.sessions && campaign.sessions.length > 0"
        class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6"
      >
        <button
          @click="handleCreateSession"
          class="group relative flex flex-col items-center justify-center h-64 bg-slate-950/80 border-2 border-dashed border-slate-700 rounded-2xl transition-all duration-300 hover:border-emerald-500/50 hover:bg-emerald-500/5"
        >
          <div
            class="p-4 rounded-full bg-slate-900 group-hover:bg-emerald-500/20 transition-colors duration-300"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="h-8 w-8 text-slate-400 group-hover:text-emerald-400 transition-colors"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M12 4v16m8-8H4"
              />
            </svg>
          </div>
          <span
            class="mt-4 font-bold text-slate-400 group-hover:text-emerald-400 tracking-tight transition-colors"
          >
            Utwórz nową Sesję
          </span>
        </button>
        <div
          v-for="session in campaign.sessions"
          :key="session.id"
          @click="openSession(session.id)"
          class="group relative flex flex-col justify-between h-64 p-6 bg-slate-900/40 backdrop-blur-sm border border-slate-800 rounded-2xl cursor-pointer transition-all duration-300 hover:border-emerald-500/30 hover:shadow-[0_0_20px_-5px_rgba(16,185,129,0.15)] hover:-translate-y-1"
        >
          <div>
            <div class="flex justify-between items-start mb-4">
              <!-- <div
                class="px-3 py-1 rounded-lg bg-slate-800 border border-slate-700 text-emerald-400 text-xs font-mono font-bold uppercase tracking-wider"
              >
                {{ session.status || 'PLANOWANA' }}
              </div> -->

              <button
                v-if="isGameMaster"
                @click.stop="handleDeleteSession(session.id)"
                class="text-slate-500 hover:text-red-500 transition-colors"
                title="Usuń sesję"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="h-5 w-5"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor"
                >
                  <path
                    stroke-linecap="round"
                    stroke-linejoin="round"
                    stroke-width="2"
                    d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                  />
                </svg>
              </button>
            </div>

            <h3
              class="text-xl font-bold text-slate-100 group-hover:text-emerald-400 transition-colors mb-2 line-clamp-2"
            >
              {{ session.name }}
            </h3>

            <div class="flex items-center text-slate-400 text-sm mb-3">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                class="h-4 w-4 mr-1"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  stroke-width="2"
                  d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
              {{ formatDate(session.scheduledDate) }}
            </div>

            <p v-if="session.description" class="text-sm text-slate-500 line-clamp-2">
              {{ session.description }}
            </p>
          </div>

          <div class="mt-4 pt-4 border-t border-slate-800 flex items-center justify-between">
            <div class="flex -space-x-2 overflow-hidden">
              <div v-if="session.players.length > 0">
                Liczba graczy: {{ session.players.length }}
              </div>
            </div>
            <span class="text-xs text-slate-500">Otwórz &rarr;</span>
          </div>
        </div>
      </div>

      <div
        v-else
        class="text-center py-12 border-2 border-dashed border-slate-800 rounded-2xl bg-slate-900/20"
      >
        <h3 class="text-lg font-medium text-slate-400">Brak zaplanowanych sesji</h3>
        <p class="text-slate-500 text-sm mt-1">Ta kampania nie ma jeszcze żadnych sesji.</p>
        <button
          v-if="isGameMaster"
          @click="handleCreateSession"
          class="mt-4 text-emerald-400 hover:underline text-sm font-medium"
        >
          Zaplanuj pierwszą sesję
        </button>
      </div>
    </div>
  </div>
</template>
