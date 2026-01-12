<template>
  <div>
    <header class="border-b">
      <nav class="mx-auto max-w-6xl px-4 h-14 flex items-center gap-4">
        <RouterLink class="font-semibold" to="/">Investment Tracker</RouterLink>
        <RouterLink class="opacity-80 hover:opacity-100" to="/assets">Assets</RouterLink>
        <RouterLink class="opacity-80 hover:opacity-100" to="/transactions">Transactions</RouterLink>
        <RouterLink class="opacity-80 hover:opacity-100" to="/settings">Settings</RouterLink>
        <div class="ml-auto">
          <button class="border rounded px-3 py-1" @click="logout">Logout</button>
        </div>
      </nav>
    </header>

    <main class="mx-auto max-w-6xl px-4">
      <router-view />
    </main>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
const router = useRouter()
const logout = () => {
  localStorage.removeItem('token')
  window.location.href = '/login'
}
</script>