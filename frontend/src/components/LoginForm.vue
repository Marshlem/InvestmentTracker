<template>
<form class="grid gap-3 max-w-sm mx-auto" @submit.prevent="onLogin">
<input class="border rounded px-3 py-2" v-model="username" placeholder="Username" />
<input class="border rounded px-3 py-2" v-model="password" type="password" placeholder="Password" />
<button class="border rounded px-3 py-2">Login</button>
<p class="text-sm text-red-600" v-if="error">{{ error }}</p>
</form>
</template>


<script setup>
import { ref } from 'vue'
import { login } from '@/services/authService'
import { useRouter } from 'vue-router'


const router = useRouter()
const username = ref('')
const password = ref('')
const error = ref('')


const onLogin = async () => {
error.value = ''
try {
const { data } = await login(username.value, password.value)
localStorage.setItem('token', data.token)
router.push({ name: 'dashboard' })
} catch (e) {
error.value = e?.response?.data || e.message
}
}
</script>