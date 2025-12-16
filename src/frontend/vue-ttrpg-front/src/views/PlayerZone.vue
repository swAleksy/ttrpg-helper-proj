<template>
  <div class="min-h-screen bg-slate-950 text-slate-100">
    <div class="mx-auto max-w-6xl px-4 py-8 space-y-6">
      <header class="space-y-1">
        <h1 class="text-3xl font-bold tracking-tight">🎲 Strefa Gracza</h1>
        <p class="text-sm text-slate-400">
          Tutaj będą: moje postacie, sesje, zaproszenia i notatki gracza.
        </p>
      </header>

      <div class="rounded-2xl border border-slate-800 bg-slate-900/30 p-6">
        <!-- <p class="text-slate-300">
          Tu będą kolejne moduły.
        </p> -->

        <router-link
          to="/characters/new"
          class="group inline-flex items-center gap-4 rounded-2xl border border-emerald-500/40 bg-emerald-500/10 px-6 py-4 text-lg font-semibold text-emerald-100 hover:bg-emerald-500/20 hover:border-emerald-400 transition"
        >
          <span
            class="flex h-10 w-10 items-center justify-center rounded-full bg-emerald-500 text-slate-950 text-2xl font-bold group-hover:scale-105 transition"
          >
            +
          </span>

          <span>
            Dodaj nową postać
          </span>
        </router-link>


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