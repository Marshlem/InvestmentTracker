<template>
  <div class="space-y-6">
    <!-- Portfolio Value -->
    <div class="border rounded-xl p-4 bg-gray-900">
      <VChart
        v-if="portfolioOption"
        :option="portfolioOption"
        autoresize
        style="height: 420px;"
      />
    </div>

    <!-- Profit / Loss -->
    <div class="border rounded-xl p-4 bg-gray-900">
      <VChart
        v-if="profitLossOption"
        :option="profitLossOption"
        autoresize
        style="height: 360px;"
      />
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { use } from 'echarts/core'
import VChart from 'vue-echarts'
import { LineChart } from 'echarts/charts'
import { TooltipComponent, LegendComponent, GridComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import { getDashboardChart } from '@/services/dashboardService'

use([
  LineChart,
  TooltipComponent,
  LegendComponent,
  GridComponent,
  CanvasRenderer
])

const props = defineProps({
  filters: Object
})

const portfolioOption = ref(null)
const profitLossOption = ref(null)

const eur = v =>
  Number(v ?? 0).toLocaleString(undefined, {
    style: 'currency',
    currency: 'EUR'
  })

async function load(filters) {
  const res = await getDashboardChart(filters ?? {})

  buildPortfolioChart(res.portfolioValue ?? [])
  buildProfitLossChart(res.profitLoss ?? [])
}

/* -----------------------
   Portfolio Value chart
------------------------ */
function buildPortfolioChart(rows) {
  portfolioOption.value = {
    backgroundColor: 'transparent',
    tooltip: { trigger: 'axis', valueFormatter: eur },
    legend: { top: 0, textStyle: { color: '#e5e7eb' } },
    grid: { top: 40, left: 60, right: 30, bottom: 40 },

    xAxis: {
      type: 'category',
      data: rows.map(r => r.period),
      axisLabel: { color: '#9ca3af' }
    },

    yAxis: {
      type: 'value',
      axisLabel: { color: '#9ca3af', formatter: eur }
    },

    series: [
      {
        name: 'Invested (accumulated)',
        type: 'line',
        smooth: true,
        data: rows.map(r => r.investedAccumulated),
        lineStyle: { width: 3 }
      },
      {
        name: 'Current Value',
        type: 'line',
        smooth: true,
        data: rows.map(r => r.currentValue),
        lineStyle: { width: 3 }
      }
    ]
  }
}

/* -----------------------
   Profit / Loss chart
------------------------ */
function buildProfitLossChart(rows) {
  profitLossOption.value = {
    backgroundColor: 'transparent',
    tooltip: { trigger: 'axis', valueFormatter: eur },
    legend: { top: 0, textStyle: { color: '#e5e7eb' } },
    grid: { top: 40, left: 60, right: 30, bottom: 40 },

    xAxis: {
      type: 'category',
      data: rows.map(r => r.period),
      axisLabel: { color: '#9ca3af' }
    },

    yAxis: {
      type: 'value',
      axisLabel: { color: '#9ca3af', formatter: eur }
    },

    series: [
      {
        name: 'Profit / Loss',
        type: 'line',
        smooth: true,
        data: rows.map(r => r.periodProfitLoss),
        lineStyle: { width: 2 }
      },
      {
        name: 'Profit / Loss (accumulated)',
        type: 'line',
        smooth: true,
        data: rows.map(r => r.accumulatedProfitLoss),
        lineStyle: { width: 3 }
      }
    ]
  }
}

watch(
  () => props.filters,
  f => load(f),
  { immediate: true, deep: true }
)
</script>
