import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { User, type UserProps } from '../domain/User'
import type { Tenant } from '../domain/Tenant'
import { HttpAuthRepository } from '../infrastructure/HttpAuthRepository'
import { LoginUseCase } from '../application/LoginUseCase'
import { GetTenantsUseCase } from '../application/GetTenantsUseCase'

const authRepository = new HttpAuthRepository()
const loginUseCase = new LoginUseCase(authRepository)
const getTenantsUseCase = new GetTenantsUseCase(authRepository)

function getInitialUser(): UserProps | null {
  const saved = localStorage.getItem('labnet_user')
  return saved ? JSON.parse(saved) : null
}

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('labnet_token'))
  const rawUser = ref<UserProps | null>(getInitialUser())

  const currentUser = computed(() => (rawUser.value ? new User(rawUser.value) : null))
  const isAuthenticated = computed(() => !!token.value && !!rawUser.value)

  const tenants = ref<Tenant[]>([])
  const loading = ref(false)
  const errorMessage = ref<string | null>(null)

  async function loadTenants() {
    try {
      tenants.value = await getTenantsUseCase.execute()
    } catch (err: any) {
      console.error('Failed to load tenants', err)
    }
  }

  async function login(tenantIdentifier: string, email: string, password: string) {
    loading.value = true
    errorMessage.value = null
    try {
      const session = await loginUseCase.execute({ tenantIdentifier, email, password })
      token.value = session.token
      rawUser.value = session.user

      localStorage.setItem('labnet_token', session.token)
      localStorage.setItem('labnet_user', JSON.stringify(session.user))
      localStorage.setItem('labnet_tenant_id', session.user.tenantId)

      return true
    } catch (err: any) {
      errorMessage.value = err.response?.data?.detail || err.message || 'Login failed.'
      return false
    } finally {
      loading.value = false
    }
  }

  function logout() {
    token.value = null
    rawUser.value = null
    localStorage.removeItem('labnet_token')
    localStorage.removeItem('labnet_user')
    localStorage.removeItem('labnet_tenant_id')
    window.location.href = '/login'
  }

  return {
    token,
    currentUser,
    isAuthenticated,
    tenants,
    loading,
    errorMessage,
    loadTenants,
    login,
    logout,
  }
})
