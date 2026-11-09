import type { Tenant } from './Tenant'
import type { UserProps } from './User'

export interface LoginCredentials {
  tenantIdentifier: string
  email: string
  password: string
}

export interface AuthSession {
  token: string
  user: UserProps
}

export interface AuthRepository {
  login(credentials: LoginCredentials): Promise<AuthSession>
  getTenants(): Promise<Tenant[]>
}
