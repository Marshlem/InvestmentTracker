<template>
  <section class="p-6 space-y-6">
    <h1 class="text-2xl font-semibold">Reports</h1>

    <!-- TABS -->
    <div class="flex gap-2 border-b pb-2">
      <button
        class="px-3 py-1 rounded"
        :class="tab === 'graphs' ? 'bg-gray-700' : 'bg-gray-800'"
        @click="tab = 'graphs'"
      >
        Graphs
      </button>

      <button
        class="px-3 py-1 rounded"
        :class="tab === 'tables' ? 'bg-gray-700' : 'bg-gray-800'"
        @click="tab = 'tables'"
      >
        Tables
      </button>
    </div>

    <!-- GRAPHS TAB -->
    <div v-if="tab === 'graphs'" class="space-y-6">

      <!-- SUMMARY -->
      <DashboardSummary
        v-if="summary"
        :summary="summary"
      />

      <!-- FILTERS -->
      <DashboardFilters
        :assets="assets"
        @apply="onApply"
      />

      <!-- CHART -->
      <DashboardChart :filters="filters" />

    </div>

    <!-- TABLES TAB -->
    <div v-else>
      <DashboardInvestmentTable
      :rows="investmentTable"
      :total="investmentTotal"
/>
    </div>

  </section>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import DashboardSummary from '@/components/DashboardSummary.vue'
import DashboardFilters from '@/components/DashboardFilters.vue'
import DashboardChart from '@/components/DashboardChart.vue'
import { getDashboardSummary } from '@/services/dashboardService'
import { getAssetsLookup } from '@/services/transactionService'
import DashboardInvestmentTable from '@/components/DashboardInvestmentTable.vue'
import { getInvestmentTable } from '@/services/dashboardService'

const route = useRoute()

const date = ref(new Date().toISOString().slice(0, 10))
const summary = ref(null)
const filters = ref(null)
const assets = ref([])
const tab = ref('graphs')
const investmentTable = ref([])
const investmentTotal = ref(null)

async function loadSummary() {
  summary.value = await getDashboardSummary(date.value)
}

function onApply(f) {
  filters.value = f
  loadInvestmentTable(f)
}

async function loadInvestmentTable() {
  const res = await getInvestmentTable()
  investmentTable.value = res.rows
  investmentTotal.value = res.total
}

onMounted(async () => {
  assets.value = await getAssetsLookup()
  await loadSummary()
  await loadInvestmentTable()
})
</script>
