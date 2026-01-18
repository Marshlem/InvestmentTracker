<template>
  <div class="grid gap-4 md:grid-cols-4">
    <div class="rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
      <div class="text-sm text-gray-500">Total Invested</div>
      <div class="text-2xl font-semibold text-gray-900">
        {{ currency(summary.totalInvested) }}
      </div>
    </div>

    <div class="rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
      <div class="text-sm text-gray-500">Current Value</div>
      <div class="text-2xl font-semibold text-gray-900">
        {{ currency(summary.currentValue) }}
      </div>
    </div>

    <div class="rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
      <div class="text-sm text-gray-500">Total Dividends</div>
      <div class="text-2xl font-semibold text-gray-900">
        {{ currency(summary.totalDividends) }}
      </div>
    </div>

    <div class="rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
      <div class="text-sm text-gray-500">P/L</div>
      <div class="text-2xl font-semibold" :class="plClass">
        {{ currency(summary.profitLoss) }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  summary: {
    type: Object,
    required: true
  }
})

const plClass = computed(() =>
  `text-2xl font-semibold ${
    props.summary.profitLoss >= 0
      ? 'text-green-600'
      : 'text-red-600'
  }`
)

const currency = (v = 0) =>
  Number(v).toLocaleString(undefined, {
    style: 'currency',
    currency: 'EUR'
  })
</script>
