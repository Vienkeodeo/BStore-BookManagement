<template>
  <div class="bg-light min-vh-100 d-flex flex-column">
    
    <nav v-if="isLoggedIn && isAdminPage" class="navbar navbar-expand-xl bg-white shadow-sm mb-4 py-3 sticky-top">
      <div class="container-fluid px-4">
        <router-link class="navbar-brand fw-bold text-primary fs-4" to="/dashboard">📚 B.Store Admin</router-link>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#adminNavbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="adminNavbarNav">
          <ul class="navbar-nav me-auto ms-4 fw-medium">
            <li class="nav-item"><router-link class="nav-link px-3" to="/dashboard" exact-active-class="text-primary fw-bold">Tổng Quan</router-link></li>
            <li class="nav-item"><router-link class="nav-link px-3" to="/books" exact-active-class="text-primary fw-bold">Kho Sách</router-link></li>
            <li class="nav-item"><router-link class="nav-link px-3" to="/categories" exact-active-class="text-primary fw-bold">Thể Loại</router-link></li>
            <li class="nav-item"><router-link class="nav-link px-3" to="/authors" exact-active-class="text-primary fw-bold">Tác Giả</router-link></li>
            <li class="nav-item"><router-link class="nav-link px-3" to="/publishers" exact-active-class="text-primary fw-bold">Nhà Xuất Bản</router-link></li>
            <li class="nav-item"><router-link class="nav-link px-3" to="/manage-orders" exact-active-class="text-primary fw-bold">Quản Lý Đơn</router-link></li>
          </ul>
          <div class="d-flex align-items-center mt-3 mt-xl-0">
            <router-link to="/create-order" class="btn btn-success rounded-pill px-4 fw-bold shadow-sm me-3">🛒 Lên Đơn POS</router-link>
            <router-link to="/home" class="btn btn-outline-secondary rounded-pill px-3 me-3 fw-medium">👁️ Xem Shop</router-link>
            <button @click="logout" class="btn btn-light border px-4 rounded-pill fw-semibold text-danger">Đăng Xuất</button>
          </div>
        </div>
      </div>
    </nav>

    <div class="flex-grow-1">
      <router-view @login-success="checkLoginStatus"></router-view>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()
const isLoggedIn = ref(false)

// Danh sách các đường dẫn của Khách hàng
const clientPaths = ['/', '/home', '/cart', '/checkout', '/admin', '/my-orders']

const isAdminPage = computed(() => {
  if (clientPaths.includes(route.path)) return false
  if (route.path.startsWith('/book/')) return false
  return true
})

const checkLoginStatus = () => {
  isLoggedIn.value = !!localStorage.getItem('token')
}

const logout = () => {
  if (confirm('Đăng xuất khỏi hệ thống quản trị?')) {
    localStorage.removeItem('token')
    checkLoginStatus()
    router.push('/admin') // Đăng xuất xong về trang login admin
  }
}

onMounted(() => checkLoginStatus())
</script>

<style>
body { margin: 0; padding: 0; background-color: #f8f9fa; font-family: 'Segoe UI', Roboto, Arial, sans-serif; }
.nav-link { transition: color 0.2s ease-in-out; }
.nav-link:hover { color: #0d6efd !important; }
.flex-grow-1 { display: flex; flex-direction: column; }
</style>