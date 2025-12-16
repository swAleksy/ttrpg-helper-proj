<script setup lang="ts">
</script>

<template>
  <section class="mx-auto flex min-h-[calc(100vh-4rem)] max-w-4xl flex-col gap-4 px-4 py-8">
    <h1 class="text-2xl font-semibold">
      Twój panel
    </h1>
    <p class="text-sm text-slate-300">
      Lista Twoich postaci, sesji i ostatnich aktywności.
    </p>

    <div class="mt-4 grid gap-4 md:grid-cols-3">
      <div class="rounded-2xl border border-slate-800 bg-slate-900/60 p-4 text-sm">
        <h2 class="text-sm font-semibold">Postacie</h2>
        <p class="mt-1 text-xs text-slate-400">
          Szybki dostęp do kart postaci.
        </p>
      </div>
      <div class="rounded-2xl border border-slate-800 bg-slate-900/60 p-4 text-sm">
        <h2 class="text-sm font-semibold">Sesje</h2>
        <p class="mt-1 text-xs text-slate-400">
          Zaplanowane i archiwalne sesje w jednym miejscu.
        </p>
      </div>
      <div class="rounded-2xl border border-slate-800 bg-slate-900/60 p-4 text-sm">
        <h2 class="text-sm font-semibold">Notatki &amp; kompendium</h2>
        <p class="mt-1 text-xs text-slate-400">
          Skróty do Twoich zasobów fabularnych.
        </p>
      </div>
    </div>
  </section>
</template>
