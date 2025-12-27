<template>
  <section class="p-6 space-y-4 text-gray-100">
    <!-- Header -->
    <div class="flex items-center justify-between">
      <h1 class="text-2xl font-semibold">Assets</h1>
      <button class="border rounded px-3 py-2" @click="openCreateForm">
        New Asset
      </button>
    </div>

    <!-- CREATE / EDIT FORM -->
    <form
      v-if="showForm"
      class="grid gap-3 rounded-xl border border-gray-600 bg-gray-800 p-4 max-w-xl"
      @submit.prevent="save"
    >
      <h2 class="text-lg font-semibold">
        {{ editingId ? 'Edit Asset' : 'New Asset' }}
      </h2>

      <label class="text-sm">Asset</label>
      <input
        v-model="form.name"
        placeholder="Name"
        class="border rounded px-3 py-2 bg-gray-700"
        required
      />

      <label class="text-sm">Category</label>
      <CategorySelect v-model="form.categoryId" />

      <label class="text-sm">Status</label>
      <select v-model.number="form.status" class="border rounded px-3 py-2 bg-gray-700">
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
          <label class="text-sm block mb-1">Asset Currency</label>
          <select v-model="form.valueCurrency" class="border rounded px-3 py-2 bg-gray-700 w-full">
            <option v-for="c in currencyOptions" :key="c">{{ c }}</option>
          </select>
        </div>

        <div>
          <label class="text-sm block mb-1">Dividend Currency</label>
          <select v-model="form.dividendCurrency" class="border rounded px-3 py-2 bg-gray-700 w-full">
            <option v-for="c in currencyOptions" :key="c">{{ c }}</option>
          </select>
        </div>
      </div>

      <div class="flex justify-end gap-2 pt-2">
        <button type="button" class="border px-3 py-2" @click="cancel">
          Cancel
        </button>
        <button class="border px-3 py-2">
          {{ editingId ? 'Update' : 'Save' }}
        </button>
      </div>
    </form>

    <!-- TABLE -->
    <div class="overflow-x-auto border rounded-xl bg-gray-800">
      <table class="w-full text-sm">
        <thead class="bg-gray-900">
          <tr>
            <th class="p-2 text-left">Name</th>
            <th class="p-2 text-left">Category</th>
            <th class="p-2 text-left">Status</th>
            <th class="p-2 text-right">Total Invested</th>
            <th class="p-2 text-right">Current Value</th>
            <th class="p-2 text-right">Total Dividends</th>
            <th class="p-2 text-right"></th>
          </tr>
        </thead>

        <tbody>
          <tr
            v-for="a in assets"
            :key="a.id"
            class="border-t border-gray-700 hover:bg-gray-700/40"
          >
            <td class="p-2">{{ a.name }}</td>
            <td class="p-2">{{ a.category.name || '-' }}</td>
            <td class="p-2">
              {{ statusLabel(a.status) }}
            </td>

            <td class="p-2 text-right">
              {{ formatValue(a.totalInvested) }}
              <span class="text-xs text-gray-400">{{ a.valueCurrency }}</span>
            </td>

            <td class="p-2 text-right">
              {{ formatValue(a.currentValue) }}
              <span class="text-xs text-gray-400">{{ a.valueCurrency }}</span>
            </td>

            <td class="p-2 text-right">
              {{ formatValue(a.totalDividends) }}
              <span class="text-xs text-gray-400">{{ a.dividendCurrency }}</span>
            </td>

            <td class="p-2 text-right space-x-2">
              <button class="border px-2 py-1" @click="editAsset(a)">
                Edit
              </button>
              <button class="border px-2 py-1 text-red-400" @click="removeAsset(a.id)">
                Delete
              </button>
            </td>
          </tr>

          <tr v-if="!assets.length && !loading">
            <td colspan="7" class="text-center text-gray-500 p-4">
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
