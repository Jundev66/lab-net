import type { AuthRepository, AuthSession, LoginCredentials } from '../domain/AuthRepository'
import type { Tenant } from '../domain/Tenant'
import { httpClient } from '@/shared/infrastructure/http/httpClient'

interface ApiLoginResponse {
  token: string
  userId: string
  fullName: string
  email: string
  role: 'Admin' | 'Member'
  tenantId: string
  tenantIdentifier: string
  tenantName: string
}

export class HttpAuthRepository implements AuthRepository {
  async login(credentials: LoginCredentials): Promise<AuthSession> {
    const { data } = await httpClient.post<ApiLoginResponse>('/auth/login', credentials)
    return {
      token: data.token,
      user: {
        id: data.userId,
        fullName: data.fullName,
        email: data.email,
        role: data.role,
        tenantId: data.tenantId,
        tenantIdentifier: data.tenantIdentifier,
        tenantName: data.tenantName,
      },
    }
  }

  async getTenants(): Promise<Tenant[]> {
    const { data } = await httpClient.get<Tenant[]>('/auth/tenants')
    return data
  }
}
