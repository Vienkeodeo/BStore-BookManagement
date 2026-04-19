<template>
  <div class="container mt-4">
    <div class="card shadow-sm">
      <div class="card-body">
        <h3>Cập Nhật Thể Loại</h3>
        <hr>
        <form @submit.prevent="updateCategory">
          <div class="mb-3">
            <label class="form-label">Tên thể loại <span class="text-danger">*</span></label>
            <input v-model="category.name" type="text" class="form-control" required />
          </div>
          <div class="mb-3">
            <label class="form-label">Mô tả</label>
            <textarea v-model="category.description" class="form-control" rows="3"></textarea>
          </div>
          
          <button type="submit" class="btn btn-warning me-2">Cập Nhật</button>
          <router-link to="/categories" class="btn btn-secondary">Quay lại</router-link>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const categoryId = route.params.id

const category = ref({ categoryId: 0, name: '', description: '' })

const fetchCategoryDetail = async () => {
  const token = localStorage.getItem('token')
  const response = await fetch(`https://localhost:7260/api/categories/${categoryId}`, {
    headers: { 'Authorization': `Bearer ${token}` }
  })
  if (response.ok) {
    category.value = await response.json()
  } else {
    alert('Không tìm thấy dữ liệu!')
    router.push('/categories')
  }
}

const updateCategory = async () => {
  const token = localStorage.getItem('token')
  const response = await fetch(`https://localhost:7260/api/categories/${categoryId}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(category.value)
  })

  if (response.ok || response.status === 204) {
    alert('Cập nhật thành công!')
    router.push('/categories')
  } else {
    alert('Cập nhật thất bại!')
  }
}

onMounted(() => fetchCategoryDetail())
</script>