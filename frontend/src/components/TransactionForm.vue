<template>
  <section class="p-4 space-y-4">
    <h2 class="text-xl font-semibold">Update Transactions</h2>

    <!-- Date -->
    <div class="grid gap-1 max-w-xs">
      <label class="text-sm">Date</label>
      <input
        type="date"
        v-model="date"
        class="border rounded px-3 py-2"
      />
    </div>

    <!-- Table -->
    <div class="overflow-x-auto">
      <table class="w-full text-sm border-collapse">
        <thead>
          <tr class="border-b bg-gray-800 text-left">
            <th class="py-2 px-2">Asset</th>
            <th class="py-2 px-2 text-right">Change</th>
            <th class="py-2 px-2 text-right">Current Value</th>
            <th class="py-2 px-2 text-right">Dividend</th>
            <th class="py-2 px-2">Notes</th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="row in rows"
            :key="row.assetId"
            class="border-b hover:bg-gray-800/50"
          >
            <td class="px-2 py-2 font-medium">
              {{ row.assetName }}
            </td>

            <td class="px-2 py-2">
              <input
                v-model.number="row.transaction.valueChange"
                type="number"
                step="0.01"
                class="border rounded px-2 py-1 w-full text-right"
              />
            </td>

            <td class="px-2 py-2">
              <input
                v-model.number="row.transaction.currentValue"
                type="number"
                step="0.01"
                class="border rounded px-2 py-1 w-full text-right"
              />
            </td>

            <td class="px-2 py-2">
              <input
                v-model.number="row.transaction.dividend"
                type="number"
                step="0.01"
                class="border rounded px-2 py-1 w-full text-right"
              />
            </td>

            <td class="px-2 py-2">
              <input
                v-model="row.transaction.notes"
                placeholder="Notes..."
                class="border rounded px-2 py-1 w-full"
              />
            </td>
          </tr>

          <tr v-if="!rows.length">
            <td colspan="5" class="text-center text-gray-500 py-4">
              No active assets
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Actions -->
    <div class="flex justify-end gap-3 pt-3">
      <button
        class="border rounded px-3 py-2"
        @click="$emit('cancel')"
      >
        Cancel
      </button>

      <button
        class="border rounded px-3 py-2"
        @click="save"
        :disabled="saving"
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
