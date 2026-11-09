<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/modules/auth/ui/useAuthStore'
import { useProjectsStore } from './useProjectsStore'
import type { TaskPriority } from '../domain/TaskItem'

const authStore = useAuthStore()
const projectsStore = useProjectsStore()

const newProjectName = ref('')
const newProjectDesc = ref('')
const showProjectModal = ref(false)

const newTaskTitle = ref<Record<string, string>>({})
const newTaskPriority = ref<Record<string, TaskPriority>>({})

onMounted(() => {
  projectsStore.fetchProjects()
})

async function handleCreateProject() {
  if (!newProjectName.value) return
  const success = await projectsStore.createProject(newProjectName.value, newProjectDesc.value)
  if (success) {
    newProjectName.value = ''
    newProjectDesc.value = ''
    showProjectModal.value = false
  }
}

async function handleAddTask(projectId: string) {
  const title = newTaskTitle.value[projectId]
  if (!title) return
  const priority = newTaskPriority.value[projectId] || 2
  const success = await projectsStore.addTask(projectId, title, '', priority)
  if (success) {
    newTaskTitle.value[projectId] = ''
  }
}
</script>

<template>
  <div class="dashboard-layout">
    <!-- Navbar -->
    <header class="navbar">
      <div class="nav-left">
        <span class="logo-mark">⚡ LabNet</span>
        <div class="tenant-tag" :class="authStore.currentUser?.tenantIdentifier">
          🏢 Tenant: <strong>{{ authStore.currentUser?.tenantName }}</strong> ({{ authStore.currentUser?.tenantIdentifier }})
        </div>
      </div>
      <div class="nav-right">
        <div class="user-info">
          <span class="user-name">{{ authStore.currentUser?.fullName }}</span>
          <span class="role-badge" :class="authStore.currentUser?.role.toLowerCase()">
            {{ authStore.currentUser?.role }}
          </span>
        </div>
        <button @click="authStore.logout" class="btn-logout">Logout</button>
      </div>
    </header>

    <!-- Main Content -->
    <main class="container">
      <div class="header-actions">
        <div>
          <h2>Projects & Tasks</h2>
          <p class="section-desc">
            Data isolated for <strong>{{ authStore.currentUser?.tenantName }}</strong> via EF Core Global Query Filter.
          </p>
        </div>
        <button @click="showProjectModal = true" class="btn-create">
          + New Project
        </button>
      </div>

      <!-- New Project Form Modal / Dropdown -->
      <div v-if="showProjectModal" class="create-card">
        <h3>Create New Project</h3>
        <div class="form-row">
          <input
            v-model="newProjectName"
            type="text"
            placeholder="Project Title (e.g. Mobile Redesign)"
            class="input-field"
          />
          <input
            v-model="newProjectDesc"
            type="text"
            placeholder="Project Description..."
            class="input-field"
          />
          <div class="modal-buttons">
            <button @click="handleCreateProject" class="btn-save">Save Project</button>
            <button @click="showProjectModal = false" class="btn-cancel">Cancel</button>
          </div>
        </div>
      </div>

      <div v-if="projectsStore.loading && projectsStore.projects.length === 0" class="loading-state">
        Loading projects for {{ authStore.currentUser?.tenantName }}...
      </div>

      <div v-if="projectsStore.error" class="error-banner">
        {{ projectsStore.error }}
      </div>

      <!-- Projects Grid -->
      <div class="projects-grid">
        <div v-for="project in projectsStore.projects" :key="project.id" class="project-card">
          <div class="project-header">
            <div>
              <h3>{{ project.name }}</h3>
              <p class="project-desc">{{ project.description || 'No description provided.' }}</p>
            </div>
            <div class="progress-indicator">
              <span class="progress-text">{{ project.progressPercentage }}%</span>
            </div>
          </div>

          <div class="progress-bar-bg">
            <div class="progress-bar-fill" :style="{ width: project.progressPercentage + '%' }"></div>
          </div>

          <!-- Tasks list -->
          <div class="tasks-section">
            <h4>Tasks ({{ project.completedTasksCount }}/{{ project.tasks.length }})</h4>
            <div v-if="project.tasks.length === 0" class="empty-tasks">
              No tasks yet. Add the first one below!
            </div>
            <ul class="task-list">
              <li
                v-for="task in project.tasks"
                :key="task.id"
                class="task-item"
                :class="{ completed: task.isCompleted }"
              >
                <label class="task-checkbox-label">
                  <input
                    type="checkbox"
                    :checked="task.isCompleted"
                    @change="projectsStore.toggleTask(project.id, task.id)"
                  />
                  <span class="task-title">{{ task.title }}</span>
                </label>
                <span class="priority-tag" :class="'p-' + task.priority">
                  {{ task.priorityLabel }}
                </span>
              </li>
            </ul>

            <!-- Add Task Form -->
            <form @submit.prevent="handleAddTask(project.id)" class="add-task-form">
              <input
                v-model="newTaskTitle[project.id]"
                type="text"
                placeholder="New task title..."
                class="task-input"
                required
              />
              <select v-model="newTaskPriority[project.id]" class="priority-select">
                <option :value="1">Low</option>
                <option :value="2">Medium</option>
                <option :value="3">High</option>
              </select>
              <button type="submit" class="btn-add-task">Add</button>
            </form>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
