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
        <button
          type="button"
          class="rounded-xl border border-slate-600 px-3 py-2 text-sm text-slate-200 hover:bg-slate-800"
          @click="router.push('/characters/all')"
        >
          ← Wróć do listy
        </button>
      </header>

      <!-- (opcjonalnie) błąd z backendu -->
      <div
        v-if="submitError"
        ref="errorBanner"
        class="scroll-mt-24 rounded-2xl border border-rose-600/40 bg-rose-500/10 px-4 py-3 text-sm text-rose-200"
        :class="bannerFlash ? 'ring-2 ring-rose-500/40' : ''"
      >
        {{ submitError }}
      </div>

      <!-- Układ formularza -->
      <div class="space-y-6">
        <!-- 1) Podstawowe informacje + Rasa obok siebie na szerokich -->
        <div class="grid gap-6 lg:grid-cols-2">
          <!-- Sekcja: podstawowe informacje -->
          <section
            ref="sectionBasic"
            class="scroll-mt-24 rounded-2xl border bg-slate-900/40 p-5 space-y-4 flex flex-col transition"
            :class="[
              flashKey === 'basic'
                ? 'border-rose-500/80 ring-2 ring-rose-500/35 bg-rose-500/5'
                : 'border-slate-800'
            ]"
          >
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

                <!-- Nazwa postaci + Level -->
                <div class="flex-1">
                  <div class="flex gap-4 items-end">
                    <!-- Nazwa postaci -->
                    <div class="flex-1 min-w-0 space-y-2.5">
                      <label class="block text-sm font-medium text-slate-200">
                        Nazwa postaci
                      </label>
                      <input
                        ref="nameInput"
                        v-model="characterName"
                        type="text"
                        class="w-full rounded-xl border bg-slate-900/60 px-3 py-2 text-sm outline-none focus:ring-1 transition"
                        :class="flashKey === 'basic'
                          ? 'border-rose-500/70 focus:border-rose-400 focus:ring-rose-500'
                          : 'border-slate-700 focus:border-emerald-400 focus:ring-emerald-500'"
                        placeholder="np. Thranduil"
                      >
                    </div>

                    <!-- Level -->
                    <div class="w-28 space-y-2.5">
                      <label class="block text-sm font-medium text-slate-200">
                        Level
                      </label>
                      <input
                        v-model.number="level"
                        type="number"
                        min="1"
                        max="20"
                        step="1"
                        class="w-full rounded-xl border border-slate-700 bg-slate-900/60 px-3 py-2 text-sm outline-none focus:border-emerald-400 focus:ring-1 focus:ring-emerald-500 transition"
                        @blur="normalizeLevel"
                        @change="normalizeLevel"
                      >
                    </div>
                  </div>
                </div>

              </div>
            </div>
          </section>

          <!-- Sekcja: Rasa -->
          <section
            ref="sectionRace"
            class="scroll-mt-24 rounded-2xl border bg-slate-900/40 p-5 space-y-4 transition"
            :class="[
              flashKey === 'race'
                ? 'border-rose-500/80 ring-2 ring-rose-500/35 bg-rose-500/5'
                : 'border-slate-800'
            ]"
          >
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
        <section
          ref="sectionClass"
          class="scroll-mt-24 rounded-2xl border bg-slate-900/40 p-5 space-y-4 transition"
          :class="[
            flashKey === 'class'
              ? 'border-rose-500/80 ring-2 ring-rose-500/35 bg-rose-500/5'
              : 'border-slate-800'
          ]"
        >
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
        <section
          ref="sectionAbilities"
          class="scroll-mt-24 rounded-2xl border bg-slate-900/40 p-5 space-y-4 transition"
          :class="[
            flashKey === 'abilities'
              ? 'border-rose-500/80 ring-2 ring-rose-500/35 bg-rose-500/5'
              : 'border-slate-800'
          ]"
        >
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
        <section
          ref="sectionBackground"
          class="scroll-mt-24 rounded-2xl border bg-slate-900/40 p-5 space-y-4 transition"
          :class="[
            flashKey === 'background'
              ? 'border-rose-500/80 ring-2 ring-rose-500/35 bg-rose-500/5'
              : 'border-slate-800'
          ]"
        >
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

        <!-- 5) Umiejętności (z biegłościami) -->
        <section
          ref="sectionSkills"
          class="scroll-mt-24 space-y-4 rounded-2xl border bg-slate-900/40 p-5 transition"
          :class="[
            flashKey === 'skills'
              ? 'border-rose-500/80 ring-2 ring-rose-500/35 bg-rose-500/5'
              : 'border-slate-800'
          ]"
        >
          <div class="flex items-center justify-between gap-3">
            <div>
              <h2 class="text-lg font-semibold">Umiejętności</h2>
              <p class="text-xs text-slate-500">
                Wybierz biegłości (proficient) dla umiejętności.
              </p>
            </div>

            <div class="text-xs text-slate-300 tabular-nums">
              Proficiency bonus: <span class="font-semibold text-slate-100">+{{ proficiencyBonus }}</span>
            </div>
          </div>

          <!-- INFO: co jest potrzebne -->
          <div class="rounded-xl border border-slate-800 bg-slate-950/40 px-3 py-2 text-xs text-slate-300">
            <div v-if="!selectedBackgroundId || !selectedClassId" class="text-slate-400">
              Wybierz <span class="text-slate-200">tło</span> i <span class="text-slate-200">klasę</span>, aby odblokować wybór biegłości.
            </div>

            <div v-else class="flex flex-wrap items-center gap-x-4 gap-y-1">
              <span>
                Tło przyznaje: <span class="text-slate-100 font-medium">{{ backgroundSkillIds.size }}</span> biegłości
              </span>
              <span>
                Klasa: wybierz <span class="text-slate-100 font-medium">{{ classPickRequired }}</span>
                (pozostało <span class="text-slate-100 font-medium">{{ classPickRemaining }}</span>)
              </span>

              <span v-if="classPickRemaining === 0" class="text-emerald-200">
                Gotowe ✓
              </span>
            </div>
          </div>

          <!-- A) Biegłości z tła (readonly) -->
          <div class="space-y-2">
            <h3 class="text-sm font-semibold text-slate-200">1) Z tła (automatycznie)</h3>

            <div v-if="!selectedBackgroundId" class="text-xs text-slate-500">
              Najpierw wybierz tło.
            </div>

            <div v-else class="grid gap-2 grid-cols-1 sm:grid-cols-2 xl:grid-cols-3">
              <div
                v-for="skill in backgroundSkillsResolved"
                :key="skill.id"
                class="rounded-xl border border-emerald-500/40 bg-emerald-500/10 px-3 py-2 text-sm"
              >
                <div class="flex items-center justify-between gap-3">
                  <button type="button" class="flex-1 text-left" @click="toggleSkillDescription(skill.id)">
                    <span class="font-medium">{{ skill.name }}</span>
                    <span class="ml-2 text-xs text-slate-300">({{ skillAbilityLabel(skill.name) }})</span>
                  </button>

                  <div class="flex items-center gap-2">
                    <span class="text-xs text-emerald-200 rounded-full border border-emerald-500/40 px-2 py-0.5">
                      Proficient
                    </span>
                    <span class="text-xs text-slate-100 tabular-nums">
                      {{ formatSkillBonus(skill.name, true) }}
                    </span>
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
          </div>

          <!-- B) Biegłości z klasy (wybór z limitem) -->
          <div class="space-y-2">
            <h3 class="text-sm font-semibold text-slate-200">2) Z klasy (wybór)</h3>

            <div v-if="!selectedClassId" class="text-xs text-slate-500">
              Najpierw wybierz klasę.
            </div>

            <div v-else class="grid gap-2 grid-cols-1 sm:grid-cols-2 xl:grid-cols-3">
              <div
                v-for="skill in classSkillsResolved"
                :key="skill.id"
                class="rounded-xl border px-3 py-2 text-sm transition"
                :class="isProficient(skill.id)
                  ? 'border-emerald-500 bg-emerald-500/10'
                  : 'border-slate-700 bg-slate-900/60 hover:border-emerald-400 hover:bg-slate-900'"
              >
                <div class="flex items-center justify-between gap-3">
                  <button type="button" class="flex-1 text-left" @click="toggleSkillDescription(skill.id)">
                    <span class="font-medium">{{ skill.name }}</span>
                    <span class="ml-2 text-xs text-slate-400">({{ skillAbilityLabel(skill.name) }})</span>
                  </button>

                  <div class="flex items-center gap-2">
                    <button
                      type="button"
                      class="rounded-lg border px-2 py-1 text-xs"
                      :disabled="isBackgroundGranted(skill.id) || isClassPickDisabled(skill.id)"
                      :class="isBackgroundGranted(skill.id)
                        ? 'border-slate-700 text-slate-500 cursor-not-allowed'
                        : 'border-slate-600 text-slate-200 hover:border-emerald-400'"
                      @click.stop="toggleClassPick(skill.id)"
                    >
                      {{ classPickButtonLabel(skill.id) }}
                    </button>

                    <span class="text-xs text-slate-100 tabular-nums">
                      {{ formatSkillBonus(skill.name, isProficient(skill.id)) }}
                    </span>
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

            <p v-if="selectedClassId && classPickRemaining > 0" class="text-xs text-slate-400">
              Wybierz jeszcze {{ classPickRemaining }} umiejętności z klasy.
            </p>
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
import { useRouter } from 'vue-router'
import axios from 'axios'

