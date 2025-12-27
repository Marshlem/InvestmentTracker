<template>
  <div class="grid gap-4 md:grid-cols-3">
    <div class="rounded-xl border p-4">
      <div class="text-sm opacity-70">Total Invested</div>
      <div class="text-2xl font-semibold">
        {{ currency(summary.totalInvested) }}
      </div>
    </div>

    <div class="rounded-xl border p-4">
      <div class="text-sm opacity-70">Current Value</div>
      <div class="text-2xl font-semibold">
        {{ currency(summary.currentValue) }}
      </div>
    </div>

    <div class="rounded-xl border p-4">
      <div class="text-sm opacity-70">P/L</div>
      <div :class="plClass">
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
