<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  variant?: 'primary' | 'danger' | 'ghost' | 'icon'
  size?: 'sm' | 'md'
  disabled?: boolean
}>()

const baseClasses =
  'transition-colors rounded-lg font-medium flex items-center justify-center gap-2 focus:outline-none focus:ring-2 focus:ring-offset-1 focus:ring-offset-slate-900'

const variantClasses = computed(() => {
  switch (props.variant) {
    case 'danger':
      return 'bg-red-500/10 text-red-400 hover:bg-red-500/20 focus:ring-red-500'
    case 'ghost':
      return 'text-slate-400 hover:text-white hover:bg-slate-800 focus:ring-slate-500'
    case 'icon': // Dla przycisków typu "X" lub dzwonek
      return 'text-slate-400 hover:text-white p-1 rounded-full hover:bg-slate-800'
    case 'primary':
    default:
      return 'bg-emerald-600 text-white hover:bg-emerald-500 focus:ring-emerald-500 disabled:opacity-50 disabled:cursor-not-allowed'
  }
})

const sizeClasses = computed(() => {
  if (props.variant === 'icon') return ''
  return props.size === 'sm' ? 'px-2 py-1 text-xs' : 'px-3 py-2 text-sm'
})
</script>

<template>
  <button :class="[baseClasses, variantClasses, sizeClasses]" :disabled="disabled">
    <slot />
  </button>
</template>