.dashboard-layout {
  min-height: 100vh;
  background-color: #f8fafc;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  color: #1e293b;
}

.navbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 2rem;
  background: #ffffff;
  border-bottom: 1px solid #e2e8f0;
}

.nav-left, .nav-right {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.logo-mark {
  font-size: 1.25rem;
  font-weight: 800;
  color: #4f46e5;
}

.tenant-tag {
  background: #f1f5f9;
  padding: 0.35rem 0.75rem;
  border-radius: 6px;
  font-size: 0.85rem;
  border: 1px solid #e2e8f0;
}

.tenant-tag.globex {
  background: #fef3c7;
  border-color: #fde68a;
  color: #92400e;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.user-name {
  font-weight: 600;
  font-size: 0.9rem;
}

.role-badge {
  font-size: 0.7rem;
  font-weight: 700;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  text-transform: uppercase;
}

.role-badge.admin {
  background: #fee2e2;
  color: #b91c1c;
}

.role-badge.member {
  background: #e0f2fe;
  color: #0369a1;
}

.btn-logout {
  background: transparent;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  padding: 0.4rem 0.8rem;
  font-size: 0.85rem;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-logout:hover {
  background: #f1f5f9;
}

.container {
  max-width: 1200px;
  margin: 2rem auto;
  padding: 0 1.5rem;
}

.header-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

h2 {
  font-size: 1.75rem;
  font-weight: 800;
  margin: 0;
}

.section-desc {
  color: #64748b;
  margin-top: 0.25rem;
  font-size: 0.9rem;
}

.btn-create {
  background: #4f46e5;
  color: #fff;
  border: none;
  padding: 0.75rem 1.25rem;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
}

.btn-create:hover {
  background: #4338ca;
}

.create-card {
  background: #ffffff;
  padding: 1.5rem;
  border-radius: 12px;
  border: 1px solid #e2e8f0;
  margin-bottom: 2rem;
}

.form-row {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
  flex-wrap: wrap;
}

.input-field {
  flex: 1;
  min-width: 200px;
  padding: 0.65rem 0.85rem;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
}

.modal-buttons {
  display: flex;
  gap: 0.5rem;
}

.btn-save {
  background: #10b981;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.65rem 1rem;
  cursor: pointer;
  font-weight: 600;
}

.btn-cancel {
  background: #e2e8f0;
  border: none;
  border-radius: 6px;
  padding: 0.65rem 1rem;
  cursor: pointer;
}

.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(360px, 1fr));
  gap: 1.5rem;
}

.project-card {
  background: #ffffff;
  border-radius: 12px;
  padding: 1.5rem;
  border: 1px solid #e2e8f0;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.project-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.project-header h3 {
  margin: 0;
  font-size: 1.2rem;
  font-weight: 700;
}

.project-desc {
  font-size: 0.85rem;
  color: #64748b;
  margin: 0.25rem 0 0.75rem 0;
}

.progress-indicator {
  font-size: 0.85rem;
  font-weight: 700;
  color: #4f46e5;
}

.progress-bar-bg {
  width: 100%;
  height: 6px;
  background: #e2e8f0;
  border-radius: 999px;
  overflow: hidden;
  margin-bottom: 1.25rem;
}

.progress-bar-fill {
  height: 100%;
  background: #4f46e5;
  transition: width 0.3s;
}

.tasks-section h4 {
  font-size: 0.95rem;
  margin: 0 0 0.75rem 0;
  color: #334155;
}

.empty-tasks {
  font-size: 0.85rem;
  color: #94a3b8;
  font-style: italic;
  margin-bottom: 0.75rem;
}

.task-list {
  list-style: none;
  padding: 0;
  margin: 0 0 1rem 0;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.task-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0.75rem;
  background: #f8fafc;
  border-radius: 6px;
  border: 1px solid #f1f5f9;
}

.task-item.completed .task-title {
  text-decoration: line-through;
  color: #94a3b8;
}

.task-checkbox-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  font-size: 0.875rem;
}

.priority-tag {
  font-size: 0.7rem;
  font-weight: 700;
  padding: 0.15rem 0.4rem;
  border-radius: 4px;
}

.priority-tag.p-3 {
  background: #fee2e2;
  color: #b91c1c;
}

.priority-tag.p-2 {
  background: #fef3c7;
  color: #b45309;
}

.priority-tag.p-1 {
  background: #f1f5f9;
  color: #64748b;
}

.add-task-form {
  display: flex;
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.task-input {
  flex: 1;
  padding: 0.5rem 0.75rem;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 0.85rem;
}

.priority-select {
  padding: 0.5rem;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 0.85rem;
}

.btn-add-task {
  background: #0284c7;
  color: white;
  border: none;
  padding: 0.5rem 0.85rem;
  border-radius: 6px;
  font-size: 0.85rem;
  cursor: pointer;
}
</style>
