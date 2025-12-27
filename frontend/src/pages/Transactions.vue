<template>
  <section class="p-6 space-y-6">
    <h1 class="text-2xl font-semibold">Transactions</h1>

    <!-- Tabs -->
    <div class="flex gap-2 border-b pb-2">
      <button
        class="px-3 py-1 rounded"
        :class="tab === 'edit' ? 'bg-gray-700' : 'bg-gray-800'"
        @click="tab = 'edit'"
      >
        Edit
      </button>

      <button
        class="px-3 py-1 rounded"
        :class="tab === 'history' ? 'bg-gray-700' : 'bg-gray-800'"
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
