<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useCampaignStore } from '@/stores/CampaignStore'

const router = useRouter()
const campaignStore = useCampaignStore()

const gmCampaigns = computed(() => campaignStore.gmCampaigns)
const playerCampaigns = computed(() => campaignStore.getUniquePlayerCampaigns)

onMounted(() => {
  campaignStore.fetchGmCampaigns()
  campaignStore.fetchPlayerCampaigns()
})

const handleCreateCampaign = () => {
  router.push('/campaigns/new')
}

const openCampaign = (id: number) => {
  router.push(`/campaigns/${id}`)
}

const handleDeleteCampaign = async (id: number) => {
  if (confirm('Czy na pewno chcesz usunąć tę kampanię?')) {
    await campaignStore.deleteCampaign(id)
  }
}
</script>

<template>
  <div class="min-h-screen bg-slate-950 text-slate-100">
    <div class="mx-auto max-w-6xl px-4 py-8 space-y-8">
      <header class="space-y-2">
        <h1 class="text-3xl font-bold tracking-tight text-slate-100">
          Twoje Kampanie
        </h1>
        <p class="text-slate-400">
          Zarządzaj kampaniami, które prowadzisz, oraz tymi, w których grasz.
        </p>
      </header>

      <div v-if="campaignStore.isLoading" class="flex justify-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-emerald-400"></div>
      </div>

      <div v-else class="space-y-12">
        <!-- GM -->
        <section>
          <h2 class="text-2xl font-semibold text-emerald-400 mb-4 flex items-center gap-2">
            <svg
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
                d="M19.428 15.428a2 2 0 00-1.022-.547l-2.384-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z"
              />
            </svg>
            Mistrz Gry
          </h2>

          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            <button
              @click="handleCreateCampaign"
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
                Utwórz nową kampanię
              </span>
            </button>

            <div
              v-for="campaign in gmCampaigns"
              :key="campaign.id"
              @click="openCampaign(campaign.id)"
              class="group relative flex flex-col justify-between h-64 p-6 bg-slate-900/40 backdrop-blur-sm border border-slate-800 rounded-2xl cursor-pointer transition-all duration-300 hover:border-emerald-500/30 hover:shadow-[0_0_20px_-5px_rgba(16,185,129,0.15)] hover:-translate-y-1"
            >
              <div>
                <div class="flex justify-between items-start mb-4">
                  <div
                    class="w-10 h-10 rounded-xl bg-slate-800 flex items-center justify-center border border-slate-700 text-emerald-400"
                  >
                    <span class="font-bold text-lg">{{ campaign.name.charAt(0).toUpperCase() }}</span>
                  </div>
                  <button
                    @click.stop="handleDeleteCampaign(campaign.id)"
                    class="text-slate-500 hover:text-red-500 transition-colors p-1"
                    title="Usuń kampanię"
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
                  class="text-xl font-bold text-slate-100 group-hover:text-emerald-400 transition-colors line-clamp-1"
                >
                  {{ campaign.name }}
                </h3>
                <p v-if="campaign.description" class="text-sm text-slate-400 mt-2 line-clamp-3">
                  {{ campaign.description }}
                </p>
              </div>
              <div class="mt-4 pt-4 border-t border-slate-800 flex items-center justify-between">
                <span class="text-xs text-slate-500 font-mono">ROLE: GAME MASTER</span>
              </div>
            </div>
          </div>
        </section>

        <!-- Player -->
        <section v-if="playerCampaigns.length > 0">
          <h2
            class="text-2xl font-semibold text-blue-400 mb-4 flex items-center gap-2 border-t border-slate-800 pt-8"
          >
            <svg
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
                d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0z"
              />
            </svg>
            Uczestnik (Gracz)
          </h2>

          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            <div
              v-for="campaign in playerCampaigns"
              :key="campaign.id"
              @click="openCampaign(campaign.id)"
              class="group relative flex flex-col justify-between h-64 p-6 bg-slate-900/20 border border-slate-800 rounded-2xl cursor-pointer transition-all duration-300 hover:border-blue-500/30 hover:bg-slate-900/40 hover:-translate-y-1"
            >
              <div>
                <div class="flex justify-between items-start mb-4">
                  <div
                    class="w-10 h-10 rounded-xl bg-slate-800 flex items-center justify-center border border-slate-700 text-blue-400"
                  >
                    <span class="font-bold text-lg">{{ campaign.name.charAt(0).toUpperCase() }}</span>
                  </div>
                </div>
                <h3
                  class="text-xl font-bold text-slate-100 group-hover:text-blue-400 transition-colors line-clamp-1"
                >
                  {{ campaign.name }}
                </h3>
                <p v-if="campaign.gameMasterName" class="text-sm text-slate-500 mt-2">
                  GM: <span class="text-slate-300">{{ campaign.gameMasterName }}</span>
                </p>
                <p v-if="campaign.description" class="text-sm text-slate-400 mt-2 line-clamp-2">
                  {{ campaign.description }}
                </p>
              </div>
              <div class="mt-4 pt-4 border-t border-slate-800 flex items-center justify-between">
                <span class="text-xs text-slate-500 font-mono">ROLE: PLAYER</span>
              </div>
            </div>
          </div>
        </section>
      </div>
    </div>
  </div>
</template>
