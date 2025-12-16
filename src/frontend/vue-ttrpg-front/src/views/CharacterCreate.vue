<template>
  <div class="min-h-screen bg-slate-950 text-slate-100">
    <div class="mx-auto max-w-6xl px-4 py-8 space-y-8">
      <!-- Nagłówek -->
      <header class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold tracking-tight">
            Nowa postać
          </h1>
          <p class="mt-1 text-sm text-slate-400">
            Uzupełnij informacje o swojej postaci.
          </p>
        </div>
      </header>

      <!-- Układ formularza -->
      <div class="space-y-6">
        <!-- 1) Podstawowe informacje + Rasa obok siebie na szerokich -->
        <div class="grid gap-6 lg:grid-cols-2">
          <!-- Sekcja: podstawowe informacje -->
          <section class="rounded-2xl border border-slate-800 bg-slate-900/40 p-5 space-y-4 flex flex-col">
            <h2 class="text-lg font-semibold">
              Podstawowe informacje
            </h2>
            
            <div class="lg:flex-1 lg:flex lg:flex-col lg:justify-center">
              <div class="w-full flex flex-row gap-6 items-end lg:flex-col lg:items-stretch">
                <!-- Avatar -->
                <div class="flex flex-col items-start gap-3 lg:self-auto">
                  <div class="flex h-40 w-40 items-center justify-center rounded-2xl border border-slate-700 bg-slate-800/60 overflow-hidden">
                    <img
                      :src="mainAvatarSrc"
                      alt="Avatar postaci"
                      class="h-full w-full object-cover"
                      @error="handleMainAvatarError"
                    />
                  </div>
                </div>

                <!-- Nazwa postaci -->
                <div class="flex-1">
                  <div class="flex-1 w-full min-w-0 lg:flex-none lg:w-full space-y-2.5">
                    <label class="block text-sm font-medium text-slate-200">
                      Nazwa postaci
                    </label>
                    <input
                      v-model="characterName"
                      type="text"
                      class="w-full rounded-xl border border-slate-700 bg-slate-900/60 px-3 py-2 text-sm outline-none focus:border-emerald-400 focus:ring-1 focus:ring-emerald-500"
                      placeholder="np. Thranduil"
                    >
                  </div>
                </div>
              </div>
            </div>
          </section>

          <!-- Sekcja: Rasa -->
          <section class="rounded-2xl border border-slate-800 bg-slate-900/40 p-5 space-y-4">
            <h2 class="text-lg font-semibold">
              Rasa
            </h2>
            <p class="text-xs text-slate-500">
              Wybierz rasę swojej postaci. Kliknij aby zobaczyć szczegóły.
            </p>

            <div class="grid gap-2 grid-cols-1 sm:grid-cols-2">
              <button
                v-for="(race, idx) in races"
                :key="race.id"
                type="button"
                class="flex w-full items-center gap-3 rounded-xl border px-3 py-2 text-sm transition"
                :class="[
                  selectedRaceId === race.id
                    ? 'border-emerald-500 bg-emerald-500/10 ring-2 ring-emerald-500/40'
                    : 'border-slate-700 bg-slate-900/60 hover:border-emerald-400 hover:bg-slate-900',
                  (races.length % 2 === 1 && idx === races.length - 1) ? 'sm:col-span-2' : ''
                ]"
                @click="openRaceModal(race)"
              >
                <span class="font-medium">
                  {{ race.name }}
                </span>
              </button>
            </div>
          </section>
        </div>

        <!-- 2) Klasa -->
        <section class="rounded-2xl border border-slate-800 bg-slate-900/40 p-5 space-y-4">
          <h2 class="text-lg font-semibold">
            Klasa
          </h2>
          <p class="text-xs text-slate-500">
            Wybierz klasę postaci. Kliknij aby zobaczyć szczegóły.
          </p>

          <div class="grid gap-4 grid-cols-2 md:grid-cols-3 lg:grid-cols-3 xl:grid-cols-4">
            <button
              v-for="charClass in classes"
              :key="charClass.id"
              type="button"
              class="group flex flex-col rounded-2xl border bg-slate-900/60 p-3 text-left transition hover:bg-slate-900"
              :class="selectedClassId === charClass.id
                ? 'border-emerald-500 ring-2 ring-emerald-500/40'
                : 'border-slate-700 hover:border-emerald-400'"
              @click="openClassModal(charClass)"
            >
              <!-- Avatar klasy -->
              <div class="w-full mb-2 flex justify-center">
                <div
                  class="aspect-square w-16 sm:w-20 md:w-24 flex items-center justify-center rounded-xl
                        bg-gradient-to-br from-emerald-700/80 to-slate-900 text-[11px] text-slate-50"
                >
                  <img
                    :src="getClassAvatarUrl(charClass)"
                    :alt="`Avatar klasy ${charClass.name}`"
                    class="h-full w-full object-cover"
                    @error="handleClassAvatarError"
                  />
                </div>
              </div>

              <p class="text-sm font-medium text-center mt-1">
                {{ charClass.name }}
              </p>
            </button>
          </div>
        </section>

        <!-- 3) Zdolności / Atrybuty (Ability Scores) -->
        <section class="rounded-2xl border border-slate-800 bg-slate-900/40 p-5 space-y-4">
          <div class="flex items-start justify-between gap-4">
            <div>
              <h2 class="text-lg font-semibold">Zdolności (Atrybuty)</h2>
              <p class="text-xs text-slate-500">
                Standard array: przypisz wartości 15, 14, 13, 12, 10, 8 do atrybutów (każda tylko raz).
              </p>
            </div>

            <button
              type="button"
              class="shrink-0 rounded-xl border border-slate-600 px-3 py-1.5 text-sm text-slate-200 hover:bg-slate-800"
              @click="resetStandardArray"
            >
              Resetuj
            </button>
          </div>

          <!-- Pula -->
          <div class="flex flex-wrap items-center gap-2">
            <span class="text-xs text-slate-400 mr-1">
              Pozostało: {{ assignedCount }}/6
            </span>

            <span
              v-for="v in STANDARD_ARRAY"
              :key="v"
              class="rounded-full border px-2.5 py-1 text-xs tabular-nums"
              :class="usedValues.has(v)
                ? 'border-slate-700 text-slate-500 line-through'
                : 'border-emerald-600/50 text-slate-200 bg-emerald-500/10'"
            >
              {{ v }}
            </span>
          </div>

          <!-- Kafelki atrybutów -->
          <div class="grid gap-3 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
            <div
              v-for="stat in abilityStats"
              :key="stat.key"
              class="rounded-xl border border-slate-700 bg-slate-900/60 px-3 py-2"
            >
              <div class="flex items-center justify-between">
                <label class="text-sm font-medium text-slate-200">
                  {{ stat.label }}
                </label>

                <span class="text-xs text-slate-400 tabular-nums">
                  {{ abilityAssign[stat.key] != null ? formatMod(abilityAssign[stat.key]!) : '—' }}
                </span>
              </div>

              <select
                v-model="abilityAssign[stat.key]"
                class="mt-2 w-full rounded-lg border border-slate-700 bg-slate-950/80 px-2 py-1.5 text-sm outline-none focus:border-emerald-400 focus:ring-1 focus:ring-emerald-500"
              >
                <option :value="null" disabled>Wybierz wartość…</option>

                <option
                  v-for="v in STANDARD_ARRAY"
                  :key="v"
                  :value="v"
                  :disabled="usedValues.has(v) && abilityAssign[stat.key] !== v"
                >
                  {{ v }}
                </option>
              </select>
            </div>
          </div>

          <p v-if="assignedCount < 6" class="text-xs text-slate-400">
            Przypisz wszystkie wartości, żeby przejść dalej.
          </p>

          <div v-else class="inline-flex items-center gap-2 rounded-full border border-emerald-500/40 bg-emerald-500/10 px-3 py-1 text-xs text-emerald-200">
            Gotowe <span aria-hidden="true">✓</span>
          </div>
        </section>


        <!-- 4) Tło -->
        <section class="rounded-2xl border border-slate-800 bg-slate-900/40 p-5 space-y-4">
          <h2 class="text-lg font-semibold">
            Tło postaci
          </h2>
          <p class="text-xs text-slate-500">
            Wybierz tło postaci. Kliknij aby zobaczyć szczegóły.
          </p>

          <div class="grid gap-2 grid-cols-1 sm:grid-cols-2">
            <button
              v-for="(bg, idx) in backgrounds"
              :key="bg.id"
              type="button"
              class="flex w-full items-center gap-3 rounded-xl border px-3 py-2 text-sm transition"
              :class="[
                selectedBackgroundId === bg.id
                  ? 'border-emerald-500 bg-emerald-500/10 ring-2 ring-emerald-500/40'
                  : 'border-slate-700 bg-slate-900/60 hover:border-emerald-400 hover:bg-slate-900',
                (backgrounds.length % 2 === 1 && idx === backgrounds.length - 1) ? 'sm:col-span-2' : ''
              ]"
              @click="openBackgroundModal(bg)"
            >
              <span class="font-medium">
                {{ bg.name }}
              </span>
            </button>
          </div>
        </section>

        <!-- 5) Umiejętności -->
        <section class="space-y-4 rounded-2xl border border-slate-800 bg-slate-900/40 p-5">
          <div class="flex items-center justify-between">
            <h2 class="text-lg font-semibold">
              Umiejętności
            </h2>
          </div>

          <div class="grid gap-2 grid-cols-1 sm:grid-cols-2 xl:grid-cols-3">
            <div
              v-for="skill in skills"
              :key="skill.id"
              class="rounded-xl border px-3 py-2 text-sm transition"
              :class="(skillValues[skill.id] ?? 0) > 0
                ? 'border-emerald-500 bg-emerald-500/10'
                : 'border-slate-700 bg-slate-900/60 hover:border-emerald-400 hover:bg-slate-900'"
            >
              <div class="flex items-center justify-between gap-3">
                <button
                  type="button"
                  class="flex-1 text-left"
                  @click="toggleSkillDescription(skill.id)"
                >
                  {{ skill.name }}
                </button>

                <div class="flex items-center gap-1">
                  <button
                    type="button"
                    class="h-7 w-7 rounded-lg border border-slate-600 text-sm leading-none hover:border-emerald-400"
                    @click.stop="changeSkillValue(skill.id, -1)"
                  >
                    −
                  </button>

                  <input
                    v-model.number="skillValues[skill.id]"
                    type="number"
                    min="0"
                    class="w-12 rounded-lg border border-slate-700 bg-slate-950/80 px-1 py-1 text-center text-sm outline-none focus:border-emerald-400 focus:ring-1 focus:ring-emerald-500"
                    @change="clampSkillValue(skill.id)"
                  >

                  <button
                    type="button"
                    class="h-7 w-7 rounded-lg border border-slate-600 text-sm leading-none hover:border-emerald-400"
                    @click.stop="changeSkillValue(skill.id, +1)"
                  >
                    +
                  </button>
                </div>
              </div>

              <p
                v-if="expandedSkillIds.has(skill.id) && skill.description"
                class="mt-2 text-xs text-slate-300 leading-snug"
              >
                {{ skill.description }}
              </p>
            </div>
          </div>
        </section>
      </div>



      <!-- Przycisk zapisu na dole strony -->
      <div class="mt-8 flex justify-end">
        <button
          type="button"
          :disabled="submitting"
          class="bg-emerald-500/90 border-emerald-400 text-slate-950 hover:bg-emerald-400
                px-10 py-3 text-base md:text-lg font-semibold rounded-2xl shadow-lg shadow-emerald-500/20
                disabled:opacity-60 disabled:cursor-not-allowed"
          @click="handleSubmit"
        >
          {{ submitting ? 'Zapisywanie…' : 'Zapisz postać' }}
        </button>
      </div>
    
      <p v-if="submitError" class="mt-3 text-sm text-rose-300">
        {{ submitError }}
      </p>


      <!-- Modal szczegółów rasy -->
      <div
        v-if="isRaceModalOpen && racePreview"
        class="fixed inset-0 z-40 flex items-center justify-center bg-black/60 px-4"
      >
        <div
          class="w-full max-w-lg rounded-2xl border border-slate-700 bg-slate-950/95 shadow-2xl"
          @click.stop
        >
          <!-- nagłówek z X -->
          <div class="flex items-center justify-between border-b border-slate-800 px-4 py-3">
            <h3 class="text-base font-semibold">
              Szczegóły rasy
            </h3>
            <button
              type="button"
              class="rounded-full p-1 text-slate-400 hover:bg-slate-800 hover:text-slate-100"
              @click="closeRaceModal"
            >
              ✕
            </button>
          </div>

          <!-- treść -->
          <div class="px-4 py-4 space-y-2">
            <h4 class="text-lg font-semibold">
              {{ racePreview.name }}
            </h4>

            <p class="text-sm text-slate-200">
              {{ racePreview.description }}
            </p>
          </div>

          <!-- przyciski na dole -->
          <div class="flex justify-end gap-3 border-t border-slate-800 px-4 py-3">
            <button
              type="button"
              class="rounded-xl border border-slate-600 px-3 py-1.5 text-sm text-slate-200 hover:bg-slate-800"
              @click="closeRaceModal"
            >
              Anuluj
            </button>
            <button
              type="button"
              class="rounded-xl border border-emerald-400 bg-emerald-500/90 px-3 py-1.5 text-sm font-medium text-slate-950 hover:bg-emerald-400"
              @click="confirmRaceSelection"
            >
              Wybierz rasę
            </button>
          </div>
        </div>
      </div>

      <!-- Modal szczegółów klasy -->
      <div
        v-if="isClassModalOpen && classPreview"
        class="fixed inset-0 z-40 flex items-center justify-center bg-black/60 px-4"
      >
        <div
          class="w-full max-w-lg rounded-2xl border border-slate-700 bg-slate-950/95 shadow-2xl"
          @click.stop
        >
          <!-- nagłówek z X -->
          <div class="flex items-center justify-between border-b border-slate-800 px-4 py-3">
            <h3 class="text-base font-semibold">
              Szczegóły klasy
            </h3>
            <button
              type="button"
              class="rounded-full p-1 text-slate-400 hover:bg-slate-800 hover:text-slate-100"
              @click="closeClassModal"
            >
              ✕
            </button>
          </div>

          <!-- treść -->
          <div class="flex gap-4 px-4 py-4">
            <!-- obraz -->
            <div
              class="h-24 w-24 flex-shrink-0 rounded-xl bg-gradient-to-br from-emerald-700/80 to-slate-900 flex items-center justify-center text-xs text-slate-50"
            >
              <img
                :src="getClassAvatarUrl(classPreview)"
                :alt="`Avatar klasy ${classPreview.name}`"
                class="h-full w-full object-cover"
                @error="handleClassAvatarError"
              />
            </div>

            <div class="space-y-2">
              <h4 class="text-lg font-semibold">
                {{ classPreview.name }}
              </h4>
              
              <p class="text-sm text-slate-200">
                {{ classPreview.description }}
              </p>
            </div>
          </div>

          <!-- przyciski na dole -->
          <div class="flex justify-end gap-3 border-t border-slate-800 px-4 py-3">
            <button
              type="button"
              class="rounded-xl border border-slate-600 px-3 py-1.5 text-sm text-slate-200 hover:bg-slate-800"
              @click="closeClassModal"
            >
              Anuluj
            </button>
            <button
              type="button"
              class="rounded-xl border border-emerald-400 bg-emerald-500/90 px-3 py-1.5 text-sm font-medium text-slate-950 hover:bg-emerald-400"
              @click="confirmClassSelection"
            >
              Wybierz klasę
            </button>
          </div>
        </div>
      </div>

      <!-- Modal szczegółów tła -->
      <div
        v-if="isBackgroundModalOpen && backgroundPreview"
        class="fixed inset-0 z-40 flex items-center justify-center bg-black/60 px-4"
      >
        <div
          class="w-full max-w-lg rounded-2xl border border-slate-700 bg-slate-950/95 shadow-2xl"
          @click.stop
        >
          <!-- nagłówek z X -->
          <div class="flex items-center justify-between border-b border-slate-800 px-4 py-3">
            <h3 class="text-base font-semibold">
              Szczegóły tła
            </h3>
            <button
              type="button"
              class="rounded-full p-1 text-slate-400 hover:bg-slate-800 hover:text-slate-100"
              @click="closeBackgroundModal"
            >
              ✕
            </button>
          </div>

          <!-- treść -->
          <div class="px-4 py-4 space-y-2">
            <h4 class="text-lg font-semibold">
              {{ backgroundPreview.name }}
            </h4>
            
            <p class="text-sm text-slate-200">
              {{ backgroundPreview.description }}
            </p>
          </div>

          <!-- przyciski na dole -->
          <div class="flex justify-end gap-3 border-t border-slate-800 px-4 py-3">
            <button
              type="button"
              class="rounded-xl border border-slate-600 px-3 py-1.5 text-sm text-slate-200 hover:bg-slate-800"
              @click="closeBackgroundModal"
            >
              Anuluj
            </button>
            <button
              type="button"
              class="rounded-xl border border-emerald-400 bg-emerald-500/90 px-3 py-1.5 text-sm font-medium text-slate-950 hover:bg-emerald-400"
              @click="confirmBackgroundSelection"
            >
              Wybierz tło
            </button>
          </div>
        </div>
      </div>



    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import axios from 'axios'

