<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from './useAuthStore'

const router = useRouter()
const authStore = useAuthStore()

const tenantIdentifier = ref('acme')
const email = ref('admin@acme.com')
const password = ref('Password123!')

onMounted(() => {
  authStore.loadTenants()
})

async function handleLogin() {
  const success = await authStore.login(tenantIdentifier.value, email.value, password.value)
  if (success) {
    router.push('/projects')
  }
}

function quickSelect(t: string, e: string) {
  tenantIdentifier.value = t
  email.value = e
  password.value = 'Password123!'
}
</script>

<template>
  <div class="login-wrapper">
    <div class="login-card">
      <div class="brand-header">
        <div class="badge">MVP Showcase</div>
        <h1>LabNet SaaS</h1>
        <p class="subtitle">Hexagonal Architecture, Multitenancy & DDD</p>
      </div>

      <div class="quick-demo-box">
        <span class="quick-title">⚡ Quick Tenant Fill:</span>
        <div class="button-group">
          <button type="button" @click="quickSelect('acme', 'admin@acme.com')" class="btn-chip">
            Acme (Admin)
          </button>
          <button type="button" @click="quickSelect('acme', 'dev@acme.com')" class="btn-chip">
            Acme (Member)
          </button>
          <button type="button" @click="quickSelect('globex', 'admin@globex.com')" class="btn-chip globex">
            Globex (Admin)
          </button>
        </div>
      </div>

      <form @submit.prevent="handleLogin" class="form">
        <div class="form-group">
          <label for="tenant">Tenant</label>
          <select id="tenant" v-model="tenantIdentifier" required>
            <option value="acme">Acme Corporation (acme)</option>
            <option value="globex">Globex Corp (globex)</option>
          </select>
        </div>

        <div class="form-group">
          <label for="email">Email</label>
          <input
            id="email"
            v-model="email"
            type="email"
            placeholder="admin@acme.com"
            required
          />
        </div>

        <div class="form-group">
          <label for="password">Password</label>
          <input
            id="password"
            v-model="password"
            type="password"
            placeholder="••••••••"
            required
          />
        </div>

        <div v-if="authStore.errorMessage" class="error-banner">
          {{ authStore.errorMessage }}
        </div>

        <button type="submit" :disabled="authStore.loading" class="btn-primary">
          {{ authStore.loading ? 'Authenticating...' : 'Sign In' }}
        </button>
      </form>
    </div>
  </div>
</template>

<style scoped>
.login-wrapper {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: radial-gradient(circle at 50% 20%, #1e293b, #0f172a);
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
  padding: 1.5rem;
}

.login-card {
  background: #ffffff;
  border-radius: 16px;
  padding: 2.5rem;
  width: 100%;
  max-width: 440px;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.3), 0 8px 10px -6px rgba(0, 0, 0, 0.3);
}

.brand-header {
  text-align: center;
  margin-bottom: 1.5rem;
}

.badge {
  display: inline-block;
  background: #e0e7ff;
  color: #4338ca;
  font-size: 0.75rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  margin-bottom: 0.5rem;
}

h1 {
  font-size: 1.75rem;
  font-weight: 800;
  color: #0f172a;
  margin: 0;
}

.subtitle {
  color: #64748b;
  font-size: 0.875rem;
  margin-top: 0.25rem;
}

.quick-demo-box {
  background: #f8fafc;
  border: 1px dashed #cbd5e1;
  border-radius: 10px;
  padding: 0.75rem;
  margin-bottom: 1.5rem;
}

.quick-title {
  display: block;
  font-size: 0.75rem;
  font-weight: 600;
  color: #475569;
  margin-bottom: 0.5rem;
}

.button-group {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.btn-chip {
  background: #e2e8f0;
  border: none;
  border-radius: 6px;
  padding: 0.3rem 0.6rem;
  font-size: 0.75rem;
  font-weight: 600;
  color: #1e293b;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-chip:hover {
  background: #cbd5e1;
}

.btn-chip.globex {
  background: #fef3c7;
  color: #92400e;
}

.btn-chip.globex:hover {
  background: #fde68a;
}

.form-group {
  margin-bottom: 1.25rem;
  text-align: left;
}

label {
  display: block;
  font-size: 0.875rem;
  font-weight: 600;
  color: #334155;
  margin-bottom: 0.4rem;
}

input, select {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  font-size: 0.95rem;
  box-sizing: border-box;
  transition: border-color 0.2s;
}

input:focus, select:focus {
  outline: none;
  border-color: #6366f1;
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.15);
}

.error-banner {
  background: #fee2e2;
  color: #991b1b;
  border-radius: 8px;
  padding: 0.75rem;
  font-size: 0.85rem;
  margin-bottom: 1rem;
}

.btn-primary {
  width: 100%;
  padding: 0.85rem;
  background: #4f46e5;
  color: #ffffff;
  border: none;
  border-radius: 8px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
}

.btn-primary:hover:not(:disabled) {
  background: #4338ca;
}

.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
