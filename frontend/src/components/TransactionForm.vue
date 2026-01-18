<template>
  <section class="p-4 space-y-4">
    <h2 class="text-xl font-semibold text-gray-900">Update Transactions</h2>

    <!-- Date -->
    <div class="grid gap-1 max-w-xs">
      <label class="text-sm font-medium text-gray-700">Date</label>
      <input
        type="date"
        v-model="date"
        class="w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-gray-900
               focus:outline-none focus:ring-2 focus:ring-blue-600/40"
      />
    </div>

    <!-- Table -->
    <div class="overflow-x-auto rounded-xl border border-gray-200 bg-white shadow-sm">
      <table class="w-full text-sm border-collapse">
        <thead>
          <tr class="border-b border-gray-200 bg-gray-100 text-left">
            <th class="py-2 px-2 font-semibold text-gray-700">Asset</th>
            <th class="py-2 px-2 text-right font-semibold text-gray-700">Change</th>
            <th class="py-2 px-2 text-right font-semibold text-gray-700">Current Value</th>
            <th class="py-2 px-2 text-right font-semibold text-gray-700">Dividend</th>
            <th class="py-2 px-2 font-semibold text-gray-700">Notes</th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="row in rows"
            :key="row.assetId"
            class="border-b border-gray-200 hover:bg-gray-50"
          >
            <td class="px-2 py-2 font-medium text-gray-900">
              {{ row.assetName }}
            </td>

            <td class="px-2 py-2">
              <input
                v-model.number="row.transaction.valueChange"
                type="number"
                step="0.01"
                class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-right text-gray-900
                       focus:outline-none focus:ring-2 focus:ring-blue-600/40"
              />
            </td>

            <td class="px-2 py-2">
              <input
                v-model.number="row.transaction.currentValue"
                type="number"
                step="0.01"
                class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-right text-gray-900
                       focus:outline-none focus:ring-2 focus:ring-blue-600/40"
              />
            </td>

            <td class="px-2 py-2">
              <input
                v-model.number="row.transaction.dividend"
                type="number"
                step="0.01"
                class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-right text-gray-900
                       focus:outline-none focus:ring-2 focus:ring-blue-600/40"
              />
            </td>

            <td class="px-2 py-2">
              <input
                v-model="row.transaction.notes"
                placeholder="Notes..."
                class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-gray-900
                       focus:outline-none focus:ring-2 focus:ring-blue-600/40"
              />
            </td>
          </tr>

          <tr v-if="!rows.length">
            <td colspan="5" class="py-4 text-center text-gray-500">
              No active assets
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Actions -->
    <div class="flex justify-end gap-3 pt-3">
      <button
        class="rounded-md border border-gray-300 bg-white px-3 py-2 text-sm
               hover:bg-gray-50"
        @click="$emit('cancel')"
        type="button"
      >
        Cancel
      </button>

      <button
        class="rounded-md border border-blue-600 bg-blue-600 px-3 py-2 text-sm font-medium text-white
               hover:bg-blue-700 disabled:opacity-60 disabled:cursor-not-allowed"
        @click="save"
        :disabled="saving"
        type="button"
      >
        Save
      </button>
    </div>
  </section>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import {
  getTransactionsForEdit,
  bulkUpsertTransactions
} from '@/services/transactionService'

const emit = defineEmits(['cancel', 'saved'])

const date = ref(new Date().toISOString().slice(0, 10))
const rows = ref([])
const saving = ref(false)

/* ---------------------------
   LOAD (VIENINTELIS SOURCE)
---------------------------- */
async function load() {
  rows.value = await getTransactionsForEdit(date.value)

  // užtikrinam, kad visada būtų transaction objektas
  for (const r of rows.value) {
    if (!r.transaction) {
      r.transaction = {
        valueChange: 0,
        currentValue: 0,
        dividend: 0,
        notes: ''
      }
    }
  }
}

onMounted(load)
watch(date, load)

/* ---------------------------
   SAVE
---------------------------- */
async function save() {
  const items = rows.value
    .map(r => ({
      assetId: r.assetId,
      valueChange: r.transaction.valueChange,
      currentValue: r.transaction.currentValue,
      dividend: r.transaction.dividend,
      notes: r.transaction.notes
    }))
    .filter(i =>
      i.valueChange ||
      i.currentValue ||
      i.dividend ||
      (i.notes && i.notes.trim())
    )

  if (!items.length) {
    alert('No changes to save.')
    return
  }

  try {
    saving.value = true
    await bulkUpsertTransactions(date.value, items)
    emit('saved')
    alert('Transactions saved ✅')
  } catch (e) {
    console.error(e)
    alert('Save failed ❌')
  } finally {
    saving.value = false
  }
}
</script>
