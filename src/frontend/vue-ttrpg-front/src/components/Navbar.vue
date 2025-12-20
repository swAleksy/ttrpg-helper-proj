<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import logo from '@/assets/img/logo.png'
import { useRouter } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useAuthStore } from '@/stores/auth'
import { useCommunicationStore } from '@/stores/communicationStore'
import { useNotificationStore } from '@/stores/notificationStore'
import { useFriendsStore } from '@/stores/friendsStore'
import FriendsDropdown from '@/components/FriendsDropdown.vue'
import NotificationDropdown from '@/components/NotificationDropdown.vue'

const router = useRouter()

// Stores
const authStore = useAuthStore()
const commStore = useCommunicationStore()
const notificationStore = useNotificationStore()
const friendsStore = useFriendsStore()

// Refs from stores
const { isAuthenticated, username, userAvatarUrl } = storeToRefs(authStore)
const { unreadCount: notificationCount } = storeToRefs(notificationStore)
const { pendingCount: friendRequestCount } = storeToRefs(friendsStore)

// Local UI state
const isMobileMenuOpen = ref(false)
const showFriendsDropdown = ref(false)
const showNotificationDropdown = ref(false)

// Computed
const navLinks = computed(() => {
  if (!isAuthenticated.value) {
    return [{ to: '/', label: 'Start' }]
  }
  return [{ to: '/dashboard', label: 'Panel' }]
})

// Actions
const toggleFriendsDropdown = () => {
  showFriendsDropdown.value = !showFriendsDropdown.value
  showNotificationDropdown.value = false
}

const toggleNotificationDropdown = () => {
  showNotificationDropdown.value = !showNotificationDropdown.value
  showFriendsDropdown.value = false
  if (showNotificationDropdown.value) {
    notificationStore.markAllAsRead()
  }
}

const handleLogout = () => {
  commStore.stopSignalR()
  authStore.logout()
  isMobileMenuOpen.value = false
  router.push('/')
}

// Lifecycle
onMounted(() => {
  if (isAuthenticated.value) {
    commStore.initSignalR()
  }
})

watch(isAuthenticated, (isAuth) => {
  if (isAuth) {
    commStore.initSignalR()
  } else {
    commStore.stopSignalR()
  }
})
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

          <template v-else>
            <!-- User Avatar -->
            <RouterLink
              to="/profile"
              class="group flex items-center gap-2 rounded-full border border-slate-700 bg-slate-900/50 pr-4 pl-1 py-1 transition hover:bg-slate-800 hover:border-slate-500"
            >
              <img
                :src="userAvatarUrl"
                class="h-8 w-8 rounded-full object-cover border border-slate-800"
              />
              <span class="hidden text-sm font-medium text-slate-200 lg:block group-hover:text-white">
                {{ username }}
              </span>
            </RouterLink>

            <!-- Notifications Button -->
            <div class="relative">
              <button
                @click="toggleNotificationDropdown"
                class="relative p-2 rounded-full bg-emerald-600 hover:bg-slate-700 text-slate-300 transition"
                aria-label="Powiadomienia"
              >
                🔔
                <span
                  v-if="notificationCount > 0"
                  class="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-red-500 text-[10px] font-bold text-white shadow-sm ring-2 ring-slate-900"
                >
                  {{ notificationCount > 9 ? '9+' : notificationCount }}
                </span>
              </button>
              <NotificationDropdown v-if="showNotificationDropdown" :onClose="toggleNotificationDropdown" />
            </div>

            <!-- Friends Button -->
            <div class="relative">
              <button
                @click="toggleFriendsDropdown"
                class="relative p-2 rounded-full bg-emerald-600 hover:bg-slate-700 text-slate-300 transition"
                aria-label="Znajomi"
              >
                👥
                <span
                  v-if="friendRequestCount > 0"
                  class="absolute -top-1 -right-1 flex h-4 w-4 items-center justify-center rounded-full bg-red-500 text-[10px] font-bold text-white shadow-sm ring-2 ring-slate-900"
                >
                  {{ friendRequestCount > 9 ? '9+' : friendRequestCount }}
                </span>
              </button>
              <FriendsDropdown v-if="showFriendsDropdown" :onClose="toggleFriendsDropdown" />
            </div>

            <!-- Logout Button -->
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
        @click="isMobileMenuOpen = !isMobileMenuOpen"
        aria-label="Menu"
      >
        <span v-if="!isMobileMenuOpen">☰</span>
        <span v-else>✕</span>
      </button>
    </nav>

    <!-- Mobile menu -->
    <div v-if="isMobileMenuOpen" class="border-t border-slate-800 bg-slate-950 md:hidden">
      <div class="flex flex-col gap-2 p-4 text-sm">
        <RouterLink
          v-for="link in navLinks"
          :key="link.to"
          :to="link.to"
          class="block rounded-lg px-3 py-2 text-slate-300 transition hover:bg-slate-800 hover:text-emerald-400"
          active-class="bg-slate-900 text-emerald-400"
          @click="isMobileMenuOpen = false"
        >
          {{ link.label }}
        </RouterLink>

        <div v-if="!isAuthenticated" class="mt-4 flex flex-col gap-2 border-t border-slate-800 pt-4">
          <RouterLink to="/login" class="w-full text-center py-2 text-slate-300" @click="isMobileMenuOpen = false">
            Zaloguj
          </RouterLink>
          <RouterLink to="/register" class="w-full text-center py-2 bg-emerald-600 rounded-lg text-white" @click="isMobileMenuOpen = false">
            Rejestracja
          </RouterLink>
        </div>

        <template v-else>
          <RouterLink
            to="/profile"
            class="group flex items-center gap-2 rounded-full border border-slate-700 bg-slate-900/50 pr-4 pl-1 py-1 transition hover:bg-slate-800 hover:border-slate-500"
          >
            <img :src="userAvatarUrl" class="h-8 w-8 rounded-full object-cover border border-slate-800" />
            <span class="text-sm font-medium text-slate-200 group-hover:text-white">{{ username }}</span>
          </RouterLink>

          <div class="mt-4 border-t border-slate-800 pt-4">
            <RouterLink
              to="/profile"
              class="w-full text-left px-3 py-2 text-slate-300 hover:bg-slate-900 rounded-lg"
              @click="isMobileMenuOpen = false"
            >
              Profil
            </RouterLink>
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
