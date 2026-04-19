<template>
  <div class="container mt-4" style="max-width: 700px;">
    <div class="card shadow-sm border-0 rounded-4">
      <div class="card-body p-5">
        <h3 class="fw-bold mb-4">Cập Nhật NXB</h3>
        
        <form @submit.prevent="updatePublisher">
          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Tên Nhà Xuất Bản <span class="text-danger">*</span></label>
            <input v-model="publisher.name" type="text" class="form-control px-4 py-2 bg-light border-0" required />
          </div>
          <div class="mb-4">
            <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Địa chỉ / Thông tin liên hệ</label>
            <textarea v-model="publisher.address" class="form-control px-4 py-3 bg-light border-0" rows="4"></textarea>
          </div>
          
          <div class="d-flex justify-content-end mt-5">
            <router-link to="/publishers" class="btn btn-light px-4 py-2 fw-semibold rounded-pill me-3 border">Hủy bỏ</router-link>
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
const pubId = route.params.id
const publisher = ref({ publisherId: 0, name: '', address: '' })

const fetchPublisherDetail = async () => {
  const token = localStorage.getItem('token')
  const response = await fetch(`https://localhost:7260/api/publishers/${pubId}`, { headers: { 'Authorization': `Bearer ${token}` } })
  if (response.ok) publisher.value = await response.json()
  else router.push('/publishers')
}

const updatePublisher = async () => {
  const token = localStorage.getItem('token')
  const response = await fetch(`https://localhost:7260/api/publishers/${pubId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
    body: JSON.stringify(publisher.value)
  })
  if (response.ok || response.status === 204) router.push('/publishers')
}

onMounted(() => fetchPublisherDetail())
</script>