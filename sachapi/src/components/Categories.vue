<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h2 class="fw-bold mb-1">Danh Mục Thể Loại</h2>
        <p class="text-muted mb-0">Quản lý các nhóm phân loại sách</p>
      </div>
      <router-link to="/add-category" class="btn btn-primary px-4 py-2 rounded-pill fw-semibold">
        + Thêm Thể Loại
      </router-link>
    </div>

    <div class="card shadow-sm border-0 rounded-4">
      <div class="card-body p-4">
        
        <div class="row mb-4">
          <div class="col-md-5">
            <input 
              v-model="searchQuery" 
              type="text" 
              class="form-control rounded-pill px-4 bg-light border-0" 
              placeholder="🔍 Tìm theo tên thể loại..." 
            />
          </div>
        </div>

        <div class="table-responsive">
          <table class="table table-hover align-middle border-bottom">
            <thead class="text-muted" style="font-size: 0.85rem; text-transform: uppercase;">
              <tr>
                <th class="border-0">Mã TL</th>
                <th class="border-0">Tên Thể Loại</th>
                <th class="border-0">Mô Tả Chi Tiết</th>
                <th class="border-0 text-center">Hành Động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="filteredCategories.length === 0">
                <td colspan="4" class="text-center py-4 text-muted">Không có dữ liệu!</td>
              </tr>
              <tr v-for="cat in filteredCategories" :key="cat.categoryId">
                <td class="text-muted fw-semibold">#{{ cat.categoryId }}</td>
                <td class="fw-bold text-dark fs-6">{{ cat.name }}</td>
                <td class="text-muted" style="max-width: 300px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">
                  {{ cat.description || '—' }}
                </td>
                <td class="text-center">
                  <router-link :to="`/edit-category/${cat.categoryId}`" style="background-color: #f1f3f5; color: #495057; padding: 6px 16px; border-radius: 6px; text-decoration: none; font-size: 0.85rem; margin-right: 8px; font-weight: 600;">
                    Sửa
                  </router-link>
                  <button @click="deleteCategory(cat.categoryId)" style="background-color: #fee2e2; color: #ef4444; padding: 6px 16px; border-radius: 6px; border: none; font-size: 0.85rem; font-weight: 600; cursor: pointer;">
                    Xóa
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'

const categories = ref([])
const searchQuery = ref('')
const API_URL = 'https://localhost:7260/api/categories'

const fetchCategories = async () => {
  const token = localStorage.getItem('token')
  const response = await fetch(API_URL, { headers: { 'Authorization': `Bearer ${token}` } })
  if (response.ok) categories.value = await response.json()
}

const filteredCategories = computed(() => {
  if (!searchQuery.value) return categories.value
  return categories.value.filter(cat => cat.name.toLowerCase().includes(searchQuery.value.toLowerCase()))
})

const deleteCategory = async (id) => {
  if (confirm('Xác nhận xóa thể loại này?')) {
    const token = localStorage.getItem('token')
    const response = await fetch(`${API_URL}/${id}`, { method: 'DELETE', headers: { 'Authorization': `Bearer ${token}` } })
    if (response.ok) fetchCategories()
    else alert('Lỗi! Vui lòng kiểm tra sách liên quan.')
  }
}

onMounted(() => fetchCategories())
</script>