<template>
  <section class="p-6 space-y-6">
    <h1 class="text-2xl font-semibold text-gray-900">Transactions</h1>

    <!-- Tabs -->
    <div class="flex gap-2 border-b border-gray-200 pb-2">
      <button
        class="rounded-md border px-3 py-1.5 text-sm font-medium transition hover:bg-gray-50"
        :class="tab === 'edit'
          ? 'border-blue-600 bg-blue-50 text-gray-900'
          : 'border-gray-300 bg-white text-gray-600'"
        type="button"
        @click="tab = 'edit'"
      >
        Edit
      </button>

      <button
        class="rounded-md border px-3 py-1.5 text-sm font-medium transition hover:bg-gray-50"
        :class="tab === 'history'
          ? 'border-blue-600 bg-blue-50 text-gray-900'
          : 'border-gray-300 bg-white text-gray-600'"
        type="button"
        @click="tab = 'history'"
      >
        History
      </button>
    </div>

    <!-- Content -->
    <TransactionForm
      v-if="tab === 'edit'"
      @saved="onSaved"
    />

    <TransactionHistory
      v-else
    />
  </section>
</template>

<script setup>
import { ref } from 'vue'
import TransactionForm from '@/components/TransactionForm.vue'
import TransactionHistory from '@/components/TransactionHistory.vue'

const tab = ref('edit')

function onSaved() {
  // optional: automatiškai perjungti į history
  tab.value = 'history'
}
</script>
