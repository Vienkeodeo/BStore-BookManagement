<template>
  <div class="container mt-4" style="max-width: 700px;">
    <div class="card shadow-sm border-0 rounded-4">
      <div class="card-body p-5">
        <h3 class="fw-bold mb-4">Thêm Tác Giả Mới</h3>
        
        <form @submit.prevent="saveAuthor">
          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Họ và Tên <span class="text-danger">*</span></label>
            <input v-model="author.fullName" type="text" class="form-control px-4 py-2 bg-light border-0" required placeholder="Nhập tên tác giả..." />
          </div>
          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Tiểu sử</label>
            <textarea v-model="author.biography" class="form-control px-4 py-3 bg-light border-0" rows="4" placeholder="Vài nét về tác giả..."></textarea>
          </div>
          
          <div class="d-flex justify-content-end mt-5">
            <router-link to="/authors" class="btn btn-light px-4 py-2 fw-semibold rounded-pill me-3">Hủy bỏ</router-link>
            <button type="submit" class="btn btn-primary px-5 py-2 fw-semibold rounded-pill">Lưu Dữ Liệu</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const author = ref({ fullName: '', biography: '' })

const saveAuthor = async () => {
  const token = localStorage.getItem('token')
  const response = await fetch('https://localhost:7260/api/authors', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
    body: JSON.stringify(author.value)
  })
  if (response.ok) router.push('/authors')
}
</script>