<template>
  <section class="p-6 space-y-6">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <h1 class="text-2xl font-semibold text-gray-900">Assets</h1>
      <button
        class="rounded-md border border-gray-300 bg-white px-3 py-2
               text-sm font-medium text-gray-900 hover:bg-gray-50"
        @click="openCreateForm"
      >
        New Asset
      </button>
    </div>

    <!-- CREATE / EDIT FORM -->
    <form
      v-if="showForm"
      class="max-w-xl space-y-3 rounded-xl border border-gray-200 bg-white p-4 shadow-sm"
      @submit.prevent="save"
    >
      <h2 class="text-lg font-semibold text-gray-900">
        {{ editingId ? 'Edit Asset' : 'New Asset' }}
      </h2>

      <label class="text-sm font-medium text-gray-700">Asset</label>
      <input
        v-model="form.name"
        placeholder="Name"
        required
        class="w-full rounded-md border border-gray-300 bg-white px-3 py-2
               text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600/40"
      />

      <label class="text-sm font-medium text-gray-700">Category</label>
      <CategorySelect v-model="form.categoryId" />

      <label class="text-sm font-medium text-gray-700">Status</label>
      <select
        v-model.number="form.status"
        class="w-full rounded-md border border-gray-300 bg-white px-3 py-2
               text-gray-900 focus:outline-none focus:ring-2 focus:ring-blue-600/40"
      >
        <option disabled value="">Select status</option>
        <option
          v-for="s in statuses"
          :key="s.value"
          :value="s.value"
        >
          {{ s.label }}
        </option>
      </select>

      <div class="grid grid-cols-2 gap-3">
        <div>
          <label class="mb-1 block text-sm font-medium text-gray-700">
            Asset Currency
          </label>
          <select
            v-model="form.valueCurrency"
            class="w-full rounded-md border border-gray-300 bg-white px-3 py-2"
          >
            <option v-for="c in currencyOptions" :key="c">{{ c }}</option>
          </select>
        </div>

        <div>
          <label class="mb-1 block text-sm font-medium text-gray-700">
            Dividend Currency
          </label>
          <select
            v-model="form.dividendCurrency"
            class="w-full rounded-md border border-gray-300 bg-white px-3 py-2"
          >
            <option v-for="c in currencyOptions" :key="c">{{ c }}</option>
          </select>
        </div>
      </div>

      <div class="flex justify-end gap-2 pt-2">
        <button
          type="button"
          class="rounded-md border border-gray-300 bg-white px-3 py-2
                 text-sm hover:bg-gray-50"
          @click="cancel"
        >
          Cancel
        </button>

        <button
          class="rounded-md border border-blue-600 bg-blue-600 px-3 py-2
                 text-sm font-medium text-white hover:bg-blue-700"
        >
          {{ editingId ? 'Update' : 'Save' }}
        </button>
      </div>
    </form>

    <!-- TABLE -->
    <div class="overflow-x-auto rounded-xl border border-gray-200 bg-white shadow-sm">
      <table class="w-full text-sm">
        <thead class="bg-gray-100 border-b border-gray-200">
          <tr>
            <th class="p-2 text-left font-semibold text-gray-700">Name</th>
            <th class="p-2 text-left font-semibold text-gray-700">Category</th>
            <th class="p-2 text-left font-semibold text-gray-700">Status</th>
            <th class="p-2 text-right font-semibold text-gray-700">Total Invested</th>
            <th class="p-2 text-right font-semibold text-gray-700">Current Value</th>
            <th class="p-2 text-right font-semibold text-gray-700">Total Dividends</th>
            <th class="p-2"></th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="a in assets"
            :key="a.id"
            class="border-t border-gray-200 hover:bg-gray-50"
          >
            <td class="p-2 text-gray-900">{{ a.name }}</td>
            <td class="p-2 text-gray-900">{{ a.category.name || '-' }}</td>
            <td class="p-2 text-gray-900">{{ statusLabel(a.status) }}</td>

            <td class="p-2 text-right text-gray-900">
              {{ formatValue(a.totalInvested) }}
              <span class="ml-1 text-xs text-gray-400">{{ a.valueCurrency }}</span>
            </td>

            <td class="p-2 text-right text-gray-900">
              {{ formatValue(a.currentValue) }}
              <span class="ml-1 text-xs text-gray-400">{{ a.valueCurrency }}</span>
            </td>

            <td class="p-2 text-right text-gray-900">
              {{ formatValue(a.totalDividends) }}
              <span class="ml-1 text-xs text-gray-400">{{ a.dividendCurrency }}</span>
            </td>

            <td class="p-2 text-right space-x-2">
              <button
                class="rounded-md border border-gray-300 px-2 py-1 text-sm hover:bg-gray-50"
                @click="editAsset(a)"
              >
                Edit
              </button>
              <button
                class="rounded-md border border-red-300 px-2 py-1 text-sm text-red-600 hover:bg-red-50"
                @click="removeAsset(a.id)"
              >
                Delete
              </button>
            </td>
          </tr>

          <tr v-if="!assets.length && !loading">
            <td colspan="7" class="p-4 text-center text-gray-500">
              No assets yet
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import {
  getAssetsSummary,
  createAsset,
  updateAsset,
  deleteAsset
} from '@/services/assetService'
import CategorySelect from '@/components/CategorySelect.vue'

/* -------------------------
   STATE
-------------------------- */
const assets = ref([])
const loading = ref(false)

const showForm = ref(false)
const editingId = ref(null)

const currencyOptions = ['EUR', 'USD']

const statuses = [
  { value: 0, label: 'Active' },
  { value: 1, label: 'Inactive' }
]

const form = reactive({
  name: '',
  categoryId: null,
  status: 0,
  valueCurrency: 'EUR',
  dividendCurrency: 'EUR'
})

const date = new Date().toISOString().slice(0, 10)

/* -------------------------
   LOAD
-------------------------- */
async function loadAssets() {
  loading.value = true
  try {
    assets.value = await getAssetsSummary(date)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadAssets()
})

/* -------------------------
   FORM
-------------------------- */
function openCreateForm() {
  editingId.value = null
  showForm.value = true
  resetForm()
}

async function save() {
  const payload = {
    name: form.name.trim(),
    categoryId: form.categoryId,
    status: Number(form.status),
    valueCurrency: form.valueCurrency,
    dividendCurrency: form.dividendCurrency
  }

  if (editingId.value) {
    await updateAsset(editingId.value, payload)
  } else {
    await createAsset(payload)
  }

  await loadAssets()
  cancel()
}

function editAsset(a) {
  editingId.value = a.id
  showForm.value = true
  Object.assign(form, {
    name: a.name,
    categoryId: a.category.id,
    status: a.status,
    valueCurrency: a.valueCurrency,
    dividendCurrency: a.dividendCurrency
  })
}

async function removeAsset(id) {
  if (!confirm('Delete asset?')) return
  await deleteAsset(id)
  await loadAssets()
}

function cancel() {
  showForm.value = false
  resetForm()
}

function resetForm() {
  form.name = ''
  form.categoryId = null
  form.status = 0
  form.valueCurrency = 'EUR'
  form.dividendCurrency = 'EUR'
}

/* -------------------------
   HELPERS
-------------------------- */
function formatValue(v) {
  if (v == null) return '-'
  return Number(v).toLocaleString(undefined, {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  })
}

function statusLabel(s) {
  if (s === 0) return 'Active'
  if (s === 1) return 'Inactive'
  return '-'
}
</script>
