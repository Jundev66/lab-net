export interface UserProps {
  id: string
  fullName: string
  email: string
  role: 'Admin' | 'Member'
  tenantId: string
  tenantIdentifier: string
  tenantName: string
}

export class User {
  readonly id: string
  readonly fullName: string
  readonly email: string
  readonly role: 'Admin' | 'Member'
  readonly tenantId: string
  readonly tenantIdentifier: string
  readonly tenantName: string

  constructor(props: UserProps) {
    if (!props.email || !props.email.includes('@')) {
      throw new Error('Invalid user email address.')
    }
    this.id = props.id
    this.fullName = props.fullName
    this.email = props.email
    this.role = props.role
    this.tenantId = props.tenantId
    this.tenantIdentifier = props.tenantIdentifier
    this.tenantName = props.tenantName
  }

  isAdmin(): boolean {
    return this.role === 'Admin'
  }
}
