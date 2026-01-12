<template>
  <form class="grid gap-3" @submit.prevent="onLogin">
    <input
      class="border rounded px-3 py-2"
      v-model="username"
      placeholder="Username"
      autocomplete="username"
    />

    <input
      class="border rounded px-3 py-2"
      v-model="password"
      type="password"
      placeholder="Password"
      autocomplete="current-password"
    />

    <button
      class="border rounded px-3 py-2 disabled:opacity-60"
      :disabled="loading"
      type="submit"
    >
      {{ loading ? 'Signing in…' : 'Sign in' }}
    </button>

    <p class="text-sm text-red-600" v-if="error">
      {{ error }}
    </p>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import { login } from '@/services/authService'
import { useRouter } from 'vue-router'

const router = useRouter()
const username = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

const onLogin = async () => {
  error.value = ''
  loading.value = true

  try {
    await login(username.value, password.value)
    router.push({ name: 'dashboard' })
  } catch {
    error.value = 'Invalid username or password'
  } finally {
    loading.value = false
  }
}
</script>
