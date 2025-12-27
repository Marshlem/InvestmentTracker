<template>
  <div class="border rounded-xl p-4 bg-gray-900">
    <VChart
      v-if="option"
      :option="option"
      autoresize
      style="height: 420px;"
    />
    <div v-else class="text-gray-500 text-sm">
      Loading chart…
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { use } from 'echarts/core'
import VChart from 'vue-echarts'
import {LineChart} from 'echarts/charts'
import {TooltipComponent, LegendComponent, GridComponent} from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import { getDashboardChart } from '@/services/dashboardService'

/* -----------------------
   ECharts registration
------------------------ */
use([
  LineChart,
  TooltipComponent,
  LegendComponent,
  GridComponent,
  CanvasRenderer
])

/* -----------------------
   Props
------------------------ */
const props = defineProps({
  filters: {
    type: Object,
    required: true
  }
})

const option = ref(null)

/* -----------------------
   Helpers
------------------------ */
const eur = v =>
  Number(v ?? 0).toLocaleString(undefined, {
    style: 'currency',
    currency: 'EUR'
  })

/* -----------------------
   Load + map data
------------------------ */
async function load(filters) {
  const res = await getDashboardChart(filters)

  const rows = res.series ?? []

  const labels = rows.map(r => r.period)

  option.value = {
    backgroundColor: 'transparent',

    tooltip: {
      trigger: 'axis',
      valueFormatter: eur
    },

    legend: {
      top: 0,
      textStyle: { color: '#e5e7eb' }
    },

    grid: {
      top: 40,
      left: 60,
      right: 30,
      bottom: 40
    },

    xAxis: {
      type: 'category',
      data: labels,
      axisLabel: { color: '#9ca3af' }
    },

    yAxis: {
      type: 'value',
      axisLabel: {
        color: '#9ca3af',
        formatter: eur
      }
    },

series: [
  {
    name: 'Invested',
    type: 'line',
    smooth: true,
    symbol: 'circle',
    symbolSize: 8,
    data: rows.map(r => r.invested),
    lineStyle: { width: 3 },
    label: {
      show: true,
      position: 'top',
      formatter: p => eur(p.value)
    }
  },
  {
    name: 'Profit / Loss',
    type: 'line',
    smooth: true,
    symbol: 'diamond',
    symbolSize: 8,
    data: rows.map(r => r.profitLoss),
    lineStyle: { width: 3 },
    label: {
      show: true,
      position: 'top',
      formatter: p => eur(p.value)
    }
  },
  {
    name: 'Current Value',
    type: 'line',
    smooth: true,
    symbol: 'rect',
    symbolSize: 8,
    data: rows.map(r => r.currentValue),
    lineStyle: { width: 3 },
    label: {
      show: true,
      position: 'top',
      formatter: p => eur(p.value)
    }
  }
]

  }
}

/* -----------------------
   React to filters
------------------------ */
watch(
  () => props.filters,
  f => {
    if (f) load(f)
  },
  { immediate: true, deep: true }
)
</script>
