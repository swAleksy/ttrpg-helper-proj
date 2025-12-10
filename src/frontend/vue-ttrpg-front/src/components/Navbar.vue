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
import { ref, computed } from 'vue'
import logo from '@/assets/img/logo.png'
import { useRouter } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth'
import FriendsDropdown from '@/components/FriendsDropdown.vue'

const router = useRouter()
const auth = useAuthStore()
const { isAuthenticated, username, userAvatarUrl } = storeToRefs(auth)

const isOpen = ref(false)
const showFriends = ref(false)

const toggleFriends = () => {
  showFriends.value = !showFriends.value
}

// Używamy computed, żeby lista linków zmieniała się dynamicznie
const navLinks = computed(() => {
  // Linki widoczne zawsze (lub tylko dla niezalogowanych)
  const links = []

  if (!isAuthenticated.value) {
    // Jeśli NIE jest zalogowany, pokaż Start
    links.push({ to: '/', label: 'Start' })
  } else {
    // Jeśli JEST zalogowany, pokaż Panel (Dashboard)
    // Możesz tu też zostawić Start, jeśli chcesz, żeby zalogowany też go widział
    links.push({ to: '/dashboard', label: 'Panel' })
    // Np. links.push({ to: '/jobs', label: 'Zlecenia' })
  }

  return links
})

const handleLogout = () => {
  auth.logout()
  isOpen.value = false
  router.push('/')
}
</script>

<template>
  <header class="sticky top-0 z-20 border-b border-slate-800 bg-slate-950/80 backdrop-blur">
    <nav class="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
      <!-- Logo + nazwa -->
      <RouterLink to="/" class="flex items-center gap-2">
        <img :src="logo" alt="TTRPG Helper logo" class="h-8 w-8 rounded-lg" />
        <span class="font-semibold tracking-wide text-slate-100">TTRPG Helper</span>
      </RouterLink>
      <!-- Desktop -->
      <div class="hidden items-center gap-6 md:flex">
        <div class="flex items-center gap-4 text-sm font-medium">
          <RouterLink
            v-for="link in navLinks"
            :key="link.to"
            :to="link.to"
            class="text-slate-300 transition hover:text-emerald-400"
            active-class="text-emerald-400"
          >
            {{ link.label }}
          </RouterLink>
        </div>

        <div class="flex items-center gap-3">
          <template v-if="!isAuthenticated">
            <RouterLink
              to="/login"
              class="text-slate-300 transition hover:text-white text-sm font-medium px-2"
            >
              Zaloguj się
            </RouterLink>

            <RouterLink
              to="/register"
              class="rounded-xl bg-emerald-600 px-4 py-2 text-sm font-semibold text-white shadow-lg shadow-emerald-900/20 transition hover:bg-emerald-500"
            >
              Zarejestruj się
            </RouterLink>
          </template>
          <!-- Nazwa + Avatar usera (link do profilu) -->
          <template v-else>
            <RouterLink
              to="/profile"
              class="group flex items-center gap-2 rounded-full border border-slate-700 bg-slate-900/50 pr-4 pl-1 py-1 transition hover:bg-slate-800 hover:border-slate-500"
            >
              <img
                :src="userAvatarUrl"
                class="h-8 w-8 rounded-full object-cover border border-slate-800"
              />
              <span
                class="hidden text-sm font-medium text-slate-200 lg:block group-hover:text-white"
              >
                {{ username }}
              </span>
            </RouterLink>

            <!-- FRIENDS ICON MENU -->

            <div class="relative">
              <button
                @click="toggleFriends"
                class="p-2 rounded-full bg-emerald-600 hover:bg-slate-700 text-slate-300 transition"
              >
                👥
              </button>

              <FriendsDropdown v-if="showFriends" :onClose="toggleFriends" />
            </div>

            <button
              @click="handleLogout"
              class="ml-2 text-sm text-slate-400 transition hover:text-red-400"
            >
              Wyloguj
            </button>
          </template>
        </div>
      </div>
      <!-- Mobile burger -->
      <button
        class="inline-flex items-center justify-center rounded-md border border-slate-700 p-2 text-slate-300 md:hidden hover:bg-slate-800"
        @click="isOpen = !isOpen"
      >
        <span v-if="!isOpen">☰</span>
        <span v-else>✕</span>
      </button>
    </nav>

    <!-- Mobile menu -->
    <div v-if="isOpen" class="border-t border-slate-800 bg-slate-950 md:hidden">
      <div class="flex flex-col gap-2 p-4 text-sm">
        <RouterLink
          v-for="link in navLinks"
          :key="link.to"
          :to="link.to"
          class="block rounded-lg px-3 py-2 text-slate-300 transition hover:bg-slate-800 hover:text-emerald-400"
          active-class="bg-slate-900 text-emerald-400"
          @click="isOpen = false"
        >
          {{ link.label }}
        </RouterLink>

        <div
          v-if="!isAuthenticated"
          class="mt-4 flex flex-col gap-2 border-t border-slate-800 pt-4"
        >
          <RouterLink
            to="/login"
            class="w-full text-center py-2 text-slate-300"
            @click="isOpen = false"
            >Zaloguj</RouterLink
          >
          <RouterLink
            to="/register"
            class="w-full text-center py-2 bg-emerald-600 rounded-lg text-white"
            @click="isOpen = false"
            >Rejestracja</RouterLink
          >
        </div>
        <template v-else>
          <!-- Mobile user menu -->
          <RouterLink
            to="/profile"
            class="group flex items-center gap-2 rounded-full border border-slate-700 bg-slate-900/50 pr-4 pl-1 py-1 transition hover:bg-slate-800 hover:border-slate-500"
          >
            <img
              :src="userAvatarUrl"
              class="h-8 w-8 rounded-full object-cover border border-slate-800"
            />
            <span class="text-sm font-medium text-slate-200 group-hover:text-white">
              {{ username }}
            </span>
          </RouterLink>

          <div class="mt-4 border-t border-slate-800 pt-4">
            <RouterLink
              to="/profile"
              class="w-full text-left px-3 py-2 text-slate-300 hover:bg-slate-900 rounded-lg"
              @click="isOpen = false"
              >Profil</RouterLink
            >
            <button
              @click="handleLogout"
              class="w-full text-left px-3 py-2 text-red-400 hover:bg-slate-900 rounded-lg"
            >
              Wyloguj
            </button>
          </div>
        </template>
      </div>
    </div>
  </header>
</template>
