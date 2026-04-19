<template>
  <div class="container d-flex align-items-center justify-content-center" style="min-height: 80vh;">
    <div class="card shadow-sm border-0 rounded-4" style="width: 100%; max-width: 400px;">
      <div class="card-body p-5">
        <div class="text-center mb-4">
          <h2 class="fw-bold text-primary mb-2">B.Store</h2>
          <p class="text-muted">Đăng nhập hệ thống quản trị</p>
        </div>
        
        <form @submit.prevent="handleLogin">
          <div class="mb-3">
            <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Tài khoản</label>
            <input v-model="username" type="text" class="form-control px-3 py-2 bg-light border-0" required placeholder="Nhập username" />
          </div>
          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Mật khẩu</label>
            <input v-model="password" type="password" class="form-control px-3 py-2 bg-light border-0" required placeholder="••••••••" />
          </div>
          
          <button type="submit" class="btn btn-primary w-100 py-2 rounded-pill fw-bold fs-6">Đăng Nhập</button>
        </form>
        
        <div v-if="errorMessage" class="alert alert-danger mt-4 border-0 rounded-3 text-center" style="font-size: 0.9rem;">
          {{ errorMessage }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
// Giữ nguyên đoạn <script setup> của Login.vue cũ ở đây (có hàm handleLogin)
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const emit = defineEmits(['login-success'])
const username = ref('')
const password = ref('')
const errorMessage = ref('')
const API_URL = 'https://localhost:7260/api/auth/login' 

const handleLogin = async () => {
  try {
    const response = await fetch(API_URL, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ Username: username.value, Password: password.value })
    })

    if (response.ok) {
      const data = await response.json()
      localStorage.setItem('token', data.token)
      emit('login-success')
      router.push('/books')
    } else {
      errorMessage.value = 'Sai tài khoản hoặc mật khẩu!'
    }
  } catch (error) {
    errorMessage.value = 'Lỗi kết nối đến Server C#!'
  }
}
</script>