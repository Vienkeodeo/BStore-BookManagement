<template>
  <div class="container mt-4" style="max-width: 800px;">
    <div class="card shadow-sm border-0 rounded-4">
      <div class="card-body p-5">
        <h3 class="fw-bold mb-4">Cập Nhật Thông Tin Sách</h3>
        
        <form @submit.prevent="updateBook">
          <div class="row">
            <div class="col-md-6 mb-4">
              <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Tên sách <span class="text-danger">*</span></label>
              <input v-model="book.title" type="text" class="form-control px-4 py-2 bg-light border-0" required />
            </div>
            <div class="col-md-6 mb-4">
              <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Link Ảnh Bìa (URL)</label>
              <input v-model="book.coverImageUrl" type="text" class="form-control px-4 py-2 bg-light border-0" />
            </div>

            <div class="col-md-6 mb-4">
              <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Giá bán (VNĐ) <span class="text-danger">*</span></label>
              <input v-model="book.price" type="number" class="form-control px-4 py-2 bg-light border-0" required />
            </div>
            <div class="col-md-6 mb-4">
              <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Số lượng tồn kho <span class="text-danger">*</span></label>
              <input v-model="book.stockQuantity" type="number" class="form-control px-4 py-2 bg-light border-0" required />
            </div>

            <div class="col-md-4 mb-4">
              <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Thể Loại <span class="text-danger">*</span></label>
              <select v-model="book.categoryId" class="form-select px-4 py-2 bg-light border-0" required>
                <option v-for="cat in categories" :key="cat.categoryId" :value="cat.categoryId">
                  {{ cat.name }}
                </option>
              </select>
            </div>
            
            <div class="col-md-4 mb-4">
              <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Tác Giả <span class="text-danger">*</span></label>
              <select v-model="book.authorId" class="form-select px-4 py-2 bg-light border-0" required>
                <option v-for="author in authors" :key="author.authorId" :value="author.authorId">
                  {{ author.fullName }}
                </option>
              </select>
            </div>

            <div class="col-md-4 mb-4">
              <label class="form-label fw-semibold text-secondary" style="font-size: 0.9rem;">Nhà Xuất Bản <span class="text-danger">*</span></label>
              <select v-model="book.publisherId" class="form-select px-4 py-2 bg-light border-0" required>
                <option v-for="pub in publishers" :key="pub.publisherId" :value="pub.publisherId">
                  {{ pub.name }}
                </option>
              </select>
            </div>
          </div>
          
          <div class="d-flex justify-content-end mt-4">
            <router-link to="/books" class="btn btn-light px-4 py-2 fw-semibold rounded-pill me-3 border">Hủy bỏ</router-link>
            <button type="submit" class="btn btn-warning px-5 py-2 fw-semibold rounded-pill text-dark">Cập Nhật Sách</button>
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
const bookId = route.params.id

const book = ref({ bookId: 0, title: '', price: 0, stockQuantity: 0, categoryId: '', authorId: '', publisherId: '', coverImageUrl: '' })

const categories = ref([])
const authors = ref([])
const publishers = ref([])

const loadAllData = async () => {
  const token = localStorage.getItem('token')
  const headers = { 'Authorization': `Bearer ${token}` }

  try {
    // Gọi đồng thời 4 API: Lấy chi tiết sách + 3 danh sách Dropdown
    const [resBook, resCat, resAuth, resPub] = await Promise.all([
      fetch(`https://localhost:7260/api/books/${bookId}`, { headers }),
      fetch('https://localhost:7260/api/categories', { headers }),
      fetch('https://localhost:7260/api/authors', { headers }),
      fetch('https://localhost:7260/api/publishers', { headers })
    ])

    if (resCat.ok) categories.value = await resCat.json()
    if (resAuth.ok) authors.value = await resAuth.json()
    if (resPub.ok) publishers.value = await resPub.json()
    
    if (resBook.ok) {
      book.value = await resBook.json()
    } else {
      alert('Không tìm thấy dữ liệu sách!')
      router.push('/books')
    }
  } catch (error) {
    console.error('Lỗi khi tải dữ liệu cập nhật:', error)
  }
}

const updateBook = async () => {
  const token = localStorage.getItem('token')
  try {
    const response = await fetch(`https://localhost:7260/api/books/${bookId}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
      body: JSON.stringify(book.value)
    })

    if (response.ok || response.status === 204) {
      router.push('/books')
    } else {
      alert('Cập nhật thất bại!')
    }
  } catch (error) {
    console.error(error)
  }
}

onMounted(() => loadAllData())
</script>