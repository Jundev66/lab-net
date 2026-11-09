import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '@/modules/auth/ui/LoginView.vue'
import ProjectsDashboardView from '@/modules/projects/ui/ProjectsDashboardView.vue'

const routes = [
  {
    path: '/',
    redirect: '/projects',
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
    meta: { public: true },
  },
  {
    path: '/projects',
    name: 'Projects',
    component: ProjectsDashboardView,
    meta: { requiresAuth: true },
  },
]

export const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('labnet_token')
  if (to.meta.requiresAuth && !token) {
    next('/login')
  } else if (to.path === '/login' && token) {
    next('/projects')
  } else {
    next()
  }
})
