<script setup lang="ts">
import type { UserInfoDto } from '@/stores/friends'

const props = defineProps<{
  user: UserInfoDto
}>()

const emit = defineEmits<{
  (e: 'close'): void // Emity wysyłany, gdy użytkownik kliknie 'X'
}>()
</script>

<template>
  <div
    class="chat-popup fixed bottom-0 right-4 w-64 bg-slate-900 border border-slate-700 shadow-xl rounded-t-xl z-50"
  >
    <div
      class="flex justify-between items-center p-3 bg-slate-800 border-b border-slate-700 rounded-t-xl"
    >
      <img
        :src="props.user.avatarUrl || 'https://via.placeholder.com/32'"
        class="h-8 w-8 rounded-full border border-slate-700 object-cover"
      />
      <span class="text-slate-200 font-semibold truncate">{{ props.user.userName }}</span>
      <button
        @click="emit('close')"
        class="text-slate-400 hover:text-white transition-colors p-1"
        aria-label="Zamknij czat"
      >
        X
      </button>
    </div>
    <div class="chat-content p-3 h-40 overflow-y-auto text-sm text-slate-400">
      <p class="mt-2 text-xs text-slate-500">
        Miejsce na przyszłe wiadomości i pole do wpisywania.
      </p>
    </div>
  </div>
</template>