import defaultAvatar from '@/assets/img/DefaultCharacterAvatar.png'
import { SKILL_ABILITY, BACKGROUND_SKILLS, CLASS_SKILLS, type SkillName } from "@/config/charactersSkillsConfig"

const API_URL = (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:8080'
const router = useRouter()

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

interface BaseEntity {
  id: number
  name: string
  description: string
}

type Race = BaseEntity
type CharacterClass = BaseEntity
type Background = BaseEntity
type Skill = BaseEntity

function getClassAvatarUrl(charClass: CharacterClass): string {
  const fileName = CLASS_AVATAR_MAP[charClass.name]
  if (!fileName) return defaultAvatar
  return `${BACKEND_ORIGIN}/characterAvatars/${fileName}`
}

// fallback, gdyby obraz się nie załadował
function handleClassAvatarError(event: Event) {
  const target = event.target as HTMLImageElement | null
  if (target) target.src = defaultAvatar
}

const mainAvatarBroken = ref(false)

const races = ref<Race[]>([])
const classes = ref<CharacterClass[]>([])
const backgrounds = ref<Background[]>([])
const skills = ref<Skill[]>([])

const selectedRaceId = ref<number | null>(null)
const selectedClassId = ref<number | null>(null)
const selectedBackgroundId = ref<number | null>(null)

const mainAvatarSrc = computed(() => {
  if (mainAvatarBroken.value) return defaultAvatar
  if (!selectedClassId.value) return defaultAvatar
  const cls = classes.value.find(c => c.id === selectedClassId.value)
  return cls ? getClassAvatarUrl(cls) : defaultAvatar
})

function handleMainAvatarError(event: Event) {
  mainAvatarBroken.value = true
  const target = event.target as HTMLImageElement | null
  if (target) target.src = defaultAvatar
}

// ---------------------
// refs do scrollowania + flash
// ---------------------
const sectionBasic = ref<HTMLElement | null>(null)
const sectionRace = ref<HTMLElement | null>(null)
const sectionClass = ref<HTMLElement | null>(null)
const sectionAbilities = ref<HTMLElement | null>(null)
const sectionBackground = ref<HTMLElement | null>(null)
const sectionSkills = ref<HTMLElement | null>(null)

const nameInput = ref<HTMLInputElement | null>(null)

const errorBanner = ref<HTMLElement | null>(null)
const bannerFlash = ref(false)

type FlashKey = 'basic' | 'race' | 'class' | 'abilities' | 'background' | 'skills' | null
const flashKey = ref<FlashKey>(null)
let flashToken = 0

function scrollTo(el: HTMLElement | null) {
  if (!el) return
  el.scrollIntoView({ behavior: 'smooth', block: 'start' })
}

function flashSection(key: Exclude<FlashKey, null>, el?: HTMLElement | null, focusEl?: HTMLElement | null) {
  flashToken += 1
  const myToken = flashToken

  flashKey.value = key
  if (el) scrollTo(el)

  // mały "focus" na input przy nazwie
  if (focusEl) {
    window.setTimeout(() => {
      focusEl.focus?.()
    }, 250)
  }

  window.setTimeout(() => {
    if (flashToken === myToken) flashKey.value = null
  }, 1200)
}

function flashBanner() {
  bannerFlash.value = true
  scrollTo(errorBanner.value)
  window.setTimeout(() => {
    bannerFlash.value = false
  }, 1200)
}

// --- state formularza ---
const characterName = ref('')

// LEVEL
const level = ref<number>(1)

function normalizeLevel() {
  if (!Number.isFinite(level.value)) {
    level.value = 1
    return
  }

  level.value = Math.round(level.value)

  if (level.value < 1) level.value = 1
  if (level.value > 20) level.value = 20
}

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

function skillAbilityLabel(skillName: string) {
  const ability = (SKILL_ABILITY as Record<string, AbilityKey | undefined>)[skillName]
  return ability ?? "—"
}

// PROF BONUS zależny od levelu
const proficiencyBonus = computed(() => {
  const lv = level.value
  if (lv >= 17) return 6
  if (lv >= 13) return 5
  if (lv >= 9) return 4
  if (lv >= 5) return 3
  return 2
})

function formatSkillBonus(skillName: string, proficient: boolean) {
  const ability = (SKILL_ABILITY as Record<string, AbilityKey | undefined>)[skillName]
  if (!ability) return "—"

  const score = abilityAssign.value[ability]
  if (score == null) return "—"

  const mod = abilityMod(score)
  const total = mod + (proficient ? proficiencyBonus.value : 0)
  return total >= 0 ? `+${total}` : `${total}`
}

function resetStandardArray() {
  abilityAssign.value = { STR: null, DEX: null, CON: null, INT: null, WIS: null, CHA: null }
}

// --- PROFI (biegłości w skillach) ---
const selectedBackgroundName = computed(() => {
  const id = selectedBackgroundId.value
  return id ? backgrounds.value.find(b => b.id === id)?.name ?? null : null
})

const selectedClassName = computed(() => {
  const id = selectedClassId.value
  return id ? classes.value.find(c => c.id === id)?.name ?? null : null
})

// A) skille przyznane z tła (po nazwach, z configu)
const backgroundSkillNames = computed<SkillName[]>(() => {
  const bgName = selectedBackgroundName.value
  if (!bgName) return []
  return (BACKGROUND_SKILLS[bgName] ?? []) as SkillName[]
})

// B) zasady klasy (ile i z jakiej listy)
const classRule = computed(() => {
  const clsName = selectedClassName.value
  if (!clsName) return null
  return CLASS_SKILLS[clsName] ?? null
})

const classPickRequired = computed(() => classRule.value?.pick ?? 0)

// mapowanie skillName -> skillId z backendu (po name!)
const skillIdByName = computed(() => {
  const map = new Map<string, number>()
  for (const s of skills.value) map.set(s.name, s.id)
  return map
})

// A) ids przyznane z tła
const backgroundSkillIds = computed<Set<number>>(() => {
  const set = new Set<number>()
  for (const n of backgroundSkillNames.value) {
    const id = skillIdByName.value.get(n)
    if (id != null) set.add(id)
  }
  return set
})

// rozwinięcie A: obiekty Skill dla tła
const backgroundSkillsResolved = computed<Skill[]>(() => {
  const ids = backgroundSkillIds.value
  return skills.value.filter(s => ids.has(s.id))
})

// B) lista opcji z klasy (po nazwach)
const classOptionNames = computed<SkillName[]>(() => {
  const rule = classRule.value
  if (!rule) return []
  return rule.options as SkillName[]
})

// B) ids opcji z klasy (bez tych z tła, bo nie chcemy duplikatów)
const classOptionIds = computed<number[]>(() => {
  const bgIds = backgroundSkillIds.value
  const out: number[] = []
  for (const n of classOptionNames.value) {
    const id = skillIdByName.value.get(n)
    if (id != null && !bgIds.has(id)) out.push(id)
  }
  return out
})

// B) obiekty Skill do renderu
const classSkillsResolved = computed<Skill[]>(() => {
  const set = new Set(classOptionIds.value)
  return skills.value.filter(s => set.has(s.id))
})

// wybory usera z klasy (ids)
const classPickedIds = ref<Set<number>>(new Set())

function resetClassPicks() {
  classPickedIds.value = new Set()
}

const classPickRemaining = computed(() => {
  const need = classPickRequired.value
  const have = classPickedIds.value.size
  return Math.max(0, need - have)
})

function isBackgroundGranted(skillId: number) {
  return backgroundSkillIds.value.has(skillId)
}

function isProficient(skillId: number) {
  return isBackgroundGranted(skillId) || classPickedIds.value.has(skillId)
}

function isClassPickDisabled(skillId: number) {
  if (classPickedIds.value.has(skillId)) return false
  return classPickedIds.value.size >= classPickRequired.value
}

function toggleClassPick(skillId: number) {
  if (isBackgroundGranted(skillId)) return
  const set = new Set(classPickedIds.value)

  if (set.has(skillId)) set.delete(skillId)
  else {
    if (set.size >= classPickRequired.value) return
    set.add(skillId)
  }
  classPickedIds.value = set
}

function classPickButtonLabel(skillId: number) {
  if (isBackgroundGranted(skillId)) return "Z tła"
  return classPickedIds.value.has(skillId) ? "Wybrane" : "Wybierz"
}

// finalne IDs do payloadu:
const proficientSkillIds = computed<number[]>(() => {
  const out = new Set<number>()
  for (const id of backgroundSkillIds.value) out.add(id)
  for (const id of classPickedIds.value) out.add(id)
  return [...out]
})

// do rozwijania opisów
const expandedSkillIds = ref<Set<number>>(new Set())

function toggleSkillDescription(id: number) {
  const set = new Set(expandedSkillIds.value)
  if (set.has(id)) set.delete(id)
  else set.add(id)
  expandedSkillIds.value = set
}

const submitting = ref(false)
const submitError = ref<string | null>(null)

// ---------------------
// NOWA WALIDACJA: scroll + chwilowy czerwony flash
// ---------------------
function validateAndFocusFirstError(): boolean {
  normalizeLevel()

  // 1) Nazwa
  if (!characterName.value.trim()) {
    flashSection('basic', sectionBasic.value, nameInput.value)
    return false
  }

  // 2) Rasa
  if (!selectedRaceId.value) {
    flashSection('race', sectionRace.value)
    return false
  }

  // 3) Klasa
  if (!selectedClassId.value) {
    flashSection('class', sectionClass.value)
    return false
  }

  // 4) Atrybuty
  if (assignedCount.value < 6) {
    flashSection('abilities', sectionAbilities.value)
    return false
  }

  // 5) Tło
  if (!selectedBackgroundId.value) {
    flashSection('background', sectionBackground.value)
    return false
  }

  // 6) Skille z klasy (limit)
  if (selectedClassId.value && classPickRemaining.value > 0) {
    flashSection('skills', sectionSkills.value)
    return false
  }

  return true
}

async function handleSubmit() {
  submitError.value = null

  // walidacja bez wypisywania tekstu pod buttonem
  if (!validateAndFocusFirstError()) return

  const payload = {
    name: characterName.value.trim(),
    raceId: selectedRaceId.value,
    classId: selectedClassId.value,
    backgroundId: selectedBackgroundId.value,
    level: level.value,

    strength: abilityAssign.value.STR!,
    dexterity: abilityAssign.value.DEX!,
    constitution: abilityAssign.value.CON!,
    intelligence: abilityAssign.value.INT!,
    wisdom: abilityAssign.value.WIS!,
    charisma: abilityAssign.value.CHA!,

    characterSkillsIds: proficientSkillIds.value,
  }

  submitting.value = true
  try {
    const res = await axios.post(`${API_URL}/api/character`, payload)
    console.log('Utworzono postać:', res.data)
    await router.push('/characters/all')
  } catch (e: any) {
    console.error(e)
    submitError.value =
      e?.response?.data?.message
      ?? `Nie udało się zapisać postaci (HTTP ${e?.response?.status ?? '??'}).`
    flashBanner()
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
    mainAvatarBroken.value = false
    resetClassPicks()
    expandedSkillIds.value = new Set()
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
    resetClassPicks()
    expandedSkillIds.value = new Set()
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
