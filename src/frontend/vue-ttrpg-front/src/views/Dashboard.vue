<template>
  <div class="min-h-screen bg-slate-950 text-slate-100">
    <div class="mx-auto max-w-6xl px-4 py-8 space-y-8">
      <!-- Header -->
      <header class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div class="space-y-1">
          <h1 class="text-2xl sm:text-3xl font-semibold tracking-tight">
            Dashboard
          </h1>
          <p class="text-slate-300">
            Wybierz tryb pracy — możesz być graczem i mistrzem gry w dowolnym momencie.
          </p>
        </div>
      </header>

      <!-- Tryb: wybór -->
      <section class="grid grid-cols-1 gap-4 lg:grid-cols-2">
        <!-- Gracz -->
        <button
          type="button"
          class="group text-left rounded-3xl border border-slate-800 bg-gradient-to-br from-slate-900/60 to-slate-900/20 p-6 hover:border-emerald-400/60 hover:shadow-[0_0_0_1px_rgba(16,185,129,0.25)] transition"
          @click="goPlayer()"
        >
          <div class="flex items-start justify-between gap-4">
            <div class="space-y-2">
              <div class="inline-flex items-center gap-2 rounded-full border border-emerald-400/30 bg-emerald-400/10 px-3 py-1 text-xs text-emerald-200">
                🎲 Tryb Gracza
              </div>
              <h2 class="text-xl font-semibold">Gram jako gracz</h2>
              <p class="text-slate-300">
                Dostęp do postaci, sesji, zaproszeń i notatek gracza.
              </p>
            </div>

            <div class="shrink-0 whitespace-nowrap rounded-2xl border border-slate-800 bg-slate-950/50 px-3 py-2 text-sm text-slate-300 group-hover:border-violet-400/40 transition">
              Wybierz →
            </div>
          </div>

          <div class="mt-5 grid grid-cols-1 gap-2 sm:grid-cols-2">
            <div class="rounded-2xl border border-slate-800 bg-slate-950/30 px-4 py-3 text-sm text-slate-300">
              ✅ Moje postacie
            </div>
            <div class="rounded-2xl border border-slate-800 bg-slate-950/30 px-4 py-3 text-sm text-slate-300">
              ✅ Sesje, w których gram
            </div>
            <div class="rounded-2xl border border-slate-800 bg-slate-950/30 px-4 py-3 text-sm text-slate-300">
              ✅ Zaproszenia
            </div>
            <div class="rounded-2xl border border-slate-800 bg-slate-950/30 px-4 py-3 text-sm text-slate-300">
              ✅ Notatki gracza
            </div>
          </div>
        </button>

        <!-- Mistrz Gry -->
        <button
          type="button"
          class="group text-left rounded-3xl border border-slate-800 bg-gradient-to-br from-slate-900/60 to-slate-900/20 p-6 hover:border-violet-400/60 hover:shadow-[0_0_0_1px_rgba(167,139,250,0.22)] transition"
          @click="goGm()"
        >
          <div class="flex items-start justify-between gap-4">
            <div class="space-y-2">
              <div class="inline-flex items-center gap-2 rounded-full border border-violet-400/30 bg-violet-400/10 px-3 py-1 text-xs text-violet-200">
                📜 Tryb Mistrza Gry
              </div>
              <h2 class="text-xl font-semibold">Prowadzę jako mistrz gry</h2>
              <p class="text-slate-300">
                Sesje, kampanie, zasoby fabularne, NPC i zarządzanie drużyną.
              </p>
            </div>

            <div class="shrink-0 whitespace-nowrap rounded-2xl border border-slate-800 bg-slate-950/50 px-3 py-2 text-sm text-slate-300 group-hover:border-violet-400/40 transition">
              Wybierz →
            </div>
          </div>

          <div class="mt-5 grid grid-cols-1 gap-2 sm:grid-cols-2">
            <div class="rounded-2xl border border-slate-800 bg-slate-950/30 px-4 py-3 text-sm text-slate-300">
              ✅ Moje sesje (MG)
            </div>
            <div class="rounded-2xl border border-slate-800 bg-slate-950/30 px-4 py-3 text-sm text-slate-300">
              ✅ Kampanie i scenariusze
            </div>
            <div class="rounded-2xl border border-slate-800 bg-slate-950/30 px-4 py-3 text-sm text-slate-300">
              ✅ NPC / Lokacje / Przedmioty
            </div>
            <div class="rounded-2xl border border-slate-800 bg-slate-950/30 px-4 py-3 text-sm text-slate-300">
              ✅ Drużyny i uczestnicy
            </div>
          </div>
        </button>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

type DashboardMode = 'player' | 'gm'

const router = useRouter()
const route = useRoute()
const auth = useAuthStore()

function goPlayer() {
  localStorage.setItem('dashboard_mode', 'player')
  router.push({ name: 'PlayerZone' })
}

function goGm() {
  localStorage.setItem('dashboard_mode', 'gm')
  router.push({ name: 'GmZone' })
}
</script>
