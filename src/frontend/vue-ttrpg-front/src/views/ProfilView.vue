<script setup lang="ts">
import { ref, reactive, onMounted, computed, watch } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { storeToRefs } from 'pinia'

const auth = useAuthStore()
const { user, userAvatarUrl } = storeToRefs(auth)

const isLoading = ref(false)
const message = ref<{ text: string; type: 'success' | 'error' } | null>(null)

const isEditingName = ref(false)
const isEditingEmail = ref(false)
const isChangingPassword = ref(false)

// Forms
const profileForm = reactive({ username: '', email: '' })
const passwordForm = reactive({ currentPassword: '', newPassword: '', confirmPassword: '' })
const fileInput = ref<HTMLInputElement | null>(null)

// --- LIFECYCLE & WATCHERS ---
const resetProfileForm = () => {
  profileForm.username = user.value?.userName || ''
  profileForm.email = user.value?.email || ''
}

onMounted(() => {
  if (!user.value) {
    auth.fetchCurrentUser()
  }
  resetProfileForm()
})

// Ensure form stays in sync if Store data loads late
watch(user, () => {
  // Only update if user isn't currently typing/editing
  if (!isEditingName.value && !isEditingEmail.value) {
    resetProfileForm()
  }
})

// --- ACTIONS ---
const isPasswordFormValid = computed(() => {
  return (
    passwordForm.currentPassword.trim().length > 0 &&
    passwordForm.newPassword.trim().length > 0 &&
    passwordForm.confirmPassword.trim().length > 0 &&
    passwordForm.newPassword === passwordForm.confirmPassword
  )
})

const cancelEditName = () => {
  profileForm.username = user.value?.userName || ''
  isEditingName.value = false
}

const cancelEditEmail = () => {
  profileForm.email = user.value?.email || ''
  isEditingEmail.value = false
}

const triggerAvatarUpload = () => fileInput.value?.click()

const handleFileChange = async (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files[0]) {
    const file = target.files[0]

    // Validate size (e.g., max 2MB)
    if (file.size > 2 * 1024 * 1024) {
      alert('Plik jest za duży (max 2MB)')
      return
    }

    try {
      // 1. Call the Store Action
      await auth.uploadAvatar(file)

      // 2. Show success message
      message.value = { text: 'Avatar został zmieniony!', type: 'success' }
    } catch (error) {
      message.value = { text: 'Błąd podczas wysyłania zdjęcia.', type: 'error' }
    } finally {
      // Clear input so selecting the same file again works if needed
      if (fileInput.value) fileInput.value.value = ''
    }
  }
}

const handleUpdateProfile = async () => {
  isLoading.value = true
  message.value = null

  try {
    const payload: any = {}

    // Only send username if in edit mode
    if (isEditingName.value) {
      payload.UserName = profileForm.username
    }

    // Only send email if in edit mode
    if (isEditingEmail.value) {
      payload.Email = profileForm.email
    }

    // === Send the partial update ===
    await auth.updateProfile(payload)

    // Update local store (example, depends on your store)
    if (user.value) {
      if (payload.UserName) user.value.userName = payload.UserName
      if (payload.Email) user.value.email = payload.Email
    }

    message.value = { text: 'Profil zaktualizowany!', type: 'success' }

    isEditingEmail.value = false
    isEditingName.value = false
  } catch (err) {
    message.value = { text: 'Błąd aktualizacji.', type: 'error' }
  } finally {
    isLoading.value = false
  }
}

const handleChangePassword = async () => {
  if (passwordForm.newPassword !== passwordForm.confirmPassword) {
    message.value = { text: 'Hasła nie są identyczne!', type: 'error' }
    return
  }

  isLoading.value = true
  try {
    await auth.updatePassword(passwordForm.currentPassword, passwordForm.newPassword)
    message.value = { text: 'Hasło zmienione pomyślnie.', type: 'success' }
    isChangingPassword.value = false
    passwordForm.currentPassword = ''
    passwordForm.newPassword = ''
    passwordForm.confirmPassword = ''
  } catch (err) {
    message.value = { text: 'Nie udało się zmienić hasła.', type: 'error' }
  } finally {
    isLoading.value = false
  }
}

