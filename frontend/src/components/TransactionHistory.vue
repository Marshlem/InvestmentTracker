<template>
  <section class="space-y-4">
    <h2 class="text-xl font-semibold">Transaction History</h2>

    <!-- FILTERS -->
    <div class="flex flex-wrap gap-3 items-end">
      <div class="grid gap-1">
        <label class="text-sm">From</label>
        <input
          type="date"
          v-model="pending.dateFrom"
          class="border rounded px-2 py-1"
        />
      </div>

      <div class="grid gap-1">
        <label class="text-sm">To</label>
        <input
          type="date"
          v-model="pending.dateTo"
          class="border rounded px-2 py-1"
        />
      </div>

      <div class="grid gap-1 min-w-[220px]">
        <label class="text-sm">Asset</label>
        <select
          v-model="pending.assetId"
          class="border rounded px-2 py-1"
        >
          <option :value="null">All assets</option>
          <option
            v-for="a in assetOptions"
            :key="a.id"
            :value="a.id"
          >
            {{ a.name }}
          </option>
        </select>
      </div>

      <button
        class="border rounded px-3 py-2"
        @click="apply"
        :disabled="loading"
      >
        Apply
      </button>
    </div>

    <!-- TABLE -->
    <div class="overflow-x-auto border rounded">
      <table class="w-full text-sm">
        <thead class="border-b bg-gray-800 text-left">
          <tr>
            <th class="p-2">Date</th>
            <th class="p-2">Asset</th>
            <th class="p-2 text-right">Change</th>
            <th class="p-2 text-right">Dividend</th>
            <th class="p-2 text-right">Current Value</th>
            <th class="p-2">Notes</th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="r in rows"
            :key="r.id"
            class="border-b"
          >
            <td class="p-2">
              {{ r.date.slice(0, 10) }}
            </td>

            <td class="p-2">
              {{ r.assetName }}
            </td>

            <td class="p-2 text-right">
              {{ format(r.valueChange) }}
            </td>

            <td class="p-2 text-right">
              {{ format(r.dividend) }}
            </td>

            <td class="p-2 text-right">
              {{ format(r.currentValue) }}
            </td>

            <td class="p-2">
              {{ r.notes || '' }}
            </td>
          </tr>

          <tr v-if="!rows.length && !loading">
            <td colspan="6" class="p-4 text-center text-gray-500">
              No transactions
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- PAGINATION -->
    <div class="flex gap-2 items-center">
      <button
        class="border rounded px-3 py-1"
        @click="prev"
        :disabled="filters.page === 1 || loading"
      >
        Prev
      </button>

      <span>Page {{ filters.page }}</span>

      <button
        class="border rounded px-3 py-1"
        @click="next"
        :disabled="filters.page * filters.pageSize >= total || loading"
      >
        Next
      </button>

      <span class="text-sm text-gray-500 ml-2">
        Total: {{ total }}
      </span>
    </div>
  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { getTransactionHistory } from '@/services/transactionService'
import { getAssetsLookup } from '@/services/transactionService'

/* ======================
   STATE
====================== */
const rows = ref([])
const total = ref(0)
const loading = ref(false)

const assetOptions = ref([])

/**
 * pending = UI forma
 * filters = aktyvus API snapshot
 */
const pending = ref({
  dateFrom: new Date(
    new Date().setMonth(new Date().getMonth() - 1)
  ).toISOString().slice(0, 10),
  dateTo: new Date().toISOString().slice(0, 10),
  assetId: null,
  page: 1,
  pageSize: 25
})

const filters = ref({ ...pending.value })

/* ======================
   HELPERS
====================== */
function format(v) {
  if (v === null || v === undefined) return ''
  return Number(v).toLocaleString(undefined, {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  })
}

/* ======================
   API
====================== */
async function load() {
  loading.value = true
  try {
    const res = await getTransactionHistory(filters.value)
    rows.value = res.items
    total.value = res.total
  } finally {
    loading.value = false
  }
}

/* ======================
   ACTIONS
====================== */
function apply() {
  filters.value = {
    ...pending.value,
    page: 1
  }
  load()
}

function prev() {
  filters.value.page--
  pending.value.page = filters.value.page
  load()
}

function next() {
  filters.value.page++
  pending.value.page = filters.value.page
  load()
}

/* ======================
   INIT
====================== */
onMounted(async () => {
  assetOptions.value = await getAssetsLookup()
  load()
})
</script>
