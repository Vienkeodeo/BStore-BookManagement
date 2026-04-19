<template>
  <div class="container d-flex align-items-center justify-content-center min-vh-100">
    <div class="card shadow-lg border-0 rounded-4 overflow-hidden" style="max-width: 900px; width: 100%;">
      <div class="row g-0">
        <div class="col-md-5 d-none d-md-block bg-primary d-flex align-items-center justify-content-center text-white p-5">
          <div class="text-center">
            <h2 class="fw-bold mb-4">Chào mừng đến với B.Store</h2>
            <p>Đăng ký thành viên để nhận nhiều ưu đãi và theo dõi đơn hàng dễ dàng hơn.</p>
          </div>
        </div>
        <div class="col-md-7 p-5 bg-white">
          <div class="d-flex mb-4 border-bottom">
            <button @click="isLogin = true" :class="['btn flex-fill pb-2 fw-bold', isLogin ? 'border-primary border-bottom border-3 text-primary' : 'text-muted']">ĐĂNG NHẬP</button>
            <button @click="isLogin = false" :class="['btn flex-fill pb-2 fw-bold', !isLogin ? 'border-primary border-bottom border-3 text-primary' : 'text-muted']">ĐĂNG KÝ</button>
          </div>

          <form v-if="isLogin" @submit.prevent="handleLogin">
            <div class="mb-3">
              <label class="form-label small fw-bold">Tên tài khoản</label>
              <input v-model="loginData.username" type="text" class="form-control bg-light border-0 py-2" required>
            </div>
            <div class="mb-4">
              <label class="form-label small fw-bold">Mật khẩu</label>
              <input v-model="loginData.password" type="password" class="form-control bg-light border-0 py-2" required>
            </div>
            <button type="submit" class="btn btn-primary w-100 py-2 rounded-pill fw-bold" :disabled="isLoading">
              {{ isLoading ? 'ĐANG XỬ LÝ...' : 'ĐĂNG NHẬP' }}
            </button>
          </form>

          <form v-else @submit.prevent="handleRegister">
             <div class="mb-3">
              <label class="form-label small fw-bold">Họ và tên</label>
              <input v-model="regData.fullName" type="text" class="form-control bg-light border-0" required>
            </div>
            <div class="mb-3">
              <label class="form-label small fw-bold">Email</label>
              <input v-model="regData.email" type="email" class="form-control bg-light border-0" required>
            </div>
            <div class="mb-3">
              <label class="form-label small fw-bold">Tên tài khoản</label>
              <input v-model="regData.username" type="text" class="form-control bg-light border-0" required>
            </div>
            <div class="mb-3">
              <label class="form-label small fw-bold">Mật khẩu (Gồm chữ Hoa, Thường, Số, Ký tự)</label>
              <input v-model="regData.password" type="password" class="form-control bg-light border-0" required>
            </div>
            <button type="submit" class="btn btn-success w-100 py-2 rounded-pill fw-bold" :disabled="isLoading">
               {{ isLoading ? 'ĐANG XỬ LÝ...' : 'TẠO TÀI KHOẢN' }}
            </button>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const isLogin = ref(true)
const isLoading = ref(false)
const router = useRouter()
const loginData = ref({ username: '', password: '' })
const regData = ref({ username: '', password: '', fullName: '', email: '' })

const handleLogin = async () => {
  isLoading.value = true
  try {
    const res = await fetch('https://localhost:7260/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(loginData.value)
    })
    if (res.ok) {
      const data = await res.json()
      localStorage.setItem('customerToken', data.token)
      localStorage.setItem('customerInfo', JSON.stringify({ username: data.username }))
      router.push('/home') // Đăng nhập xong đẩy vào /home
    } else {
      const err = await res.json()
      alert(err.message || 'Sai tài khoản hoặc mật khẩu!')
    }
  } finally { isLoading.value = false }
}

const handleRegister = async () => {
  isLoading.value = true
  try {
    const res = await fetch('https://localhost:7260/api/auth/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(regData.value)
    })
    const data = await res.json()
    if (res.ok) {
      alert('Đăng ký thành công! Vui lòng đăng nhập.')
      isLogin.value = true
    } else {
      alert(data.message || 'Lỗi đăng ký!')
    }
  } finally { isLoading.value = false }
}
</script>