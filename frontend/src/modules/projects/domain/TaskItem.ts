export type TaskPriority = 1 | 2 | 3

export interface TaskItemProps {
  id: string
  projectId: string
  title: string
  description: string
  priority: TaskPriority
  isCompleted: boolean
  createdAtUtc: string
}

export class TaskItem {
  readonly id: string
  readonly projectId: string
  readonly title: string
  readonly description: string
  readonly priority: TaskPriority
  isCompleted: boolean
  readonly createdAtUtc: string

  constructor(props: TaskItemProps) {
    this.id = props.id
    this.projectId = props.projectId
    this.title = props.title
    this.description = props.description
    this.priority = props.priority
    this.isCompleted = props.isCompleted
    this.createdAtUtc = props.createdAtUtc
  }

  toggle() {
    this.isCompleted = !this.isCompleted
  }

  get priorityLabel(): string {
    switch (this.priority) {
      case 3:
        return 'High'
      case 2:
        return 'Medium'
      default:
        return 'Low'
    }
  }
}
