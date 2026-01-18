<template>
  <section class="p-4 space-y-4">
    <h2 class="text-xl font-semibold text-gray-900">Transaction History</h2>

    <!-- FILTERS -->
    <div class="flex flex-wrap gap-3 items-end">
      <div class="grid gap-1">
        <label class="text-sm font-medium text-gray-700">From</label>
        <input
          type="date"
          v-model="pending.dateFrom"
          class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-gray-900
                 focus:outline-none focus:ring-2 focus:ring-blue-600/40"
        />
      </div>

      <div class="grid gap-1">
        <label class="text-sm font-medium text-gray-700">To</label>
        <input
          type="date"
          v-model="pending.dateTo"
          class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-gray-900
                 focus:outline-none focus:ring-2 focus:ring-blue-600/40"
        />
      </div>

      <div class="grid gap-1 min-w-[220px]">
        <label class="text-sm font-medium text-gray-700">Asset</label>
        <select
          v-model="pending.assetId"
          class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-gray-900
                 focus:outline-none focus:ring-2 focus:ring-blue-600/40"
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
        class="rounded-md border border-blue-600 bg-blue-600 px-3 py-2 text-sm font-medium text-white
               hover:bg-blue-700 disabled:opacity-60 disabled:cursor-not-allowed"
        @click="apply"
        :disabled="loading"
        type="button"
      >
        Apply
      </button>
    </div>

    <!-- TABLE -->
    <div class="overflow-x-auto rounded-xl border border-gray-200 bg-white shadow-sm">
      <table class="w-full text-sm">
        <thead class="border-b border-gray-200 bg-gray-100 text-left">
          <tr>
            <th class="p-2 font-semibold text-gray-700">Date</th>
            <th class="p-2 font-semibold text-gray-700">Asset</th>
            <th class="p-2 text-right font-semibold text-gray-700">Change</th>
            <th class="p-2 text-right font-semibold text-gray-700">Dividend</th>
            <th class="p-2 text-right font-semibold text-gray-700">Current Value</th>
            <th class="p-2 font-semibold text-gray-700">Notes</th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="r in rows"
            :key="r.id"
            class="border-b border-gray-200 hover:bg-gray-50"
          >
            <td class="p-2 text-gray-900">
              {{ r.date.slice(0, 10) }}
            </td>

            <td class="p-2 text-gray-900">
              {{ r.assetName }}
            </td>

            <td class="p-2 text-right text-gray-900">
              {{ format(r.valueChange) }}
            </td>

            <td class="p-2 text-right text-gray-900">
              {{ format(r.dividend) }}
            </td>

            <td class="p-2 text-right text-gray-900">
              {{ format(r.currentValue) }}
            </td>

            <td class="p-2 text-gray-900">
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
    <div class="flex flex-wrap gap-2 items-center">
      <button
        class="rounded-md border border-gray-300 bg-white px-3 py-1 text-sm
               hover:bg-gray-50 disabled:opacity-60 disabled:cursor-not-allowed"
        @click="prev"
        :disabled="filters.page === 1 || loading"
        type="button"
      >
        Prev
      </button>

      <span class="text-sm text-gray-900">Page {{ filters.page }}</span>

      <button
        class="rounded-md border border-gray-300 bg-white px-3 py-1 text-sm
               hover:bg-gray-50 disabled:opacity-60 disabled:cursor-not-allowed"
        @click="next"
        :disabled="filters.page * filters.pageSize >= total || loading"
        type="button"
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
