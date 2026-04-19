<template>
  <div class="bg-light min-vh-100 pb-5">
    <nav class="navbar navbar-expand-lg bg-white shadow-sm py-3 mb-5 sticky-top">
      <div class="container">
        <router-link class="navbar-brand fw-bold text-primary fs-3" to="/home">📚 B.Store</router-link>
        <router-link to="/home" class="btn btn-outline-secondary rounded-pill px-4 fw-medium">Tiếp tục mua sắm</router-link>
      </div>
    </nav>

    <div class="container" style="max-width: 500px;">
      <div class="card border-0 shadow-sm rounded-4 p-5 text-center">
        <div class="mb-4">
          <div class="bg-primary text-white rounded-circle d-inline-flex align-items-center justify-content-center shadow" style="width: 90px; height: 90px; font-size: 2.5rem;">
            👤
          </div>
        </div>
        
        <h3 class="fw-bold mb-1">Xin chào, {{ userInfo.username }}!</h3>
        <p class="text-muted mb-4">Khách hàng thành viên</p>

        <div class="d-grid gap-3 mt-2">
          <router-link to="/my-orders" class="btn btn-primary rounded-pill py-3 fw-bold shadow-sm">
            📦 Quản lý đơn hàng của tôi
          </router-link>
          
          <button @click="handleLogout" class="btn btn-outline-danger rounded-pill py-3 fw-bold">
            🚪 Đăng xuất tài khoản
          </button>
        </div>

        <div class="mt-5">
          <router-link to="/admin" class="text-muted small text-decoration-none" title="Dành cho nhân viên">
            <i class="bi bi-shield-lock"></i> Khu vực Quản trị viên
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const userInfo = ref({ username: '' })

onMounted(() => {
  // Lấy thông tin khách hàng đang đăng nhập
  const storedInfo = localStorage.getItem('customerInfo')
  if (storedInfo) {
    userInfo.value = JSON.parse(storedInfo)
  } else {
    alert('Anh/Chị vui lòng đăng nhập trước nhé!')
    router.push('/')
  }
})

const handleLogout = () => {
  if (confirm('Anh/chị có chắc chắn muốn đăng xuất không?')) {
    // Xóa Token và thông tin khách hàng khỏi trình duyệt
    localStorage.removeItem('customerToken')
    localStorage.removeItem('customerInfo')
    localStorage.removeItem('clientCart') // Xóa luôn giỏ hàng cho bảo mật
    
    // Đẩy về trang Đăng nhập
    router.push('/')
  }
}
</script>

<style scoped>
.card { transition: transform 0.2s; }
.card:hover { transform: translateY(-5px); }
</style>