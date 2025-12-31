/**
 * Shared TypeScript interfaces/types
 * Used across multiple stores and components
 */

// ============== USER ==============

export interface UserProfile {
  id: number
  userName: string
  email: string
  avatarUrl?: string | null
}

export interface UserInfoDto {
  id: number
  userName: string
  email: string
  avatarUrl: string
}

// ============== AUTH ==============

export interface LoginResponse {
  token: string
  refreshToken: string
}

// ============== NOTIFICATIONS ==============

export type NotificationType =
  | 'AddedToGroup'
  | 'NewMessage'
  | 'FriendRequest'
  | 'FriendRequestAccepted'

export interface NotificationDto {
  id: number
  title: string
  message: string
  type: NotificationType
  isRead: boolean
  createdAt: string
  fromUserId?: number
}

// ============== CHAT ==============

export interface MessageDto {
  id: number
  senderId: number
  senderName: string
  receiverId: number
  content: string
  sentAt: string
  isRead: boolean
}

// ============== Campaign / Session ==============

export interface PlayerDto {
  playerId: number
  playerName: string
}

export interface SessionDto {
  id: number
  name: string
  description: string | null
  scheduledDate: string
  status: string
  players: PlayerDto[]
}

export interface GameCampaignDto {
  id: number
  name: string
  description: string | null
  gameMasterId: number
  gameMasterName: string
  sessions: SessionDto[]
}

// ============== SESSION EVENTS ===============
// ---  Payloads---

export interface ChatMessagePayload {
  message: string
}

export interface DiceRollPayload {
  dice: string // np. "d20"
  result: number // np. 14
}

// export interface SkillCheckPayload {
//   skillName: string // np. "Percepcja"
//   difficultyClass: number // DC
//   rollResult: number
//   success: boolean
// }

export interface ShareNpcPayload {
  userId: number
}

export interface ShareItemPayload {
  itemId: number
  itemName: string
  // ... whatever properties you serialize in C#
}

export interface UserLifecyclePayload {
  userId: number
  userName: string
}

// --- enum ---
export type SessionEventDomain =
  | { type: 'ChatMessage'; data: ChatMessagePayload }
  | { type: 'DiceRoll'; data: DiceRollPayload }
  | { type: 'ShareItem'; data: ShareItemPayload }
  | { type: 'ShareNpc'; data: ShareNpcPayload }
  | { type: 'UserJoined'; data: UserLifecyclePayload }
  | { type: 'UserLeft'; data: UserLifecyclePayload }
  | { type: 'Unknown'; data: unknown }

// --- 3. Wspólne pola dla każdego eventu ---
export type SessionEventModel = SessionEventDomain & {
  id: number
  sessionId: number
  userId: number
  userName: string
  timestamp: Date // Tutaj już chcemy mieć obiekt Date, nie string
}

export interface GetSessionEventDto {
  id: number
  sessionId: number
  type: string // Tu jest jeszcze zwykły string
  dataJson: string // Tu jest zserializowany JSON
  timestamp: string // Data w formacie ISO z C#
  userId: number
  userName: string
}

export interface CreateSessionEventDto {
  sessionId: number
  type: string
  dataJson: string
}
