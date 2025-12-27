<template>
  <section class="p-6 space-y-6 max-w-2xl mx-auto">
    <h1 class="text-2xl font-semibold">Settings</h1>

    <!-- Categories -->
    <div class="rounded-xl border p-4 space-y-3">
      <h2 class="text-lg font-semibold">Asset Categories</h2>

      <form @submit.prevent="addCategory" class="flex gap-2">
        <input
          v-model="newCategory"
          placeholder="New category name"
          class="border rounded px-3 py-2 flex-1"
          required
        />
        <button class="border rounded px-3 py-2">Add</button>
      </form>

      <ul class="list-disc pl-6">
        <li
          v-for="(cat, i) in settings.categories"
          :key="i"
          class="flex justify-between items-center"
        >
          <span>{{ cat }}</span>
          <button class="text-red-600 text-sm" @click="removeCategory(i)">Remove</button>
        </li>
      </ul>
    </div>

    <div class="flex justify-end">
      <button class="border rounded px-4 py-2" @click="saveSettings">💾 Save</button>
    </div>
  </section>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'

// We’ll store locally for now, but this can later sync to API
const settings = reactive({
  categories: [],
  statuses: []
})

const newCategory = ref('')
const newStatus = ref('')

// Load from localStorage
onMounted(() => {
  const saved = localStorage.getItem('settings')
  if (saved) Object.assign(settings, JSON.parse(saved))
})

const addCategory = () => {
  if (!settings.categories.includes(newCategory.value.trim())) {
    settings.categories.push(newCategory.value.trim())
    newCategory.value = ''
  }
}

const removeCategory = (index) => {
  settings.categories.splice(index, 1)
}


const saveSettings = () => {
  localStorage.setItem('settings', JSON.stringify(settings))
  alert('Settings saved ✅')
}
</script>
