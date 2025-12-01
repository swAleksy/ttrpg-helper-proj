<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const email = ref('')
const password = ref('')
const isSubmitting = ref(false)

const onSubmit = async () => {
  isSubmitting.value = true
  try {
    // POŁĄCZENIE Z BACKENDEM + Pinia (store auth) !!!!!!!!!!!!!!!!!
    console.log('Logowanie...', { email: email.value, password: password.value })

    // na razie tylko prowizoryczne przekierowanie "udane logowanie"
    await router.push('/dashboard')
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
          <label for="email" class="text-sm font-medium text-slate-200">
            E-mail
          </label>
          <input
            id="email"
            v-model="email"
            type="email"
            required
            class="w-full rounded-xl border border-slate-700 bg-slate-900 px-3 py-2 text-sm text-slate-50 outline-none ring-emerald-500/40 placeholder:text-slate-500 focus:border-emerald-400 focus:ring-2"
            placeholder="mistrz@przygoda.com"
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
