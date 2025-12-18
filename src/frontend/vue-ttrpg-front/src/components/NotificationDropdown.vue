<script setup lang="ts">
import { onMounted } from 'vue'
import { storeToRefs } from 'pinia'
import { useNotificationStore } from '@/stores/notificationStore' // Adjust path if needed

// 1. Define Props (for the Close button)
const props = defineProps<{
  onClose: () => void
}>()

// 2. Initialize Store
const notificationStore = useNotificationStore()
const { notifications, isLoading } = storeToRefs(notificationStore)

// 3. Handlers
const handleMarkAsRead = async (id: number) => {
  await notificationStore.markAsRead(id)
}

// 4. Lifecycle - Fetch on mount
onMounted(() => {
  notificationStore.fetchNotifications()
})
</script>

<template>
  <div
    class="absolute right-0 mt-2 w-72 bg-slate-900 border border-slate-700 rounded-xl shadow-xl p-2 z-50"
  >
    <button
      @click="props.onClose"
      class="absolute top-2 right-2 text-slate-500 hover:text-white transition-colors p-1"
      aria-label="Zamknij powiadomienia"
    >
      X
    </button>

    <h3 class="block text-sm text-slate-400 m-2 px-1 pb-2">Powiadomienia</h3>

    <div v-if="isLoading" class="text-slate-500 text-xs text-center p-2">Ładowanie...</div>
    <div v-else-if="notifications.length === 0" class="text-slate-500 text-xs text-center p-2">
      Brak nowych powiadomień
    </div>

    <div v-else>
      <div
        v-for="n in notifications"
        :key="n.id"
        class="flex items-start justify-between p-2 rounded-lg group transition-colors"
        :class="{
          'bg-slate-800/50 hover:bg-slate-800 border border-slate-700/50': !n.isRead,
          'bg-slate-900 hover:bg-slate-800/50': n.isRead,
        }"
      >
        <div class="flex-grow min-w-0 pr-3">
          <span
            v-if="!n.isRead"
            class="block h-2 w-2 rounded-full bg-emerald-500 float-left mr-2 mt-1"
          ></span>

          <p
            class="text-sm font-semibold truncate"
            :class="n.isRead ? 'text-slate-400' : 'text-white'"
            :title="n.title"
          >
            {{ n.title }}
          </p>
          <p class="text-xs text-slate-500 mt-0.5 whitespace-normal">
            {{ n.message }}
          </p>
        </div>

        <button
          v-if="!n.isRead"
          @click="handleMarkAsRead(n.id)"
          class="flex-shrink-0 ml-2 mt-0.5 text-emerald-400 hover:text-emerald-300 hover:bg-emerald-200/20 rounded-md text-sm font-medium p-1 transition-opacity"
          aria-label="Oznacz jako przeczytane"
        >
          ✓
        </button>
        <div v-else class="flex-shrink-0 ml-2 mt-0.5 text-slate-600 text-sm font-medium p-1">✓</div>
      </div>
    </div>
  </div>
</template>
