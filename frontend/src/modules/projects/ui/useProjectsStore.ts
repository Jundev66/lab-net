import { defineStore } from 'pinia'
import { ref } from 'vue'
import { Project } from '../domain/Project'
import { HttpProjectRepository } from '../infrastructure/HttpProjectRepository'
import {
  AddTaskUseCase,
  CreateProjectUseCase,
  GetProjectsUseCase,
  ToggleTaskUseCase,
} from '../application/ProjectUseCases'
import type { TaskPriority } from '../domain/TaskItem'

const repository = new HttpProjectRepository()
const getProjectsUseCase = new GetProjectsUseCase(repository)
const createProjectUseCase = new CreateProjectUseCase(repository)
const addTaskUseCase = new AddTaskUseCase(repository)
const toggleTaskUseCase = new ToggleTaskUseCase(repository)

export const useProjectsStore = defineStore('projects', () => {
  const projects = ref<Project[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchProjects() {
    loading.value = true
    error.value = null
    try {
      projects.value = await getProjectsUseCase.execute()
    } catch (err: any) {
      error.value = err.response?.data?.detail || err.message || 'Error loading projects.'
    } finally {
      loading.value = false
    }
  }

  async function createProject(name: string, description: string) {
    loading.value = true
    error.value = null
    try {
      await createProjectUseCase.execute({ name, description })
      await fetchProjects()
      return true
    } catch (err: any) {
      error.value = err.response?.data?.detail || err.message || 'Failed to create project.'
      return false
    } finally {
      loading.value = false
    }
  }

  async function addTask(projectId: string, title: string, description: string, priority: TaskPriority) {
    try {
      await addTaskUseCase.execute(projectId, { title, description, priority })
      await fetchProjects()
      return true
    } catch (err: any) {
      error.value = err.response?.data?.detail || err.message || 'Failed to add task.'
      return false
    }
  }

  async function toggleTask(projectId: string, taskId: string) {
    try {
      await toggleTaskUseCase.execute(projectId, taskId)
      await fetchProjects()
    } catch (err: any) {
      error.value = err.response?.data?.detail || err.message || 'Failed to toggle task.'
    }
  }

  return {
    projects,
    loading,
    error,
    fetchProjects,
    createProject,
    addTask,
    toggleTask,
  }
})
