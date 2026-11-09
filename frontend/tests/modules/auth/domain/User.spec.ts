import { describe, it, expect } from 'vitest'
import { User } from '@/modules/auth/domain/User'

describe('User Domain Entity', () => {
  it('should create a valid user instance', () => {
    const user = new User({
      id: 'u-1',
      fullName: 'Alice Walker',
      email: 'alice@acme.com',
      role: 'Admin',
      tenantId: 'tenant-1',
      tenantIdentifier: 'acme',
      tenantName: 'Acme Corp',
    })

    expect(user.id).toBe('u-1')
    expect(user.isAdmin()).toBe(true)
    expect(user.tenantIdentifier).toBe('acme')
  })

  it('should identify member role correctly', () => {
    const user = new User({
      id: 'u-2',
      fullName: 'Bob Smith',
      email: 'bob@acme.com',
      role: 'Member',
      tenantId: 'tenant-1',
      tenantIdentifier: 'acme',
      tenantName: 'Acme Corp',
    })

    expect(user.isAdmin()).toBe(false)
  })

  it('should throw an error if email is invalid', () => {
    expect(() => {
      new User({
        id: 'u-3',
        fullName: 'Invalid User',
        email: 'invalid-email',
        role: 'Member',
        tenantId: 'tenant-1',
        tenantIdentifier: 'acme',
        tenantName: 'Acme Corp',
      })
    }).toThrow('Invalid user email address.')
  })
})
