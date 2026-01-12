<template>
  <section class="min-h-[60vh] grid place-items-center p-6">
    <div class="w-full max-w-sm rounded-xl border p-6 space-y-4">

      <!-- Tabs -->
      <div class="flex gap-2">
        <button
          class="flex-1 px-3 py-2 rounded"
          :class="tab === 'login' ? 'bg-gray-700' : 'bg-gray-800'"
          @click="tab = 'login'"
        >
          Sign in
        </button>

        <button
          class="flex-1 px-3 py-2 rounded"
          :class="tab === 'register' ? 'bg-gray-700' : 'bg-gray-800'"
          @click="tab = 'register'"
        >
          Register
        </button>
      </div>

      <!-- Content -->
      <LoginForm v-if="tab === 'login'" />
      <RegisterForm v-else />

    </div>
  </section>
</template>

<script setup>
import { ref } from 'vue'
import LoginForm from '@/components/LoginForm.vue'
import RegisterForm from '@/components/RegisterForm.vue'

const tab = ref('login')
</script>
