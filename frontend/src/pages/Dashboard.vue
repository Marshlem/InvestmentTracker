<template>
  <section class="space-y-6">
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

const route = useRoute()

const date = ref(new Date().toISOString().slice(0, 10))
const summary = ref(null)
const filters = ref(null)
const assets = ref([])

async function loadSummary() {
  summary.value = await getDashboardSummary(date.value)
}

function onApply(f) {
  filters.value = f
}

onMounted(async () => {
  assets.value = await getAssetsLookup()
  await loadSummary()
})
</script>
