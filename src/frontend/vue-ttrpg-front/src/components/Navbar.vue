<!-- <script setup>
import logo from '@/assets/img/logoxD.png'
import { useRoute } from 'vue-router'

const route = useRoute()

const isActiveLink = (routePath) => {
  return route.path === routePath
}
</script>

<template>
  <nav class="bg-green-700 border-b border-green-500">
    <div class="mx-auto max-w-7xl px-2 sm:px-6 lg:px-8">
      <div class="flex h-20 items-center justify-between">
        <div class="flex flex-1 items-center justify-center md:items-stretch md:justify-start">

          <RouterLink class="flex shrink-0 items-center mr-4" to="/">
            <img class="h-10 w-auto" :src="logo" alt="TTRPG" />
            <span class="hidden md:block text-white text-2xl font-bold ml-2">TTRPG</span>
          </RouterLink>
          <div class="md:ml-auto">
            <div class="flex space-x-2">
              <RouterLink
                to="/"
                :class="[
                  isActiveLink('/') ? 'bg-green-900' : 'hover:bg-gray-900 hover:text-white',
                  'text-white',
                  'px-3',
                  'py-2',
                  'rounded-md',
                ]"
                >Home</RouterLink
              >
              <RouterLink
                to="/jobs"
                :class="[
                  isActiveLink('/jobs') ? 'bg-green-900' : 'hover:bg-gray-900 hover:text-white',
                  'text-white',
                  'px-3',
                  'py-2',
                  'rounded-md',
                ]"
                >Jobs</RouterLink
              >
              <RouterLink
                to="/jobs/add"
                :class="[
                  isActiveLink('/jobs/add') ? 'bg-green-900' : 'hover:bg-gray-900 hover:text-white',
                  'text-white',
                  'px-3',
                  'py-2',
                  'rounded-md',
                ]"
                >Add Job</RouterLink
              >
            </div>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template> -->

<script setup lang="ts">
import { ref } from 'vue'
import logo from '@/assets/img/logo.png'

const isOpen = ref(false)

const navLinks = [
  { to: '/', label: 'Start' },
  { to: '/dashboard', label: 'Panel' },
]
</script>

<template>
  <header
    class="sticky top-0 z-20 border-b border-slate-800 bg-slate-950/80 backdrop-blur"
  >
    <nav class="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
      <!-- Logo + nazwa -->
      <RouterLink to="/" class="flex items-center gap-2">
        <img :src="logo" alt="TTRPG Helper logo" class="h-8 w-8 rounded-lg" />
        <span class="font-semibold tracking-wide">TTRPG Helper</span>
      </RouterLink>

      <!-- Desktop -->
      <div class="hidden items-center gap-6 md:flex">
        <div class="flex items-center gap-4 text-sm">
          <RouterLink
            v-for="link in navLinks"
            :key="link.to"
            :to="link.to"
            class="transition hover:text-emerald-300"
          >
            {{ link.label }}
          </RouterLink>
        </div>

        <div class="flex items-center gap-2">
          <RouterLink
            to="/login"
            class="rounded-xl px-4 py-2 text-sm font-medium transition hover:bg-slate-800"
          >
            Zaloguj się
          </RouterLink>
          <RouterLink
            to="/register"
            class="rounded-xl bg-emerald-500 px-4 py-2 text-sm font-semibold text-slate-950 shadow transition hover:bg-emerald-400"
          >
            Zarejestruj się
          </RouterLink>
        </div>
      </div>

      <!-- Mobile burger -->
      <button
        class="inline-flex items-center justify-center rounded-md border border-slate-700 p-2 md:hidden"
        @click="isOpen = !isOpen"
        aria-label="Otwórz menu"
      >
        <span class="i-[heroicons-outline-bars-3] h-5 w-5" v-if="!isOpen">☰</span>
        <span v-else class="i-[heroicons-outline-x-mark] h-5 w-5">✕</span>
      </button>
    </nav>

    <!-- Mobile menu -->
    <div v-if="isOpen" class="border-t border-slate-800 bg-slate-950 md:hidden">
      <div class="mx-auto flex max-w-6xl flex-col gap-2 px-4 py-3 text-sm">
        <RouterLink
          v-for="link in navLinks"
          :key="link.to"
          :to="link.to"
          class="rounded-lg px-2 py-2 transition hover:bg-slate-800"
          @click="isOpen = false"
        >
          {{ link.label }}
        </RouterLink>

        <div class="mt-2 flex flex-col gap-2">
          <RouterLink
            to="/login"
            class="rounded-lg px-3 py-2 text-center transition hover:bg-slate-800"
            @click="isOpen = false"
          >
            Zaloguj się
          </RouterLink>
          <RouterLink
            to="/register"
            class="rounded-lg bg-emerald-500 px-3 py-2 text-center font-semibold text-slate-950 shadow transition hover:bg-emerald-400"
            @click="isOpen = false"
          >
            Zarejestruj się
          </RouterLink>
        </div>
      </div>
    </div>
  </header>
</template>
