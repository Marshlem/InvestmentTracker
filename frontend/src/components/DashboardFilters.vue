<template>
  <div class="flex flex-wrap items-end gap-4">
    <!-- From -->
    <div class="flex flex-col">
      <label class="text-sm font-medium text-gray-700">From</label>
      <input
        type="date"
        v-model="local.dateFrom"
        class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-gray-900
               focus:outline-none focus:ring-2 focus:ring-blue-600/40"
      />
    </div>

    <!-- To -->
    <div class="flex flex-col">
      <label class="text-sm font-medium text-gray-700">To</label>
      <input
        type="date"
        v-model="local.dateTo"
        class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-gray-900
               focus:outline-none focus:ring-2 focus:ring-blue-600/40"
      />
    </div>

    <!-- Group by -->
    <div class="flex flex-col">
      <label class="text-sm font-medium text-gray-700">Group by</label>
      <select
        v-model="local.groupBy"
        class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-gray-900
               focus:outline-none focus:ring-2 focus:ring-blue-600/40"
      >
        <option value="month">Month</option>
        <option value="quarter">Quarter</option>
        <option value="year">Year</option>
      </select>
    </div>

    <!-- Asset -->
    <div class="flex flex-col min-w-[220px]">
      <label class="text-sm font-medium text-gray-700">Asset</label>
      <select
        v-model="local.assetId"
        class="w-full rounded-md border border-gray-300 bg-white px-2 py-1 text-gray-900
               focus:outline-none focus:ring-2 focus:ring-blue-600/40"
      >
        <option :value="null">All assets</option>
        <option
          v-for="a in assets"
          :key="a.id"
          :value="a.id"
        >
          {{ a.name }}
        </option>
      </select>
    </div>

    <!-- Apply -->
    <button
      class="h-[38px] rounded-md border border-blue-600 bg-blue-600 px-4 py-2
             text-sm font-medium text-white hover:bg-blue-700"
      type="button"
      @click="apply"
    >
      Apply
    </button>
  </div>
</template>

<script setup>
import { reactive } from 'vue'

defineProps({ assets: Array })

const emit = defineEmits(['apply'])

const local = reactive({
  dateFrom: null,
  dateTo: null,
  groupBy: 'month',
  assetId: null
})

function apply() {
  emit('apply', {
    dateFrom: local.dateFrom || null,
    dateTo: local.dateTo || null,
    groupBy: local.groupBy,
    assetIds: local.assetId ? [local.assetId] : null
  })
}
</script>
