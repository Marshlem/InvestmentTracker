<template>
  <section class="p-6 space-y-6 max-w-2xl mx-auto">
    <h1 class="text-2xl font-semibold">Settings</h1>

    <div class="flex justify-end">
      <button class="border rounded px-4 py-2" @click="saveSettings">💾 Save</button>
    </div>
  </section>
</template>

<script setup>
import { onMounted } from 'vue'


// Load from localStorage
onMounted(() => {
  const saved = localStorage.getItem('settings')
  if (saved) Object.assign(settings, JSON.parse(saved))
})

const saveSettings = () => {
  localStorage.setItem('settings', JSON.stringify(settings))
  alert('Settings saved ✅')
}
</script>
