import type { AuthRepository } from '../domain/AuthRepository'
import type { Tenant } from '../domain/Tenant'

export class GetTenantsUseCase {
  constructor(private readonly authRepository: AuthRepository) {}

  async execute(): Promise<Tenant[]> {
    return await this.authRepository.getTenants()
  }
}
