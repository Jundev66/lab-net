import { describe, it, expect } from 'vitest'
import { Project } from '@/modules/projects/domain/Project'

describe('Project Domain Entity', () => {
  it('should calculate completion percentage accurately', () => {
    const project = new Project({
      id: 'p-1',
      name: 'Cloud Migration',
      description: 'Moving servers',
      status: 1,
      createdAtUtc: new Date().toISOString(),
      tasks: [
        {
          id: 't-1',
          projectId: 'p-1',
          title: 'Provision VMs',
          description: '',
          priority: 3,
          isCompleted: true,
          createdAtUtc: new Date().toISOString(),
        },
        {
          id: 't-2',
          projectId: 'p-1',
          title: 'Setup DNS',
          description: '',
          priority: 2,
          isCompleted: false,
          createdAtUtc: new Date().toISOString(),
        },
      ],
    })

    expect(project.completedTasksCount).toBe(1)
    expect(project.progressPercentage).toBe(50)
  })

  it('should throw an error if project name is empty', () => {
    expect(() => {
      new Project({
        id: 'p-2',
        name: '   ',
        description: 'No name',
        status: 1,
        createdAtUtc: new Date().toISOString(),
        tasks: [],
      })
    }).toThrow('Project name cannot be empty.')
  })
})
