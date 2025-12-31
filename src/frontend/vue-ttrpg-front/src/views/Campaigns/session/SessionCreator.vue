<script setup lang="ts">
import { ref } from 'vue' // dodano computed
import { useRouter, useRoute } from 'vue-router' // dodano useRoute
import { useCampaignStore } from '@/stores/CampaignStore'

// Definiujemy props, ale nie polegamy tylko na nim
const props = defineProps<{ campaignId: string | number }>()

const router = useRouter()
const route = useRoute() // Dostęp do aktualnej ścieżki
const campaignStore = useCampaignStore()

const name = ref('')
const description = ref('')
const scheduledDate = ref('')

const handleSubmit = async () => {
  // FIX: Sprawdzamy props ORAZ route.params jako zabezpieczenie
  const rawId = props.campaignId || route.params.campaignId
  const id = Number(rawId)

  if (!id || isNaN(id)) {
    console.error('Błąd ID:', rawId)
    alert('Błąd: Nieprawidłowe ID kampanii!')
    return
  }

  const success = await campaignStore.createSession(
    name.value,
    description.value,
    scheduledDate.value,
    id,
  )

  if (success) {
    router.push(`/campaigns/${id}`)
  }
}
</script>

<template>
  <div class="min-h-screen bg-slate-950 text-slate-100 p-6 md:p-10">
    <div class="max-w-xl mx-auto">
      <h1 class="text-3xl font-bold mb-2">Utwórz nową sesję</h1>
      <p class="text-slate-400 mb-8">Zaplanuj kolejne spotkanie swojej drużyny.</p>

      <form @submit.prevent="handleSubmit" class="space-y-6">
        <div>
          <label class="block text-sm font-medium text-slate-400 mb-2 ml-1">Nazwa sesji</label>
          <input
            v-model="name"
            type="text"
            placeholder="np. Bitwa o Cytadelę"
            class="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 focus:ring-2 focus:ring-emerald-500 outline-none transition-all"
            required
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-400 mb-2 ml-1"
            >Opis (opcjonalnie)</label
          >
          <textarea
            v-model="description"
            rows="4"
            placeholder="Co wydarzy się w tym rozdziale?"
            class="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 focus:ring-2 focus:ring-emerald-500 outline-none transition-all"
          ></textarea>
        </div>

        <div>
          <label class="block text-sm font-medium text-slate-400 mb-2 ml-1">Data sesji</label>
          <input
            v-model="scheduledDate"
            type="datetime-local"
            class="w-full bg-slate-900 border border-slate-700 rounded-xl px-4 py-3 focus:ring-2 focus:ring-emerald-500 outline-none transition-all text-slate-100"
            required
          />
        </div>

        <div class="flex gap-3 pt-2">
          <button
            type="submit"
            class="flex-1 bg-emerald-500 hover:bg-emerald-400 text-slate-950 font-bold py-3 rounded-xl transition-colors shadow-lg shadow-emerald-900/20"
          >
            Stwórz Sesję
          </button>

          <button
            type="button"
            @click="$router.back()"
            class="px-6 py-3 border border-slate-700 text-slate-400 hover:bg-slate-900 hover:text-white rounded-xl transition-colors"
          >
            Anuluj
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