import defaultAvatar from '@/assets/img/DefaultCharacterAvatar.png'

const API_URL = (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:8080'

// usuwanie ewentualnego "/api" na końcu
const BACKEND_ORIGIN = API_URL.replace(/\/api\/?$/, '')

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

function getClassAvatarUrl(charClass: CharacterClass): string {
  const fileName = CLASS_AVATAR_MAP[charClass.name]
  if (!fileName) {
    // gdyby brakowało mapowania –> użyj domyślnego avatara z frontu
    return defaultAvatar
  }
  return `${BACKEND_ORIGIN}/characterAvatars/${fileName}`
}

// fallback, gdyby obraz się nie załadował
function handleClassAvatarError(event: Event) {
  const target = event.target as HTMLImageElement | null
  if (target) {
    target.src = defaultAvatar
  }
}

const mainAvatarBroken = ref(false)

const mainAvatarSrc = computed(() => {
  // jeśli kiedyś obraz nie zadziałał, trzymaj fallback
  if (mainAvatarBroken.value) return defaultAvatar

  // jeśli nie wybrano klasy -> default
  if (!selectedClassId.value) return defaultAvatar

  const cls = classes.value.find(c => c.id === selectedClassId.value)
  return cls ? getClassAvatarUrl(cls) : defaultAvatar
})

function handleMainAvatarError(event: Event) {
  mainAvatarBroken.value = true
  const target = event.target as HTMLImageElement | null
  if (target) target.src = defaultAvatar
}



interface BaseEntity {
  id: number
  name: string
  description: string
}

type Race = BaseEntity
type CharacterClass = BaseEntity
type Background = BaseEntity
type Skill = BaseEntity

// --- state formularza ---
const characterName = ref('')

type AbilityKey = 'STR' | 'DEX' | 'CON' | 'INT' | 'WIS' | 'CHA'

const abilityStats = [
  { key: 'STR', label: 'Siła (STR)' },
  { key: 'DEX', label: 'Zręczność (DEX)' },
  { key: 'CON', label: 'Kondycja (CON)' },
  { key: 'INT', label: 'Inteligencja (INT)' },
  { key: 'WIS', label: 'Mądrość (WIS)' },
  { key: 'CHA', label: 'Charyzma (CHA)' },
] as const

const STANDARD_ARRAY = [15, 14, 13, 12, 10, 8] as const

// tu trzymany wybór usera -> jaka wartość jest przypisana do jakiego atrybutu
const abilityAssign = ref<Record<AbilityKey, number | null>>({
  STR: null,
  DEX: null,
  CON: null,
  INT: null,
  WIS: null,
  CHA: null,
})

const usedValues = computed(() => {
  const vals = Object.values(abilityAssign.value).filter((v): v is number => v != null)
  return new Set(vals)
})

const assignedCount = computed(() => Object.values(abilityAssign.value).filter(v => v != null).length)

function abilityMod(score: number) {
  const s = Number.isFinite(score) ? score : 0
  return Math.floor((s - 10) / 2)
}

function formatMod(score: number) {
  const m = abilityMod(score)
  return m >= 0 ? `+${m}` : `${m}`
}

function resetStandardArray() {
  abilityAssign.value = { STR: null, DEX: null, CON: null, INT: null, WIS: null, CHA: null }
}


const races = ref<Race[]>([])
const classes = ref<CharacterClass[]>([])
const backgrounds = ref<Background[]>([])
const skills = ref<Skill[]>([])

const selectedRaceId = ref<number | null>(null)
const selectedClassId = ref<number | null>(null)
const selectedBackgroundId = ref<number | null>(null)

// tylko do rozwijania opisów
const expandedSkillIds = ref<Set<number>>(new Set())

// wartości umiejętności
const skillValues = ref<Record<number, number>>({})

const selectedSkillIds = computed<number[]>(() => {
  return Object.entries(skillValues.value)
    .filter(([, v]) => (v ?? 0) > 0)
    .map(([id]) => Number(id))
})

function toggleSkillDescription(id: number) {
  const set = expandedSkillIds.value
  if (set.has(id)) set.delete(id)
  else set.add(id)
}

function clampSkillValue(id: number) {
  const current = Number(skillValues.value[id] ?? 0)
  const safe = Number.isFinite(current) ? Math.max(0, Math.trunc(current)) : 0
  skillValues.value[id] = safe
}

function changeSkillValue(id: number, delta: number) {
  const current = skillValues.value[id] ?? 0
  const next = Math.max(0, Math.trunc(current + delta))
  skillValues.value[id] = next
}

const submitting = ref(false)
const submitError = ref<string | null>(null)

async function handleSubmit() {
  submitError.value = null

  // część walidacji ------- do dokończenia
  if (!characterName.value.trim()) {
    submitError.value = 'Podaj nazwę postaci.'
    return
  }
  if (!selectedRaceId.value || !selectedClassId.value || !selectedBackgroundId.value) {
    submitError.value = 'Wybierz rasę, klasę i tło.'
    return
  }
  if (assignedCount.value < 6) {
    submitError.value = 'Przypisz wszystkie wartości atrybutów.'
    return
  }


  // payload zgodny z /api/character
  const payload = {
    name: characterName.value.trim(),
    raceId: selectedRaceId.value,
    classId: selectedClassId.value,
    backgroundId: selectedBackgroundId.value,
    level: 1,

    strength: abilityAssign.value.STR!,
    dexterity: abilityAssign.value.DEX!,
    constitution: abilityAssign.value.CON!,
    intelligence: abilityAssign.value.INT!,
    wisdom: abilityAssign.value.WIS!,
    charisma: abilityAssign.value.CHA!,

    characterSkillsIds: selectedSkillIds.value, // int[]
  }

  submitting.value = true
  try {
    const res = await axios.post(`${API_URL}/api/character`, payload)
    console.log('Utworzono postać:', res.data)

    alert('Postać zapisana')
    // opcjonalnie: router.push('/dashboard') albo do widoku postaci
  } catch (e: any) {
    console.error(e)
    submitError.value =
      e?.response?.data?.message
      ?? `Nie udało się zapisać postaci (HTTP ${e?.response?.status ?? '??'}).`
  } finally {
    submitting.value = false
  }
}


// --- pobieranie danych z backendu ---
async function fetchRaces() {
  const res = await axios.get<Race[]>(`${API_URL}/api/character/races`)
  races.value = res.data
}

async function fetchClasses() {
  const res = await axios.get<CharacterClass[]>(`${API_URL}/api/character/classes`)
  classes.value = res.data
}

async function fetchBackgrounds() {
  const res = await axios.get<Background[]>(`${API_URL}/api/character/backgrounds`)
  backgrounds.value = res.data
}

async function fetchSkills() {
  const res = await axios.get<Skill[]>(`${API_URL}/api/character/skills`)
  skills.value = res.data

  // domyślnie wszystkie mają 0
  const initial: Record<number, number> = {}
  for (const s of skills.value) {
    initial[s.id] = 0
  }
  skillValues.value = initial
}

// okno ze szczegółami (rasa)

const isRaceModalOpen = ref(false)
const racePreview = ref<Race | null>(null)

function openRaceModal(race: Race) {
  racePreview.value = race
  isRaceModalOpen.value = true
}

function closeRaceModal() {
  isRaceModalOpen.value = false
  racePreview.value = null
}

function confirmRaceSelection() {
  if (racePreview.value) {
    selectedRaceId.value = racePreview.value.id
    mainAvatarBroken.value = false
  }
  isRaceModalOpen.value = false
}


// okno ze szczegółami (klasa)

const isClassModalOpen = ref(false)
const classPreview = ref<CharacterClass | null>(null)

function openClassModal(charClass: CharacterClass) {
  classPreview.value = charClass
  isClassModalOpen.value = true
}

function closeClassModal() {
  isClassModalOpen.value = false
  classPreview.value = null
}

function confirmClassSelection() {
  if (classPreview.value) {
    selectedClassId.value = classPreview.value.id
  }
  isClassModalOpen.value = false
}


// okno ze szczegółami (tło)

const isBackgroundModalOpen = ref(false)
const backgroundPreview = ref<Background | null>(null)

function openBackgroundModal(bg: Background) {
  backgroundPreview.value = bg
  isBackgroundModalOpen.value = true
}

function closeBackgroundModal() {
  isBackgroundModalOpen.value = false
  backgroundPreview.value = null
}

function confirmBackgroundSelection() {
  if (backgroundPreview.value) {
    selectedBackgroundId.value = backgroundPreview.value.id
  }
  isBackgroundModalOpen.value = false
}




onMounted(async () => {
  await Promise.all([
    fetchRaces(),
    fetchClasses(),
    fetchBackgrounds(),
    fetchSkills(),
  ])
})
</script>
