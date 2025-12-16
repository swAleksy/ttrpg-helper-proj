import { defineStore } from 'pinia'
import { ref } from 'vue'
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr'
import { useAuthStore, API_URL } from '@/stores/auth'
import { useFriendsStore } from '@/stores/friendsStore'
import axios from 'axios'

export const useCommunicationStore = defineStore('communication', () => {
  const connection = ref<HubConnection | null>(null)
  const isConnected = ref(false)

  // Licznik nieprzeczytanych powiadomień (np. nowe zaproszenia)
  const unreadNotificationsCount = ref(0)
  const hasNewMessages = ref(false) // Np. do kropki przy czacie

  const authStore = useAuthStore()
  const friendsStore = useFriendsStore()

  const initSignalR = async () => {
    // Jeśli już połączony lub brak tokena - nie rób nic
    if (isConnected.value || !authStore.token) return
    await fetchInitialNotificationsState()
    const newConnection = new HubConnectionBuilder()
      .withUrl(`${API_URL}/mainHub`, {
        accessTokenFactory: () => authStore.token || '',
      })
      .withAutomaticReconnect()
      .build()

    // --- HANDLERS (Co robić jak przyjdzie dane zdarzenie) ---

    // 1. Ktoś wysłał wiadomość
    newConnection.on('ReceivePrivateMessage', (msg) => {
      // Tu możesz np. odtworzyć dźwięk
      hasNewMessages.value = true
      // Jeśli masz otwarty Store czatu, możesz tam dodać wiadomość bezpośrednio
    })

    // 2. Ktoś wysłał zaproszenie do znajomych
    newConnection.on('ReceiveFriendRequest', () => {
      unreadNotificationsCount.value++
      // Opcjonalnie: odśwież listę oczekujących w tle
      friendsStore.fetchPending()
    })

    // 3. Ktoś zaakceptował zaproszenie
    newConnection.on('FriendRequestAccepted', () => {
      // Np. pokaż "toast" notification: "Użytkownik X przyjął zaproszenie"
      friendsStore.fetchFriends()
    })

    try {
      await newConnection.start()
      console.log('SignalR Global Connected')
      isConnected.value = true
      connection.value = newConnection
    } catch (err) {
      console.error('SignalR Connection Error:', err)
    }
  }

  const fetchInitialNotificationsState = async () => {
    if (!authStore.token) return
    try {
      // TODO dedykowany endpoint
      // const res = await axios.get(`${API_URL}/api/notifications/count`);
      // unreadNotificationsCount.value = res.data.count;
      const res = await axios.get(`${API_URL}/api/friend/pending`)
      unreadNotificationsCount.value = res.data.length
    } catch (err) {
      console.error('Błąd pobierania licznika powiadomień', err)
    }
  }

  const stopSignalR = () => {
    if (connection.value) {
      connection.value.stop()
      isConnected.value = false
      connection.value = null
    }
  }

  // Metoda do zerowania powiadomień jak otworzysz okno
  const markNotificationsAsRead = () => {
    unreadNotificationsCount.value = 0
  }

  return {
    connection,
    isConnected,
    unreadNotificationsCount,
    initSignalR,
    stopSignalR,
    markNotificationsAsRead,
  }
})
