<template>
  <div class="space-y-4">

  <div
    class="relative flex flex-col items-center justify-center rounded-lg border-2 border-dashed p-6 text-center transition"
    :class="isDragging
      ? 'border-blue-500 bg-blue-50'
      : 'border-gray-300 bg-gray-50 hover:bg-gray-100'"
    @dragenter.prevent="isDragging = true"
    @dragover.prevent
    @dragleave.prevent="isDragging = false"
    @drop.prevent="onDrop"
  >
    <input
      ref="fileInput"
      type="file"
      accept=".xlsx"
      class="hidden"
      @change="onFileChange"
    />

    <p class="text-sm text-gray-700">
      <span class="font-medium text-blue-600 cursor-pointer" @click="openFileDialog">
        Click to upload
      </span>
      or drag & drop Excel file here
    </p>

    <p v-if="file" class="mt-2 text-xs text-gray-500">
      Selected: {{ file.name }}
    </p>
  </div>

  <button
    class="rounded bg-blue-600 px-4 py-2 text-white disabled:opacity-50"
    :disabled="!file"
    @click="loadPreview"
  >
    Preview
  </button>

    <!-- Errors -->
    <div v-if="previewData?.errors?.length" class="text-red-600">
      <div v-for="e in previewData.errors" :key="e.row + e.field">
        Row {{ e.row }} – {{ e.field }}: {{ e.message }}
      </div>
    </div>

    <!-- Preview table -->
    <div v-if="previewData" class="overflow-x-auto rounded-lg border border-gray-200">
      <table class="min-w-full table-fixed text-sm">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-3 py-2 text-left font-semibold text-gray-700">Date</th>
            <th class="px-3 py-2 text-left font-semibold text-gray-700">Asset</th>

            <th class="px-3 py-2 text-right font-semibold text-gray-700">Value</th>
            <th class="px-3 py-2 text-right font-semibold text-gray-700">Current</th>
            <th class="px-3 py-2 text-right font-semibold text-gray-700">Dividend</th>

            <th class="px-3 py-2 text-left font-semibold text-gray-700">Notes</th>
            <th class="px-3 py-2 text-left font-semibold text-gray-700">Status</th>
          </tr>
        </thead>

        <tbody class="divide-y divide-gray-200 bg-white">
          <tr v-for="r in previewData.rows" :key="r.row" class="hover:bg-gray-50">
            <td class="px-3 py-2 whitespace-nowrap text-gray-900">
              {{ formatYmd(r.date) }}
            </td>

            <td class="px-3 py-2 whitespace-nowrap text-gray-900 truncate" :title="r.asset">
              {{ r.asset }}
            </td>

            <td class="px-3 py-2 whitespace-nowrap text-right text-gray-900">
              {{ r.valueChange ?? '' }}
            </td>

            <td class="px-3 py-2 whitespace-nowrap text-right text-gray-900">
              {{ r.currentValue ?? '' }}
            </td>

            <td class="px-3 py-2 whitespace-nowrap text-right text-gray-900">
              {{ r.dividend ?? '' }}
            </td>

            <td class="px-3 py-2 text-gray-900 truncate" :title="r.notes || ''">
              {{ r.notes || '' }}
            </td>

            <td class="px-3 py-2 text-gray-700" :title="r.status">
              {{ r.status }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <button
      v-if="previewData?.canImport"
      class="rounded bg-green-600 px-4 py-2 text-white"
      @click="confirmImport"
    >
      Confirm import
    </button>

  </div>
</template>

<script setup>
import { ref } from 'vue'
import { formatYmd } from '@/utils/formatYmd'
import {
  previewTransactionsImport,
  confirmTransactionsImport
} from '@/services/transactionService'

const file = ref(null)
const previewData = ref(null)
const isDragging = ref(false)
const fileInput = ref(null)

function openFileDialog() {
  fileInput.value?.click()
}

function onFileChange(e) {
  const selected = e.target.files?.[0]
  if (selected) {
    file.value = selected
  }
}

function onDrop(e) {
  isDragging.value = false

  const dropped = e.dataTransfer.files?.[0]
  if (!dropped) return

  if (!dropped.name.endsWith('.xlsx')) {
    alert('Only .xlsx files are supported')
    return
  }

  file.value = dropped
}

async function loadPreview() {
  previewData.value = await previewTransactionsImport(file.value)
}

async function confirmImport() {
  await confirmTransactionsImport(previewData.value.importRows)
}
</script>
