<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useCampaignStore } from '@/stores/CampaignStore'

const router = useRouter()
const campaignStore = useCampaignStore()

const name = ref('')
const description = ref('')
const error = ref<string | null>(null)

const submit = async () => {
  error.value = null

  if (!name.value.trim()) {
    error.value = 'Campaign name is required'
    return
  }

  const success = await campaignStore.createCampaign(name.value, description.value)

  if (success) {
    router.push('/campaigns')
  } else {
    error.value = 'Failed to create campaign'
  }
}
</script>

<template>
  <div class="min-h-screen bg-slate-950 text-slate-100 p-6 md:p-10">
    <div class="max-w-xl mx-auto">
      <h1 class="text-3xl font-bold mb-2">Stwórz nową kampanię</h1>
      <p class="text-slate-400 mb-8">Zaplanuj swoją następną przygodę.</p>

      <div class="space-y-6">
        <input
          v-model="name"
          placeholder="Nazwa kampanii"
          class="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 focus:ring-2 focus:ring-emerald-500 outline-none"
        />

        <textarea
          v-model="description"
          placeholder="Opis (opcjonalny)"
          rows="4"
          class="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 focus:ring-2 focus:ring-emerald-500 outline-none"
        />

        <p v-if="error" class="text-red-400 text-sm">
          {{ error }}
        </p>

        <div class="flex gap-3">
          <button
            @click="submit"
            :disabled="campaignStore.isLoading"
            class="flex-1 bg-emerald-500 hover:bg-emerald-400 disabled:opacity-50 text-slate-950 font-bold py-3 rounded-xl transition-colors"
          >
            {{ campaignStore.isLoading ? 'Przygotowywanie...' : 'Stwórz Kampanię' }}
          </button>

          <button
            @click="router.back()"
            class="px-4 py-3 border border-slate-700 text-slate-400 hover:bg-slate-800 rounded-xl"
          >
            Anuluj
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
