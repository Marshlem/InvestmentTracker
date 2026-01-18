<script setup>
import { isAuthenticated, logout } from '@/services/authService'
</script>

<template>
  <div class="min-h-screen bg-gray-100">
    <header class="h-14 flex items-center gap-6 px-4 border-b border-black/10">
      <!-- LOGO + BRAND -->
      <router-link to="/" class="flex items-center gap-2 font-semibold">
        <img class="h-7 w-auto" src="/logo40px.svg" alt="Marshlem Development" />
        <span>Investment Tracker</span>
      </router-link>

      <!-- MENU -->
      <nav class="flex items-center gap-2">
        <router-link
          to="/"
          class="px-3 py-2 rounded-md text-sm hover:bg-black/5"
          exact-active-class="bg-black/10"
        >
          Dashboard
        </router-link>

        <router-link
          to="/assets"
          class="px-3 py-2 rounded-md text-sm hover:bg-black/5"
          active-class="bg-black/10"
        >
          Assets
        </router-link>

        <router-link
          to="/transactions"
          class="px-3 py-2 rounded-md text-sm hover:bg-black/5"
          active-class="bg-black/10"
        >
          Transactions
        </router-link>
      </nav>

      <!-- LOGOUT -->
      <div class="ml-auto">
        <button
          v-if="isAuthenticated()"
          @click="logout"
          class="px-3 py-2 text-sm rounded-md hover:bg-black/5"
        >
          Logout
        </button>
      </div>
    </header>

    <main class="p-4">
      <router-view />
    </main>
  </div>
</template>
