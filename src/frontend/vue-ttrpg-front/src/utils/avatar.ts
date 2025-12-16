import { API_URL } from '@/stores/auth'

export function resolveAvatarUrl(avatarUrl?: string | null, fallbackName = 'User') {
  if (avatarUrl) {
    if (avatarUrl.startsWith('http')) return avatarUrl

    const baseUrl = API_URL.replace(/\/$/, '')
    const path = avatarUrl.replace(/^\//, '')
    return `${baseUrl}/${path}`
  }

  return generateUiAvatar(fallbackName)
}

export function generateUiAvatar(name: string) {
  const bgColor = stringToColor(name)
  const encodedName = encodeURIComponent(name)

  return `https://ui-avatars.com/api/?name=${encodedName}&background=${bgColor}&color=fff&size=128`
}

// move these here too
const stringToColor = (str: string) => {
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
