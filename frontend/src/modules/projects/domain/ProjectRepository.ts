import type { ProjectProps } from './Project'
import type { TaskItemProps, TaskPriority } from './TaskItem'

export interface CreateProjectPayload {
  name: string
  description: string
}

export interface CreateTaskPayload {
  title: string
  description: string
  priority: TaskPriority
}

export interface ProjectRepository {
  getAll(): Promise<ProjectProps[]>
  getById(id: string): Promise<ProjectProps>
  create(payload: CreateProjectPayload): Promise<ProjectProps>
  addTask(projectId: string, payload: CreateTaskPayload): Promise<TaskItemProps>
  toggleTask(projectId: string, taskId: string): Promise<TaskItemProps>
}
