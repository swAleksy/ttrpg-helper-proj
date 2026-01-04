<template>
  <div class="min-h-screen bg-slate-950 text-slate-100">
    <div class="mx-auto max-w-6xl px-4 py-8 space-y-6">
      <!-- Header -->
      <header class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div class="space-y-1">
          <h1 class="text-3xl font-bold tracking-tight">Postacie</h1>
          <p class="text-sm text-slate-400">
            Lista Twoich postaci.
          </p>
        </div>

        <!-- przycisk Nowa postać -->
        <router-link
          to="/characters/new"
          class="inline-flex items-center gap-2 rounded-xl px-4 py-2 border border-emerald-500/40 bg-emerald-500/10 text-emerald-100 hover:bg-emerald-500/20 hover:border-emerald-400 transition"
        >
          ➕ Nowa postać
        </router-link>
      </header>

      <!-- Content -->
      <section class="rounded-3xl border border-slate-800 bg-slate-900/30 p-6">
        <div v-if="loading" class="text-slate-300">
          Ładowanie postaci...
        </div>

        <div v-else-if="error" class="text-red-300">
          {{ error }}
        </div>

        <div v-else>
          <div v-if="characters.length === 0" class="text-slate-300">
            Nie masz jeszcze żadnych postaci. Kliknij <span class="text-emerald-200 font-medium">Nowa postać</span>, aby dodać pierwszą.
          </div>

          <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            <div
              v-for="c in characters"
              :key="c.id"
              class="rounded-2xl border border-slate-800 bg-slate-950/30 p-4 hover:bg-slate-950/45 transition cursor-pointer"
              role="button"
              tabindex="0"
              @click="openDetailsModal(c)"
              @keydown.enter.prevent="openDetailsModal(c)"
              @keydown.space.prevent="openDetailsModal(c)"            >
              <div class="flex items-center gap-3 min-w-0">
                <!-- avatar + tekst -->
                <div class="flex items-center gap-3 min-w-0">
                  <img
                    :src="avatarSrc(c)"
                    alt=""
                    class="h-12 w-12 rounded-xl object-cover border border-slate-800 bg-slate-900/40"
                    loading="lazy"
                    @error="handleAvatarError"
                  />

                  <div class="min-w-0">
                    <div class="font-semibold text-lg truncate">
                      {{ c.name ?? 'Bez nazwy' }}
                    </div>

                    <div class="text-sm text-slate-400 truncate">
                      {{ subtitle(c) || '—' }}
                    </div>
                  </div>
                </div>
              </div>

            </div>
          </div>
        </div>
      </section>

      <!-- Modal: potwierdzenie usunięcia -->
      <div
        v-if="isDeleteModalOpen"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/60 px-4"
        @click="closeDeleteModal"
      >
        <div
          class="w-full max-w-md rounded-2xl border border-slate-700 bg-slate-950/95 shadow-2xl"
          @click.stop
        >
          <!-- Header -->
          <div class="flex items-center justify-between border-b border-slate-800 px-4 py-3">
            <h3 class="text-base font-semibold">Usuń postać</h3>
            <button
              type="button"
              class="rounded-full p-1 text-slate-400 hover:bg-slate-800 hover:text-slate-100"
              @click="closeDeleteModal"
              aria-label="Zamknij"
            >
              ✕
            </button>
          </div>

          <!-- Body -->
          <div class="px-4 py-4 space-y-2">
            <p class="text-sm text-slate-200">
              Czy na pewno chcesz usunąć postać
              <span class="font-semibold text-slate-100">
                {{ deleteTargetName }}
              </span>?
            </p>
            <p class="text-xs text-slate-500">
              Tej operacji nie można cofnąć.
            </p>
          </div>

          <!-- Actions -->
          <div class="flex justify-end gap-3 border-t border-slate-800 px-4 py-3">
            <button
              type="button"
              class="rounded-xl border border-slate-600 px-3 py-1.5 text-sm text-slate-200 hover:bg-slate-800"
              :disabled="deleting"
              @click="closeDeleteModal"
            >
              Anuluj
            </button>

            <button
              type="button"
              class="rounded-xl border border-rose-400 bg-rose-500/90 px-3 py-1.5 text-sm font-medium text-slate-950 hover:bg-rose-400 disabled:opacity-60 disabled:cursor-not-allowed"
              :disabled="deleting"
              @click="confirmDelete"
            >
              {{ deleting ? 'Usuwanie…' : 'Usuń' }}
            </button>
          </div>
        </div>
      </div>

      <!-- Modal: szczegóły postaci -->
      <div
        v-if="isDetailsModalOpen"
        class="fixed inset-0 z-40 flex items-center justify-center bg-black/60 px-4"
        @click="closeDetailsModal"
      >
        <div
          class="w-full max-w-2xl rounded-2xl border border-slate-700 bg-slate-950/95 shadow-2xl"
          @click.stop
        >
          <!-- Header -->
          <div class="flex items-center justify-between border-b border-slate-800 px-4 py-3">
            <div class="min-w-0">
              <h3 class="text-base font-semibold truncate">
                Szczegóły postaci: <span class="font-bold text-slate-100">{{ detailsTitle }}</span>
              </h3>
            </div>

            <button
              type="button"
              class="rounded-full p-1 text-slate-400 hover:bg-slate-800 hover:text-slate-100"
              @click="closeDetailsModal"
              aria-label="Zamknij"
            >
              ✕
            </button>
          </div>

          <!-- Body -->
          <div class="px-4 py-4 space-y-4">
            <div class="flex items-center gap-4">
              <!-- Avatar -->
              <img
                :src="detailsAvatar"
                alt=""
                class="h-16 w-16 rounded-2xl object-cover border border-slate-800 bg-slate-900/40 shrink-0"
                @error="handleAvatarError"
              />

              <!-- nazwa (wypełnienie miedzy avatar i level) -->
              <div class="min-w-0 flex-1">
                <div class="rounded-xl border border-slate-800 bg-slate-900/20 p-3">
                  <div class="text-xs text-slate-400">Nazwa</div>
                  <div class="text-sm font-medium truncate">{{ detailsTitle }}</div>
                </div>
              </div>

              <!-- level -> symetrycznie do avataru -->
              <div
                class="h-16 w-16 rounded-2xl border border-slate-800 bg-slate-900/20 flex flex-col items-center justify-center shrink-0"
                title="Poziom"
              >
                <div class="text-[10px] text-slate-400 leading-none">LEVEL</div>
                <div class="text-lg font-bold leading-none text-slate-100">
                  {{ detailsLevel ?? '—' }}
                </div>
              </div>
            </div>


            <!-- Rasa / Klasa / Tło -->
            <div class="grid gap-3 grid-cols-1 sm:grid-cols-3">

              <div v-if="detailsRace" class="rounded-xl border border-slate-800 bg-slate-900/20 p-3">
                <div class="text-xs text-slate-400">Rasa</div>
                <div class="text-sm font-medium truncate">{{ detailsRace }}</div>
              </div>

              <div v-if="detailsClass" class="rounded-xl border border-slate-800 bg-slate-900/20 p-3">
                <div class="text-xs text-slate-400">Klasa</div>
                <div class="text-sm font-medium truncate">{{ detailsClass }}</div>
              </div>

              <div v-if="detailsBackground" class="rounded-xl border border-slate-800 bg-slate-900/20 p-3">
                <div class="text-xs text-slate-400">Tło</div>
                <div class="text-sm font-medium truncate">{{ detailsBackground }}</div>
              </div>
            </div>

            <!-- Ability scores -->
            <div class="rounded-xl border border-slate-800 bg-slate-900/20 p-3">
              <div class="flex items-center justify-between">
                <div class="text-xs text-slate-400">Atrybuty</div>
              </div>

              <div class="mt-2 grid grid-cols-3 sm:grid-cols-6 gap-2">
                <div
                  v-for="a in detailsAbilities"
                  :key="a.key"
                  class="rounded-xl border border-slate-800 bg-slate-950/30 p-2 text-center"
                >
                  <div class="text-[10px] text-slate-400 leading-none">{{ a.abbr }}</div>
                  <div class="text-lg font-semibold leading-tight">
                    {{ a.value ?? '—' }}
                  </div>
                </div>
              </div>
            </div>

            <!-- Proficient skills -->
            <div class="rounded-xl border border-slate-800 bg-slate-900/20 p-3">
              <div class="text-xs text-slate-400">Biegłości w umiejętnościach</div>

              <div v-if="detailsProficientSkills.length === 0" class="mt-2 text-sm text-slate-300">
                Brak zaznaczonych biegłości.
              </div>

              <div v-else class="mt-2 flex flex-wrap gap-2">
                <span
                  v-for="s in detailsProficientSkills"
                  :key="s"
                  class="inline-flex items-center rounded-full border border-emerald-500/30 bg-emerald-500/10 px-2.5 py-1 text-xs text-emerald-100"
                >
                  {{ s }}
                </span>
              </div>
            </div>


          </div>

          <!-- Actions -->
          <div class="flex justify-end gap-3 border-t border-slate-800 px-4 py-3">
            <button
              type="button"
              class="rounded-xl border border-slate-600 px-3 py-1.5 text-sm text-slate-200 hover:bg-slate-800"
              @click="closeDetailsModal"
            >
              Anuluj
            </button>

            <button
              type="button"
              class="rounded-xl border border-slate-600 px-3 py-1.5 text-sm text-slate-200 hover:bg-slate-800"
              :disabled="!detailsId"
              @click="goToEdit"
              title="Przejdź do edycji"
            >
              Edytuj
            </button>

            <button
              type="button"
              class="rounded-xl border border-rose-400 bg-rose-500/90 px-3 py-1.5 text-sm font-medium text-slate-950 hover:bg-rose-400 disabled:opacity-60 disabled:cursor-not-allowed"
              :disabled="!detailsTarget || deleting"
              @click="deleteFromDetails"
            >
              Usuń
            </button>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'

