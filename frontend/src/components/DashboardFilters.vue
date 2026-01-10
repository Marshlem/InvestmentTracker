<template>
  <div class="flex items-end gap-4 flex-wrap">

    <!-- From -->
    <div class="flex flex-col">
      <label class="text-sm">From</label>
      <input
        type="date"
        v-model="local.dateFrom"
        class="border rounded px-2 py-1"
      />
    </div>

    <!-- To -->
    <div class="flex flex-col">
      <label class="text-sm">To</label>
      <input
        type="date"
        v-model="local.dateTo"
        class="border rounded px-2 py-1"
      />
    </div>

    <!-- Group by -->
    <div class="flex flex-col">
      <label class="text-sm">Group by</label>
      <select
        v-model="local.groupBy"
        class="border rounded px-2 py-1"
      >
        <option value="month">Month</option>
        <option value="quarter">Quarter</option>
        <option value="year">Year</option>
      </select>
    </div>

    <!-- Asset -->
    <div class="flex flex-col min-w-[220px]">
      <label class="text-sm">Asset</label>
      <select
        v-model="local.assetId"
        class="border rounded px-2 py-1"
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
      class="border px-4 py-2 rounded h-[38px]"
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
