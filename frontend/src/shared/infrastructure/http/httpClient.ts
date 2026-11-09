import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'

export const httpClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

httpClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('labnet_token')
  const tenantId = localStorage.getItem('labnet_tenant_id')

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  if (tenantId) {
    config.headers['X-Tenant-Id'] = tenantId
  }

  return config
})

httpClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401 && window.location.pathname !== '/login') {
      localStorage.removeItem('labnet_token')
      localStorage.removeItem('labnet_user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)