import defaultAvatar from '@/assets/img/DefaultCharacterAvatar.png'

// ten sam sposób budowania URL co w auth store
const API_URL = (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:8080'
const BACKEND_ORIGIN = API_URL.replace(/\/api\/?$/, '')

const router = useRouter()

const isDetailsModalOpen = ref(false)
const detailsTarget = ref<AnyCharacter | null>(null)

const detailsId = computed(() => {
  const c = detailsTarget.value
  return c ? getCharacterId(c) : null
})

const detailsTitle = computed(() => {
  const c = detailsTarget.value
  return (c?.name ?? 'Bez nazwy') as string
})

const detailsAvatar = computed(() => {
  const c = detailsTarget.value
  return c ? avatarSrc(c) : defaultAvatar
})

const detailsRace = computed(() => {
  const c = detailsTarget.value
  if (!c) return ''
  return (racesById.value[c.raceId] ?? '') as string
})

const detailsClass = computed(() => {
  const c = detailsTarget.value
  if (!c) return ''
  return (classesById.value[c.classId] ?? '') as string
})

const detailsBackground = computed(() => {
  const c = detailsTarget.value
  if (!c) return ''
  return (backgroundsById.value[c.backgroundId] ?? '') as string
})

const detailsLevel = computed(() => {
  const c = detailsTarget.value
  if (!c) return null
  return c.level ?? null
})


type AbilityKey = 'strength' | 'dexterity' | 'constitution' | 'intelligence' | 'wisdom' | 'charisma'

const ABILITIES: Array<{ key: AbilityKey; abbr: string }> = [
  { key: 'strength', abbr: 'STR' },
  { key: 'dexterity', abbr: 'DEX' },
  { key: 'constitution', abbr: 'CON' },
  { key: 'intelligence', abbr: 'INT' },
  { key: 'wisdom', abbr: 'WIS' },
  { key: 'charisma', abbr: 'CHA' },
]

function readAbilityScore(c: AnyCharacter | null, key: AbilityKey): number | null {
  if (!c) return null
  const val = Number(c[key])
  return Number.isFinite(val) ? val : null
}

// --- Computed używane w template ---
const detailsAbilities = computed(() => {
  const c = detailsTarget.value
  return ABILITIES.map(a => ({
    ...a,
    value: readAbilityScore(c, a.key),
  }))
})

const detailsProficientSkills = computed(() => {
  const c = detailsTarget.value
  if (!c || !Array.isArray(c.characterSkillsIds)) return []

  return Array.from(new Set(
    c.characterSkillsIds
      .map((x: any) => Number(x))
      .filter((n: number) => Number.isFinite(n))
      .map((id: number) => skillsById.value[id] ?? `Skill #${id}`)
  ))
})


function openDetailsModal(c: AnyCharacter) {
  console.log('[openDetailsModal] c =', c)
  detailsTarget.value = c
  isDetailsModalOpen.value = true
}

function closeDetailsModal() {
  if (deleting.value) return
  isDetailsModalOpen.value = false
  detailsTarget.value = null
}

function deleteFromDetails() {
  if (!detailsTarget.value) return
  // zamykanie szczegółów i otwieranie potwierdzenia usuwania
  const c = detailsTarget.value
  closeDetailsModal()
  openDeleteModal(c)
}

function goToEdit() {
  const id = detailsId.value
  if (id == null) {
    error.value = 'Nie mogę edytować postaci: brak ID.'
    closeDetailsModal()
    return
  }

  router.push(`/characters/${id}/edit`)
}



type AnyCharacter = Record<string, any>

const racesById = ref<Record<number, string>>({})
const classesById = ref<Record<number, string>>({})
const backgroundsById = ref<Record<number, string>>({})
const skillsById = ref<Record<number, string>>({})

const CLASS_AVATAR_MAP: Record<string, string> = {
  Barbarian: 'barbarian.png',
  Bard: 'bard.png',
  Cleric: 'cleric.png',
  Druid: 'druid.png',
  Fighter: 'fighter.png',
  Monk: 'monk.png',
  Paladin: 'paladin.png',
  Ranger: 'ranger.png',
  Rogue: 'rogue.png',
  Sorcerer: 'sorcerer.png',
  Warlock: 'warlock.png',
  Wizard: 'wizard.png',
}

async function fetchDictionaries() {
  try {
    const [racesRes, classesRes, backgroundsRes, skillsRes] = await Promise.all([
      axios.get(`${API_URL}/api/character/races`),
      axios.get(`${API_URL}/api/character/classes`),
      axios.get(`${API_URL}/api/character/backgrounds`),
      axios.get(`${API_URL}/api/character/skills`),
    ])

    racesById.value = Object.fromEntries(racesRes.data.map((r: any) => [Number(r.id), r.name]))
    classesById.value = Object.fromEntries(classesRes.data.map((c: any) => [Number(c.id), c.name]))
    backgroundsById.value = Object.fromEntries(backgroundsRes.data.map((b: any) => [Number(b.id), b.name]))
    skillsById.value = Object.fromEntries(skillsRes.data.map((s: any) => [Number(s.id), s.name]))
  } catch (e: any) {
    error.value = e?.message ?? 'Nie udało się pobrać rasy/klasy/tła/umiejętności.'
  }
}

function avatarSrc(c: AnyCharacter) {
  const className = classesById.value[c.classId]
  const fileName = className ? CLASS_AVATAR_MAP[className] : null
  if (!fileName) return defaultAvatar
  return `${BACKEND_ORIGIN}/characterAvatars/${fileName}`
}


function subtitle(c: AnyCharacter) {
  const race = racesById.value[c.raceId] ?? '—'
  const cls  = classesById.value[c.classId] ?? '—'
  const lvl  = c.level != null ? `Poziom ${c.level}` : null
  return [race, cls, lvl].filter(Boolean).join(' • ')
}


// fallback gdyby obraz się nie załadował
function handleAvatarError(event: Event) {
  const target = event.target as HTMLImageElement | null
  if (target) target.src = defaultAvatar
}

const characters = ref<AnyCharacter[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

function getCharacterId(c: AnyCharacter): number | null {
  const num = Number(c.id)
  return Number.isFinite(num) ? num : null
}


async function fetchCharacters() {
  loading.value = true
  error.value = null
  try {
    const res = await axios.get<AnyCharacter[]>(`${API_URL}/api/character/allcharacters`)
    characters.value = Array.isArray(res.data) ? res.data : []
  } catch (e: any) {
    error.value = e?.response?.data?.message
      ?? e?.message
      ?? 'Nie udało się pobrać listy postaci.'
  } finally {
    loading.value = false
  }
}

const isDeleteModalOpen = ref(false)
const deleteTarget = ref<AnyCharacter | null>(null)
const deleting = ref(false)

const deleteTargetName = computed(() => deleteTarget.value?.name ?? 'Bez nazwy')

function openDeleteModal(c: AnyCharacter) {
  deleteTarget.value = c
  isDeleteModalOpen.value = true
}

function closeDeleteModal() {
  if (deleting.value) return
  isDeleteModalOpen.value = false
  deleteTarget.value = null
}

async function confirmDelete() {
  const c = deleteTarget.value
  if (!c) return

  const id = getCharacterId(c)
  if (id == null) {
    error.value = 'Nie mogę usunąć postaci: brak ID.'
    closeDeleteModal()
    return
  }

  deleting.value = true
  error.value = null

  let success = false

  try {
    await axios.delete(`${API_URL}/api/character/deletecharacter/${id}`)
    characters.value = characters.value.filter(x => getCharacterId(x) !== id)
    success = true
  } catch (e: any) {
    error.value =
      e?.response?.data?.message
      ?? `Nie udało się usunąć postaci (HTTP ${e?.response?.status ?? '??'}).`
  } finally {
    deleting.value = false
    if (success) closeDeleteModal()
  }
}


onMounted(async () => {
  await Promise.all([fetchDictionaries(), fetchCharacters()])
})


</script>