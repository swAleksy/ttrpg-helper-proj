<script setup lang="ts">
import Navbar from '@/components/Navbar.vue'
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()

onMounted(() => {
  auth.initializeAuth()
})
</script>

<template>
  <div class="min-h-screen flex flex-col bg-slate-950 text-slate-50">
    <Navbar />

    <main class="flex-1">
      <RouterView />
    </main>
  </div>
</template>
