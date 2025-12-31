/**
 * API Configuration
 * Centralized configuration for API endpoints and base URLs
 */

export const API_URL =
  (import.meta.env.VITE_API_URL as string | undefined) ?? 'http://localhost:8080'

export const SIGNALR_HUB_URL = `${API_URL}/mainHub`
export const SIGNALR_HUB_SESSION_URL = `${API_URL}/GameSessionHub`
