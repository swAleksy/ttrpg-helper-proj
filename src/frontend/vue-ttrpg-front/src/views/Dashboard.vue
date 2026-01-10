<template>
  <div class="min-h-screen bg-slate-950 text-slate-100">
    <div class="mx-auto max-w-6xl px-4 py-8 space-y-6">
      <header class="flex flex-col gap-2 sm:flex-row sm:items-end sm:justify-between">
        <div class="space-y-1">
          <h1 class="text-3xl font-bold tracking-tight">Dashboard</h1>
          <p class="text-sm text-slate-400">Szybki dostęp do modułów.</p>
        </div>
      </header>

      <div class="grid grid-cols-1 gap-4 sm:gap-6 lg:grid-cols-2">
        <router-link
          :to="CHARACTERS_PATH"
          class="group rounded-3xl border border-slate-800 bg-slate-900/20 p-7 transition hover:border-slate-700 hover:bg-slate-900/30 focus:outline-none focus-visible:ring-2 focus-visible:ring-slate-300/40"
        >
          <div class="min-w-0">
            <div class="flex items-start gap-3">
              <div class="flex h-7 shrink-0 items-center sm:h-8">
                <span class="inline-block h-2 w-2 rounded-full bg-emerald-400/70" />
              </div>

              <div class="min-w-0">
                <h2 class="text-xl sm:text-2xl font-semibold tracking-tight truncate">Postacie</h2>
                <p class="mt-3 text-sm text-slate-400 leading-relaxed min-h-[40px]">
                  Zarządzaj postaciami: lista, podgląd i edycja.
                </p>
              </div>
            </div>
          </div>

          <div class="mt-7 h-px w-full bg-slate-800/70" />

          <div class="mt-4 flex items-center justify-between gap-3 text-xs text-slate-500">
            <span class="shrink-0">Moduł</span>
            <span
              class="min-w-0 truncate font-medium tracking-wide text-slate-400 tabular-nums"
              title="CHARACTERS"
            >
              CHARACTERS
            </span>
          </div>
        </router-link>

        <router-link
          :to="CAMPAIGNS_PATH"
          class="group rounded-3xl border border-slate-800 bg-slate-900/20 p-7 transition hover:border-slate-700 hover:bg-slate-900/30 focus:outline-none focus-visible:ring-2 focus-visible:ring-slate-300/40"
        >
          <div class="min-w-0">
            <div class="flex items-start gap-3">
              <div class="flex h-7 shrink-0 items-center sm:h-8">
                <span class="inline-block h-2 w-2 rounded-full bg-violet-400/70" />
              </div>

              <div class="min-w-0">
                <h2 class="text-xl sm:text-2xl font-semibold tracking-tight truncate">
                  Kampanie i Sesje
                </h2>
                <p class="mt-3 text-sm text-slate-400 leading-relaxed min-h-[40px]">
                  Dostęp do kampanii, sesji i powiązanych informacji.
                </p>
              </div>
            </div>
          </div>

          <div class="mt-7 h-px w-full bg-slate-800/70" />

          <div class="mt-4 flex items-center justify-between gap-3 text-xs text-slate-500">
            <span class="shrink-0">Moduł</span>
            <span
              class="min-w-0 truncate font-medium tracking-wide text-slate-400 tabular-nums"
              title="CAMPAIGNS"
            >
              CAMPAIGNS
            </span>
          </div>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const CHARACTERS_PATH = '/characters'
const CAMPAIGNS_PATH = '/campaigns'
</script>
