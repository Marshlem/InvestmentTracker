<template>
  <section class="min-h-[60vh] grid place-items-center p-6">
    <div class="w-full max-w-sm rounded-xl border p-6">
      <h1 class="text-xl font-semibold mb-4">Sign in</h1>
      <LoginForm />
    </div>
  </section>
</template>

<script setup>
import LoginForm from '@/components/LoginForm.vue'
</script>
