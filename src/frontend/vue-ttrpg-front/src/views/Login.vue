<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const auth = useAuthStore()

const username = ref('')
const password = ref('')
const isSubmitting = ref(false)
const errorMessage = ref('')

const onSubmit = async () => {
  isSubmitting.value = true
  errorMessage.value = ''
  try {
    await auth.login(username.value, password.value)
    // POŁĄCZENIE Z BACKENDEM + Pinia (store auth) !!!!!!!!!!!!!!!!!
    console.log('Logowanie...', { username: username.value, password: password.value })

    router.push('/dashboard')
  } catch (err: any) {
    // czy backend zwraca komunikat błędu w body?
    errorMessage.value = 'Nie udało się zalogować. Sprawdź nazwę użytkownika lub hasło.'
    console.error(err)
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <section class="flex min-h-[calc(100vh-4rem)] items-center justify-center">
    <div class="mx-auto w-full max-w-md rounded-2xl border border-slate-800 bg-slate-900/60 p-6 shadow-xl">
      <h2 class="text-center text-2xl font-semibold tracking-tight">
        Zaloguj się
      </h2>
      <p class="mt-1 text-center text-sm text-slate-400">
        Wróć do swoich kampanii i postaci.
      </p>

      <form class="mt-6 space-y-4" @submit.prevent="onSubmit">
        <div class="space-y-1">
          <label for="username" class="text-sm font-medium text-slate-200">
            Nazwa użytkownika
          </label>
          <input
            id="username"
            v-model="username"
            type="text"
            required
            class="w-full rounded-xl border border-slate-700 bg-slate-900 px-3 py-2 text-sm text-slate-50 outline-none ring-emerald-500/40 placeholder:text-slate-500 focus:border-emerald-400 focus:ring-2"
            placeholder="MistrzGry123"
          />
        </div>

        <div class="space-y-1">
          <label for="password" class="text-sm font-medium text-slate-200">
            Hasło
          </label>
          <input
            id="password"
            v-model="password"
            type="password"
            required
            class="w-full rounded-xl border border-slate-700 bg-slate-900 px-3 py-2 text-sm text-slate-50 outline-none ring-emerald-500/40 placeholder:text-slate-500 focus:border-emerald-400 focus:ring-2"
            placeholder="••••••••"
          />
        </div>

        <button
          type="submit"
          :disabled="isSubmitting"
          class="mt-4 flex w-full items-center justify-center rounded-xl bg-emerald-500 px-4 py-2.5 text-sm font-semibold text-slate-950 shadow transition hover:bg-emerald-400 disabled:cursor-not-allowed disabled:opacity-70"
        >
          {{ isSubmitting ? 'Logowanie...' : 'Zaloguj się' }}
     
        </button>
        <!-- ??? -->
        <p v-if="errorMessage" class="mt-2 text-sm text-red-400">
          {{ errorMessage }}
        </p>

      </form>

      <p class="mt-4 text-center text-xs text-slate-400">
        Nie masz konta?
        <RouterLink to="/register" class="text-emerald-400 hover:underline">
          Zarejestruj się
        </RouterLink>
      </p>
    </div>
  </section>
</template>
