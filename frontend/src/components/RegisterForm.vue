<template>
  <form class="grid gap-3" @submit.prevent="onRegister">
    <input
      class="border rounded px-3 py-2"
      v-model="username"
      placeholder="Username"
    />

    <input
      class="border rounded px-3 py-2"
      v-model="password"
      type="password"
      placeholder="Password"
    />

    <input
      class="border rounded px-3 py-2"
      v-model="confirm"
      type="password"
      placeholder="Confirm password"
    />

    <button class="border rounded px-3 py-2">
      Register
    </button>

    <p class="text-sm text-red-600" v-if="error">
      {{ error }}
    </p>

    <p class="text-sm text-green-600" v-if="success">
      Account created. You can now sign in.
    </p>
  </form>
</template>

<script setup>
import { ref } from 'vue'
import { register } from '@/services/authService'

const username = ref('')
const password = ref('')
const confirm = ref('')
const error = ref('')
const success = ref(false)

const onRegister = async () => {
  error.value = ''
  success.value = false

  if (!username.value || !password.value) {
    error.value = 'Username and password are required'
    return
  }

  if (password.value !== confirm.value) {
    error.value = 'Passwords do not match'
    return
  }

  try {
    await register(username.value, password.value)
    success.value = true

    username.value = ''
    password.value = ''
    confirm.value = ''
  } catch (e) {
    error.value =
      e?.response?.data ??
      e.message ??
      'Registration failed'
  }
}
</script>
