<template>
  <div class="flex flex-wrap gap-3 items-end">
    <div>
      <label>From</label>
      <input type="date" v-model="local.dateFrom" />
    </div>

    <div>
      <label>To</label>
      <input type="date" v-model="local.dateTo" />
    </div>

    <div>
      <label>Group by</label>
      <select v-model="local.groupBy">
        <option value="month">Month</option>
        <option value="quarter">Quarter</option>
        <option value="year">Year</option>
      </select>
    </div>

    <div class="grid gap-1 min-w-[220px]">
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

    <button class="border px-3 py-2" @click="apply">
      Apply
    </button>
  </div>
</template>

<script setup>
import { reactive } from 'vue'

defineProps({ assets: Array })

const emit = defineEmits(['apply'])

const local = reactive({
  dateFrom: new Date(new Date().setMonth(new Date().getMonth() - 6))
    .toISOString()
    .slice(0, 10),
  dateTo: new Date().toISOString().slice(0, 10),
  groupBy: 'month',
  assetId: null
})

function apply() {
  emit('apply', {
    dateFrom: local.dateFrom,
    dateTo: local.dateTo,
    groupBy: local.groupBy,
    assetIds: local.assetId ? [local.assetId] : null
  })
}
</script>
