import { Project } from '../domain/Project'
import type {
  CreateProjectPayload,
  CreateTaskPayload,
  ProjectRepository,
} from '../domain/ProjectRepository'
import { TaskItem } from '../domain/TaskItem'

export class GetProjectsUseCase {
  constructor(private readonly repository: ProjectRepository) {}

  async execute(): Promise<Project[]> {
    const raw = await this.repository.getAll()
    return raw.map((p) => new Project(p))
  }
}

export class CreateProjectUseCase {
  constructor(private readonly repository: ProjectRepository) {}

  async execute(payload: CreateProjectPayload): Promise<Project> {
    if (!payload.name || payload.name.trim().length === 0) {
      throw new Error('Project name is required.')
    }
    const raw = await this.repository.create(payload)
    return new Project(raw)
  }
}

export class AddTaskUseCase {
  constructor(private readonly repository: ProjectRepository) {}

  async execute(projectId: string, payload: CreateTaskPayload): Promise<TaskItem> {
    if (!payload.title || payload.title.trim().length === 0) {
      throw new Error('Task title is required.')
    }
    const raw = await this.repository.addTask(projectId, payload)
    return new TaskItem(raw)
  }
}

export class ToggleTaskUseCase {
  constructor(private readonly repository: ProjectRepository) {}

  async execute(projectId: string, taskId: string): Promise<TaskItem> {
    const raw = await this.repository.toggleTask(projectId, taskId)
    return new TaskItem(raw)
  }
}
