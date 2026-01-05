/**
 * Avatar Utilities
 * Helper functions for avatar URL resolution and generation
 */
import { API_URL } from '@/config/api'

export function resolveAvatarUrl(avatarUrl?: string | null, fallbackName = 'User'): string {
  if (avatarUrl) {
    if (avatarUrl.startsWith('http')) {
      return avatarUrl
    }

    const baseUrl = API_URL.replace(/\/$/, '')
    const path = avatarUrl.replace(/^\//, '')
    return `${baseUrl}/${path}`
  }

  return generateUiAvatar(fallbackName)
}

export function generateUiAvatar(name: string): string {
  const bgColor = stringToColor(name)
  const encodedName = encodeURIComponent(name)
  return `https://ui-avatars.com/api/?name=${encodedName}&background=${bgColor}&color=fff&size=128`
}

function stringToColor(str: string): string {
  let hash = 0
  for (let i = 0; i < str.length; i++) {
    hash = str.charCodeAt(i) + ((hash << 5) - hash)
  }

  let color = ''
  for (let i = 0; i < 3; i++) {
    const value = (hash >> (i * 8)) & 0xff
    color += ('00' + value.toString(16)).slice(-2)
  }

  return color
}
