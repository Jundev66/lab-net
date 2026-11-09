import type { AuthRepository, AuthSession, LoginCredentials } from '../domain/AuthRepository'

export class LoginUseCase {
  constructor(private readonly authRepository: AuthRepository) {}

  async execute(credentials: LoginCredentials): Promise<AuthSession> {
    if (!credentials.tenantIdentifier) {
      throw new Error('Tenant selection is required.')
    }
    if (!credentials.email || !credentials.email.includes('@')) {
      throw new Error('Valid email address is required.')
    }
    if (!credentials.password) {
      throw new Error('Password cannot be empty.')
    }

    return await this.authRepository.login(credentials)
  }
}
