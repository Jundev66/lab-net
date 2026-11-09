import type {
  CreateProjectPayload,
  CreateTaskPayload,
  ProjectRepository,
} from '../domain/ProjectRepository'
import type { ProjectProps } from '../domain/Project'
import type { TaskItemProps } from '../domain/TaskItem'
import { httpClient } from '@/shared/infrastructure/http/httpClient'

export class HttpProjectRepository implements ProjectRepository {
  async getAll(): Promise<ProjectProps[]> {
    const { data } = await httpClient.get<ProjectProps[]>('/projects')
    return data
  }

  async getById(id: string): Promise<ProjectProps> {
    const { data } = await httpClient.get<ProjectProps>(`/projects/${id}`)
    return data
  }

  async create(payload: CreateProjectPayload): Promise<ProjectProps> {
    const { data } = await httpClient.post<ProjectProps>('/projects', payload)
    return data
  }

  async addTask(projectId: string, payload: CreateTaskPayload): Promise<TaskItemProps> {
    const { data } = await httpClient.post<TaskItemProps>(`/projects/${projectId}/tasks`, payload)
    return data
  }

  async toggleTask(projectId: string, taskId: string): Promise<TaskItemProps> {
    const { data } = await httpClient.patch<TaskItemProps>(
      `/projects/${projectId}/tasks/${taskId}/toggle`
    )
    return data
  }
}
