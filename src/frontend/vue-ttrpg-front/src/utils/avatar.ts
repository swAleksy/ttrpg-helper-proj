/**
 * Avatar Utilities
 * Helper functions for avatar URL resolution and generation
 */
import { API_URL } from '@/config/api'

/**
 * Resolves avatar URL - handles relative paths and fallback to generated avatar
 */
export function resolveAvatarUrl(avatarUrl?: string | null, fallbackName = 'User'): string {
  if (avatarUrl) {
    // If already absolute URL, return as-is
    if (avatarUrl.startsWith('http')) {
      return avatarUrl
    }

    // Convert relative path to absolute
    const baseUrl = API_URL.replace(/\/$/, '')
    const path = avatarUrl.replace(/^\//, '')
    return `${baseUrl}/${path}`
  }

  // Generate fallback avatar
  return generateUiAvatar(fallbackName)
}

/**
 * Generates a UI Avatar URL based on name
 */
export function generateUiAvatar(name: string): string {
  const bgColor = stringToColor(name)
  const encodedName = encodeURIComponent(name)
  return `https://ui-avatars.com/api/?name=${encodedName}&background=${bgColor}&color=fff&size=128`
}

/**
 * Generates a consistent color from a string (for avatar backgrounds)
 */
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
