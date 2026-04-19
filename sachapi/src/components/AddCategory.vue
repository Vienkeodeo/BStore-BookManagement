<template>
  <div class="container mt-4" style="max-width: 700px;">
    <div class="card shadow-sm border-0 rounded-4">
      <div class="card-body p-5">
        <h3 class="fw-bold mb-4">Thêm Thể Loại Mới</h3>
        
        <form @submit.prevent="saveCategory">
          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Tên thể loại <span class="text-danger">*</span></label>
            <input v-model="category.name" type="text" class="form-control px-4 py-2 bg-light border-0" required placeholder="Nhập tên..." />
          </div>
          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Mô tả</label>
            <textarea v-model="category.description" class="form-control px-4 py-3 bg-light border-0" rows="4" placeholder="Viết mô tả chi tiết..."></textarea>
          </div>
          
          <div class="d-flex justify-content-end mt-5">
            <router-link to="/categories" class="btn btn-light px-4 py-2 fw-semibold rounded-pill me-3">Hủy bỏ</router-link>
            <button type="submit" class="btn btn-primary px-5 py-2 fw-semibold rounded-pill">Lưu Dữ Liệu</button>
          </div>
        </form>
        
      </div>
    </div>
  </div>
</template>

<script setup>
// Giữ nguyên logic script setup của form AddCategory cũ nhé
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const category = ref({ name: '', description: '' })

const saveCategory = async () => {
  const token = localStorage.getItem('token')
  const response = await fetch('https://localhost:7260/api/categories', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
    body: JSON.stringify(category.value)
  })
  if (response.ok) router.push('/categories')
}
</script>