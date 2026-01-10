<template>
  <div class="border rounded-xl bg-gray-900 overflow-x-auto">
    <table class="w-full text-sm">
      <thead class="bg-gray-800">
        <tr>
          <th class="px-3 py-2 text-left">Investment</th>
          <th class="px-3 py-2 text-right">Total Invested</th>
          <th class="px-3 py-2 text-right">Total Value</th>
          <th class="px-3 py-2 text-right">Dividends</th>
          <th class="px-3 py-2 text-right">Gain / Loss</th>
          <th class="px-3 py-2 text-right">% Gain / Loss</th>
          <th class="px-3 py-2 text-right">Gain / Loss (Last month)</th>
          <th class="px-3 py-2 text-right">% (Last month)</th>
          <th class="px-3 py-2 text-right">Invested %</th>
        </tr>
      </thead>

      <tbody>
        <tr
          v-for="row in rows"
          :key="row.investment"
          class="border-t border-gray-800"
        >
          <td class="px-3 py-2">{{ row.investment }}</td>

          <td class="px-3 py-2 text-right">{{ eur(row.totalInvested) }}</td>
          <td class="px-3 py-2 text-right">{{ eur(row.totalValue) }}</td>
          <td class="px-3 py-2 text-right">{{ eur(row.totalDividends) }}</td>

          <td
            class="px-3 py-2 text-right"
            :class="row.gainLoss >= 0 ? 'text-green-400' : 'text-red-400'"
          >
            {{ eur(row.gainLoss) }}
          </td>

          <td
            class="px-3 py-2 text-right"
            :class="row.gainLossPercent >= 0 ? 'text-green-400' : 'text-red-400'"
          >
            {{ row.gainLossPercent.toFixed(2) }}%
          </td>

          <td
            class="px-3 py-2 text-right"
            :class="row.gainLossLastMonth >= 0 ? 'text-green-400' : 'text-red-400'"
          >
            {{ eur(row.gainLossLastMonth) }}
          </td>

          <td
            class="px-3 py-2 text-right"
            :class="row.gainLossLastMonthPercent >= 0 ? 'text-green-400' : 'text-red-400'"
          >
            {{ row.gainLossLastMonthPercent.toFixed(2) }}%
          </td>

          <td class="px-3 py-2 text-right">
            {{ row.investedPortfolioPercent.toFixed(2) }}%
          </td>
        </tr>

        <!-- TOTAL ROW -->
        <tr class="border-t border-gray-700 font-semibold bg-gray-800">
        <td class="px-3 py-2">TOTAL</td>
        <td class="px-3 py-2 text-right">{{ eur(total.totalInvested) }}</td>
        <td class="px-3 py-2 text-right">{{ eur(total.totalValue) }}</td>
        <td class="px-3 py-2 text-right">{{ eur(total.totalDividends) }}</td>
        <td class="px-3 py-2 text-right">{{ eur(total.gainLoss) }}</td>
        <td class="px-3 py-2 text-right">{{ total.gainLossPercent.toFixed(2) }}%</td>
        <td class="px-3 py-2 text-right">{{ eur(total.gainLossLastMonth) }}</td>
        <td class="px-3 py-2 text-right">{{ total.gainLossLastMonthPercent.toFixed(2) }}%</td>
        <td class="px-3 py-2 text-right">100%</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  rows: Array,
  total: Object
})

const eur = v =>
  Number(v ?? 0).toLocaleString(undefined, {
    style: 'currency',
    currency: 'EUR'
  })
</script>