const handleDeleteAccount = async () => {
  if (!confirm('Czy na pewno chcesz usunąć konto?')) return
  try {
    auth.deleteUser()
    alert('Konto usunięte.')
  } catch (err) {
    alert('Błąd usuwania konta.')
  }
}
</script>

<template>
  <div class="min-h-screen bg-slate-950 px-4 py-12 text-slate-200">
    <div class="mx-auto max-w-5xl">
      <h1 class="mb-8 text-3xl font-bold text-white">Ustawienia Profilu</h1>

      <div
        v-if="message"
        :class="[
          'mb-6 rounded-lg p-4 text-sm font-medium border',
          message.type === 'success'
            ? 'bg-emerald-900/30 border-emerald-800 text-emerald-400'
            : 'bg-red-900/30 border-red-800 text-red-400',
        ]"
      >
        {{ message.text }}
      </div>

      <div class="grid gap-8 md:grid-cols-3">
        <div class="md:col-span-1">
          <div
            class="rounded-2xl border border-slate-800 bg-slate-900/50 p-6 text-center shadow-xl"
          >
            <div class="relative mx-auto mb-4 h-32 w-32">
              <img
                :src="userAvatarUrl"
                alt="Avatar"
                class="h-full w-full rounded-full object-cover ring-4 ring-slate-800"
              />
              <button
                @click="triggerAvatarUpload"
                class="absolute bottom-0 right-0 rounded-full bg-emerald-600 p-2 text-white shadow-lg transition hover:bg-emerald-500 hover:scale-105"
                title="Zmień avatar"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="16"
                  height="16"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                >
                  <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                  <polyline points="17 8 12 3 7 8" />
                  <line x1="12" x2="12" y1="3" y2="15" />
                </svg>
              </button>
              <input
                type="file"
                ref="fileInput"
                class="hidden"
                accept="image/*"
                @change="handleFileChange"
              />
            </div>

            <h2 class="text-xl font-bold text-white">{{ user?.userName }}</h2>
            <p class="text-sm text-slate-400">{{ user?.email || 'Brak adresu e-mail' }}</p>
          </div>
        </div>

        <div class="space-y-6 md:col-span-2">
          <div class="rounded-2xl border border-slate-800 bg-slate-900/50 p-6 shadow-xl">
            <div class="flex items-center justify-between mb-6">
              <h3 class="text-lg font-semibold text-white">Dane</h3>
            </div>
            <!-- USERNAME DISPLAY -->
            <div v-if="!isEditingName" class="flex items-center justify-between py-2">
              <div>
                <span class="text-slate-500">Nazwa użytkownika</span>
                <span class="ml-3 text-slate-200 font-medium">{{ user?.userName }}</span>
              </div>

              <button
                @click="isEditingName = true"
                class="text-sm text-emerald-400 hover:text-emerald-300 hover:underline"
              >
                Edytuj
              </button>
            </div>
            <!-- USERNAME EDIT FORM -->
            <div v-else class="space-y-3 py-2">
              <label class="block text-sm font-medium text-slate-400">Nazwa użytkownika</label>
              <input
                v-model="profileForm.username"
                type="text"
                class="mt-1 w-full rounded-lg border border-slate-700 bg-slate-950 px-4 py-2 text-white focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
              />

              <div class="flex justify-end gap-3 pt-1">
                <button
                  type="button"
                  @click="cancelEditName"
                  class="rounded-lg px-3 py-1 text-sm text-slate-400 hover:text-white transition"
                >
                  Anuluj
                </button>

                <button
                  @click="handleUpdateProfile"
                  :disabled="isLoading || !profileForm.username"
                  class="rounded-lg bg-emerald-600 px-3 py-1 text-sm font-semibold text-white hover:bg-emerald-500 disabled:opacity-50"
                >
                  {{ isLoading ? 'Zapisywanie...' : 'Zapisz' }}
                </button>
              </div>
            </div>

            <!-- EMAIL DISPLAY -->
            <div v-if="!isEditingEmail" class="flex items-center justify-between py-2">
              <div>
                <span class="text-slate-500">Email</span>
                <span class="ml-3 text-slate-200 font-medium">{{ user?.email || '-' }}</span>
              </div>

              <button
                @click="isEditingEmail = true"
                class="text-sm text-emerald-400 hover:text-emerald-300 hover:underline"
              >
                Edytuj
              </button>
            </div>
            <!-- EMAIL EDIT FORM -->
            <div v-else class="space-y-3 py-2">
              <label class="block text-sm font-medium text-slate-400">Email</label>
              <input
                v-model="profileForm.email"
                type="email"
                class="mt-1 w-full rounded-lg border border-slate-700 bg-slate-950 px-4 py-2 text-white focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
              />

              <div class="flex justify-end gap-3 pt-1">
                <button
                  type="button"
                  @click="cancelEditEmail"
                  class="rounded-lg px-3 py-1 text-sm text-slate-400 hover:text-white transition"
                >
                  Anuluj
                </button>

                <button
                  @click="handleUpdateProfile"
                  :disabled="isLoading || !profileForm.email"
                  class="rounded-lg bg-emerald-600 px-3 py-1 text-sm font-semibold text-white hover:bg-emerald-500 disabled:opacity-50"
                >
                  {{ isLoading ? 'Zapisywanie...' : 'Zapisz' }}
                </button>
              </div>
            </div>
          </div>

          <div class="rounded-2xl border border-slate-800 bg-slate-900/50 p-6 shadow-xl">
            <div class="flex items-center justify-between mb-6">
              <h3 class="text-lg font-semibold text-white">Bezpieczeństwo</h3>
              <button
                @click="isChangingPassword = !isChangingPassword"
                class="text-sm text-emerald-400 hover:text-emerald-300 hover:underline"
              >
                {{ isChangingPassword ? 'Anuluj' : 'Zmień hasło' }}
              </button>
            </div>

            <form @submit.prevent="handleChangePassword" class="space-y-4">
              <div v-show="isChangingPassword">
                <div>
                  <label class="block text-sm font-medium text-slate-400">Obecne hasło</label>
                  <input
                    v-model="passwordForm.currentPassword"
                    type="password"
                    autocomplete="current-password"
                    class="mt-1 w-full rounded-lg border border-slate-700 bg-slate-950 px-4 py-2 text-white focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
                  />
                </div>
                <div class="grid gap-4 md:grid-cols-2">
                  <div>
                    <label class="block text-sm font-medium text-slate-400">Nowe hasło</label>
                    <input
                      v-model="passwordForm.newPassword"
                      type="password"
                      autocomplete="new-password"
                      class="mt-1 w-full rounded-lg border border-slate-700 bg-slate-950 px-4 py-2 text-white focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-slate-400">Potwierdź hasło</label>
                    <input
                      v-model="passwordForm.confirmPassword"
                      type="password"
                      autocomplete="new-password"
                      class="mt-1 w-full rounded-lg border border-slate-700 bg-slate-950 px-4 py-2 text-white focus:border-emerald-500 focus:outline-none focus:ring-1 focus:ring-emerald-500"
                    />
                  </div>
                  <p
                    v-if="
                      passwordForm.confirmPassword.length > 0 &&
                      passwordForm.newPassword !== passwordForm.confirmPassword
                    "
                    class="text-sm text-red-500 mt-1"
                  >
                    Hasła nie są identyczne.
                  </p>
                </div>
                <div class="flex justify-end pt-2">
                  <button
                    type="submit"
                    :disabled="!isPasswordFormValid"
                    class="rounded-lg bg-emerald-600 px-4 py-2 text-sm font-semibold text-white disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    Zaktualizuj hasło
                  </button>
                </div>
              </div>
            </form>
          </div>

          <div class="rounded-2xl border border-red-900/30 bg-red-950/10 p-6 shadow-xl">
            <h3 class="text-lg font-semibold text-red-500">Strefa Niebezpieczna</h3>
            <div class="mt-4 flex flex-col md:flex-row md:items-center md:justify-between gap-4">
              <p class="text-sm text-slate-400">
                Usunięcie konta jest nieodwracalne. Utracisz wszystkie swoje postacie i kampanie.
              </p>
              <button
                @click="handleDeleteAccount"
                class="shrink-0 rounded-lg border border-red-800 px-4 py-2 text-sm font-medium text-red-500 transition hover:bg-red-900/50 hover:text-red-400"
              >
                Usuń konto
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
