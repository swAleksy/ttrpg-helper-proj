import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Landing',
    component: () => import('@/views/Landing.vue'),
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/Login.vue'),
  },
  {
    path: '/register',
    name: 'Register',
    component: () => import('@/views/Register.vue'),
    // component: () => import('@/views/SignUpPage.vue'), // wersja Adama
  },
  {
    path: '/dashboard',
    name: 'Dashboard',
    component: () => import('@/views/Dashboard.vue'),
    meta: { requiresAuth: true }, // tylko dla zalogowanych
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore()

  const isLoggedIn = authStore.isAuthenticated

  // strony tylko dla niezalogowanych
  const guestOnlyPages = ['Login', 'Register', 'Landing']

  // 1) jeśli user jest zalogowany, a idzie na /, /login lub /register -> przekieruj na dashboard
  if (isLoggedIn && guestOnlyPages.includes(to.name as string)) {
    next({ name: 'Dashboard' })
    return
  }

  // 2) jeśli trasa wymaga zalogowania, a user NIE jest zalogowany -> logowanie
  if (to.meta.requiresAuth && !isLoggedIn) {
    next({ name: 'Login', query: { redirect: to.fullPath } })
  }
  else {
    next()
  }
})

export default router
