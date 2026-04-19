<template>
  <div class="container mt-4" style="max-width: 700px;">
    <div class="card shadow-sm border-0 rounded-4">
      <div class="card-body p-5">
        <h3 class="fw-bold mb-4">Cập Nhật Tác Giả</h3>
        
        <form @submit.prevent="updateAuthor">
          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Họ và Tên <span class="text-danger">*</span></label>
            <input v-model="author.fullName" type="text" class="form-control px-4 py-2 bg-light border-0" required />
          </div>
          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Tiểu sử</label>
            <textarea v-model="author.biography" class="form-control px-4 py-3 bg-light border-0" rows="4"></textarea>
          </div>
          
          <div class="d-flex justify-content-end mt-5">
            <router-link to="/authors" class="btn btn-light px-4 py-2 fw-semibold rounded-pill me-3 border">Hủy bỏ</router-link>
            <button type="submit" class="btn btn-warning px-5 py-2 fw-semibold rounded-pill text-dark">Cập Nhật</button>
          </div>
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
const authorId = route.params.id
const author = ref({ authorId: 0, fullName: '', biography: '' })

const fetchAuthorDetail = async () => {
  const token = localStorage.getItem('token')
  const response = await fetch(`https://localhost:7260/api/authors/${authorId}`, { headers: { 'Authorization': `Bearer ${token}` } })
  if (response.ok) author.value = await response.json()
  else router.push('/authors')
}

const updateAuthor = async () => {
  const token = localStorage.getItem('token')
  const response = await fetch(`https://localhost:7260/api/authors/${authorId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
    body: JSON.stringify(author.value)
  })
  if (response.ok || response.status === 204) router.push('/authors')
}

onMounted(() => fetchAuthorDetail())
</script>