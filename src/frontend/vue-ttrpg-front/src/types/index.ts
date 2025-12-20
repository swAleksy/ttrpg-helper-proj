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
