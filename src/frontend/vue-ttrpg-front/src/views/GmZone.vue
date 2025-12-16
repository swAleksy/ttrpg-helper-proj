<template>
  <div class="min-h-screen bg-slate-950 text-slate-100">
    <div class="mx-auto max-w-6xl px-4 py-8 space-y-6">
      <header class="space-y-1">
        <h1 class="text-3xl font-bold tracking-tight">📜 Strefa Mistrza Gry</h1>
        <p class="text-sm text-slate-400">
          Tutaj będą: sesje jako MG, kampanie, NPC, zasoby fabularne i drużyny.
        </p>
      </header>

      <div class="rounded-2xl border border-slate-800 bg-slate-900/30 p-6">
        <p class="text-slate-300">
          Tu będą kolejne moduły.
        </p>

        <div class="mt-4">
          <router-link
            to="/dashboard"
            class="inline-flex items-center gap-2 rounded-xl px-3 py-2 border border-slate-700 hover:bg-slate-800 transition"
          >
            ← Wróć do dashboardu
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>