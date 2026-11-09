import { TaskItem, type TaskItemProps } from './TaskItem'

export type ProjectStatus = 1 | 2

export interface ProjectProps {
  id: string
  name: string
  description: string
  status: ProjectStatus
  createdAtUtc: string
  tasks: TaskItemProps[]
}

export class Project {
  readonly id: string
  readonly name: string
  readonly description: string
  readonly status: ProjectStatus
  readonly createdAtUtc: string
  readonly tasks: TaskItem[]

  constructor(props: ProjectProps) {
    if (!props.name || props.name.trim().length === 0) {
      throw new Error('Project name cannot be empty.')
    }
    this.id = props.id
    this.name = props.name
    this.description = props.description
    this.status = props.status
    this.createdAtUtc = props.createdAtUtc
    this.tasks = (props.tasks || []).map((t) => new TaskItem(t))
  }

  get completedTasksCount(): number {
    return this.tasks.filter((t) => t.isCompleted).length
  }

  get progressPercentage(): number {
    if (this.tasks.length === 0) return 0
    return Math.round((this.completedTasksCount / this.tasks.length) * 100)
  }
}
