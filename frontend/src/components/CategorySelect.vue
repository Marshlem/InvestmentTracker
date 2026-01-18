<template>
  <div class="space-y-2">
    <select
      v-model="internalValue"
      class="w-full rounded-md border border-gray-300 bg-white px-3 py-2 text-gray-900
             focus:outline-none focus:ring-2 focus:ring-blue-600/40"
    >
      <option disabled value="">Select category</option>

      <option
        v-for="c in activeCategories"
        :key="c.id"
        :value="c.id"
      >
        {{ c.name }}
      </option>

      <option value="__new__">➕ Add new…</option>
      <option value="__manage__">⚙️ Manage…</option>
    </select>

    <!-- Manage section -->
    <div v-if="managing" class="rounded-xl border border-gray-200 bg-white p-3 space-y-2 shadow-sm">
      <div
        v-for="c in categories"
        :key="c.id"
        class="flex items-center gap-2"
      >
        <template v-if="editingId !== c.id">
          <span :class="c.isActive ? 'text-gray-900' : 'text-gray-500 line-through'">
            {{ c.name }}
          </span>

          <span class="text-gray-300">➜</span>

          <button
            type="button"
            class="rounded-md border border-gray-300 bg-white px-2 py-1 text-sm hover:bg-gray-50"
            @click="startEdit(c)"
          >
            ✏️
          </button>

          <button
            type="button"
            class="rounded-md border border-gray-300 bg-white px-2 py-1 text-sm hover:bg-gray-50"
            @click="toggleActive(c)"
          >
            {{ c.isActive ? '❌' : '✔️' }}
          </button>
        </template>

        <template v-else>
          <input
            v-model="tempName"
            class="w-full max-w-[260px] rounded-md border border-gray-300 bg-white px-2 py-1 text-gray-900
                   focus:outline-none focus:ring-2 focus:ring-blue-600/40"
            @keydown.enter.stop.prevent
          />

          <button
            type="button"
            class="rounded-md border border-gray-300 bg-white px-2 py-1 text-sm hover:bg-gray-50"
            @click="saveEdit(c)"
          >
            💾
          </button>

          <button
            type="button"
            class="rounded-md border border-gray-300 bg-white px-2 py-1 text-sm hover:bg-gray-50"
            @click="cancelEdit"
          >
            ✖️
          </button>
        </template>
      </div>

      <!-- New category -->
      <div class="flex gap-2 pt-2">
        <input
          v-model="newCategory"
          placeholder="New category name"
          class="flex-1 rounded-md border border-gray-300 bg-white px-2 py-1 text-gray-900
                 focus:outline-none focus:ring-2 focus:ring-blue-600/40"
          @keydown.enter.stop.prevent
        />

        <button
          type="button"
          class="rounded-md border border-blue-600 bg-blue-600 px-3 py-1 text-sm font-medium text-white
                 hover:bg-blue-700"
          @click="addCategory"
        >
          Add
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, computed, onMounted } from 'vue'
import {getCategories, createCategory, renameCategory, deleteCategory} from '@/services/categoryService'

/* -------- props -------- */
const props = defineProps({
  modelValue: {
    type: Number,
    default: null
  }
})

const emit = defineEmits(['update:modelValue'])

/* -------- state -------- */
const categories = ref([])
const editingId = ref(null)
const tempName = ref('')
const newCategory = ref('')
const managing = ref(false)

const internalValue = ref(props.modelValue ?? '')

/* -------- computed -------- */
const activeCategories = computed(() =>
  categories.value.filter(c => c.isActive)
)

/* -------- watch two way binding -------- */
watch(
  () => props.modelValue,
  v => (internalValue.value = v)
)

watch(
  () => internalValue.value,
  async v => {
    if (v === '__new__') {
      managing.value = true
      internalValue.value = null
      return
    }

    if (v === '__manage__') {
      managing.value = !managing.value
      internalValue.value = null
      return
    }

    emit('update:modelValue', v)
  }
)

/* -------- actions -------- */

async function load() {
  categories.value = await getCategories()
}

function startEdit(c) {
  editingId.value = c.id
  tempName.value = c.name
}

function cancelEdit() {
  editingId.value = null
}

async function saveEdit(c) {
  await renameCategory(c.id, {
    name: tempName.value,
    isActive: c.isActive
  })
  editingId.value = null
  await load()
}

async function toggleActive(c) {
  if (c.isActive) {
    await deleteCategory(c.id)  
  } else {
    await renameCategory(c.id, { isActive: true })
  }

  await load()
}

async function addCategory() {
  if (!newCategory.value.trim()) return

  await createCategory(newCategory.value.trim())

  newCategory.value = ''
  await load()
}

/* -------- init -------- */
onMounted(load)
</script>
