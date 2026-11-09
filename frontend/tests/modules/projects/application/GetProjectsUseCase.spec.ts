import { describe, it, expect, vi } from 'vitest'
import { GetProjectsUseCase } from '@/modules/projects/application/ProjectUseCases'
import type { ProjectRepository } from '@/modules/projects/domain/ProjectRepository'

describe('GetProjectsUseCase', () => {
  it('should fetch and map project entities correctly', async () => {
    const mockRepo: ProjectRepository = {
      getAll: vi.fn().mockResolvedValue([
        {
          id: 'proj-123',
          name: 'Architecture Redesign',
          description: 'Hexagonal refactoring',
          status: 1,
          createdAtUtc: new Date().toISOString(),
          tasks: [],
        },
      ]),
      getById: vi.fn(),
      create: vi.fn(),
      addTask: vi.fn(),
      toggleTask: vi.fn(),
    }

    const useCase = new GetProjectsUseCase(mockRepo)
    const projects = await useCase.execute()

    expect(mockRepo.getAll).toHaveBeenCalledOnce()
    expect(projects).toHaveLength(1)
    expect(projects[0].name).toBe('Architecture Redesign')
    expect(projects[0].tasks).toEqual([])
  })
})
