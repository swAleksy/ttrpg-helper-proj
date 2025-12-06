<script setup lang="ts">
import heroImage from '@/assets/img/landing-bg.jpg'
</script>

<template>
  <section
    class="relative flex min-h-[calc(100vh-4rem)] items-center justify-center overflow-hidden"
  >
    <!-- Tło -->
    <div
      class="pointer-events-none absolute inset-0 bg-cover bg-center opacity-30"
      :style="{ backgroundImage: `url(${heroImage})` }"
    ></div>
    <div
      class="absolute inset-0 bg-gradient-to-b from-slate-950/70 via-slate-950/80 to-slate-950"
    ></div>

    <!-- Treść -->
    <div class="relative z-10 mx-auto flex max-w-4xl flex-col gap-6 px-4 text-center">
      <p class="text-xs uppercase tracking-[0.2em] text-emerald-300/80">
        aplikacja webowa wspomagająca prowadzenie sesji gier ttrpg
      </p>

      <h1 class="text-balance text-3xl font-bold tracking-tight sm:text-4xl md:text-5xl">
        Ogarnij swoje kampanie, postacie i sesje
        <span class="text-emerald-400">w jednym miejscu</span>.
      </h1>

      <p class="mx-auto max-w-2xl text-sm text-slate-300 md:text-base">
        Twórz postacie, planuj sesje, zapisuj notatki, rzucaj kośćmi i dziel się zasobami z
        graczami. Zaprojektowane z myślą o TTRPG: D&amp;D, Warhammer, Call of Cthulhu i nie tylko.
      </p>

      <div class="mt-2 flex flex-col items-center justify-center gap-3 sm:flex-row sm:gap-4">
        <RouterLink
          to="/login"
          class="inline-flex items-center justify-center rounded-2xl border border-slate-700 px-6 py-3 text-sm font-medium text-slate-100 transition hover:border-emerald-400 hover:text-emerald-300"
        >
          Zaloguj się
        </RouterLink>

        <RouterLink
          to="/register"
          class="inline-flex items-center justify-center rounded-2xl bg-emerald-500 px-6 py-3 text-sm font-semibold text-slate-950 shadow-lg shadow-emerald-500/30 transition hover:bg-emerald-400"
        >
          Zarejestruj się
        </RouterLink>
      </div>

      <div class="mt-4 flex flex-col items-center gap-2 text-xs text-slate-400 md:text-sm">
        <span>Zarówno dla Mistrzów Gry, jak i graczy.</span>
        <span class="text-slate-500">
          Zawiera moduł postaci, sesji, kompendium, komunikacji i kalendarza.
        </span>
      </div>
    </div>
  </section>
</template>
