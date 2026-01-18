<template>
  <div class="overflow-x-auto rounded-xl border border-gray-200 bg-white shadow-sm">
    <table class="w-full text-sm">
      <thead class="bg-gray-100 border-b border-gray-200">
        <tr>
          <th class="px-3 py-2 text-left font-semibold text-gray-700">Investment</th>
          <th class="px-3 py-2 text-right font-semibold text-gray-700">Total Invested</th>
          <th class="px-3 py-2 text-right font-semibold text-gray-700">Total Value</th>
          <th class="px-3 py-2 text-right font-semibold text-gray-700">Dividends</th>
          <th class="px-3 py-2 text-right font-semibold text-gray-700">Gain / Loss</th>
          <th class="px-3 py-2 text-right font-semibold text-gray-700">% Gain / Loss</th>
          <th class="px-3 py-2 text-right font-semibold text-gray-700">Gain / Loss (Last month)</th>
          <th class="px-3 py-2 text-right font-semibold text-gray-700">% (Last month)</th>
          <th class="px-3 py-2 text-right font-semibold text-gray-700">Invested %</th>
        </tr>
      </thead>

      <tbody>
        <tr
          v-for="row in rows"
          :key="row.investment"
          class="border-t border-gray-200 hover:bg-gray-50"
        >
          <td class="px-3 py-2 text-gray-900">{{ row.investment }}</td>

          <td class="px-3 py-2 text-right text-gray-900">{{ eur(row.totalInvested) }}</td>
          <td class="px-3 py-2 text-right text-gray-900">{{ eur(row.totalValue) }}</td>
          <td class="px-3 py-2 text-right text-gray-900">{{ eur(row.totalDividends) }}</td>

          <td
            class="px-3 py-2 text-right"
            :class="row.gainLoss >= 0 ? 'text-green-600' : 'text-red-600'"
          >
            {{ eur(row.gainLoss) }}
          </td>

          <td
            class="px-3 py-2 text-right"
            :class="row.gainLossPercent >= 0 ? 'text-green-600' : 'text-red-600'"
          >
            {{ row.gainLossPercent.toFixed(2) }}%
          </td>

          <td
            class="px-3 py-2 text-right"
            :class="row.gainLossLastMonth >= 0 ? 'text-green-600' : 'text-red-600'"
          >
            {{ eur(row.gainLossLastMonth) }}
          </td>

          <td
            class="px-3 py-2 text-right"
            :class="row.gainLossLastMonthPercent >= 0 ? 'text-green-600' : 'text-red-600'"
          >
            {{ row.gainLossLastMonthPercent.toFixed(2) }}%
          </td>

          <td class="px-3 py-2 text-right text-gray-900">
            {{ row.investedPortfolioPercent.toFixed(2) }}%
          </td>
        </tr>

        <!-- TOTAL ROW -->
        <tr class="border-t border-gray-300 bg-gray-100 font-semibold">
          <td class="px-3 py-2 text-gray-900">TOTAL</td>
          <td class="px-3 py-2 text-right text-gray-900">{{ eur(total.totalInvested) }}</td>
          <td class="px-3 py-2 text-right text-gray-900">{{ eur(total.totalValue) }}</td>
          <td class="px-3 py-2 text-right text-gray-900">{{ eur(total.totalDividends) }}</td>
          <td class="px-3 py-2 text-right text-gray-900">{{ eur(total.gainLoss) }}</td>
          <td class="px-3 py-2 text-right text-gray-900">{{ total.gainLossPercent.toFixed(2) }}%</td>
          <td class="px-3 py-2 text-right text-gray-900">{{ eur(total.gainLossLastMonth) }}</td>
          <td class="px-3 py-2 text-right text-gray-900">{{ total.gainLossLastMonthPercent.toFixed(2) }}%</td>
          <td class="px-3 py-2 text-right text-gray-900">100%</td>
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
