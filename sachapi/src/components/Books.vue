<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <div>
        <h2 class="fw-bold mb-1">Quản Lý Kho Sách</h2>
        <p class="text-muted mb-0">Quản lý thông tin, danh mục và số lượng tồn kho</p>
      </div>
      <router-link to="/add-book" class="btn btn-primary px-4 py-2 rounded-pill fw-semibold">
        + Thêm Sách Mới
      </router-link>
    </div>

    <div class="card shadow-sm border-0 rounded-4">
      <div class="card-body p-4">
        
        <div class="row mb-4">
          <div class="col-md-5">
            <input 
              v-model="searchQuery" 
              type="text" 
              class="form-control rounded-pill px-4" 
              placeholder="🔍 Nhập tên sách để tìm kiếm..." 
            />
          </div>
        </div>

        <div class="table-responsive">
          <table class="table table-hover align-middle border-bottom">
            <thead class="text-muted" style="font-size: 0.85rem; text-transform: uppercase;">
              <tr>
                <th class="border-0">ID</th>
                <th class="border-0">Sách</th>
                <th class="border-0">Giá Bán</th>
                <th class="border-0">Tồn Kho</th>
                <th class="border-0">Thể Loại</th>
                <th class="border-0 text-center">Hành Động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="filteredBooks.length === 0">
                <td colspan="6" class="text-center py-4 text-muted">
                  Không tìm thấy cuốn sách nào!
                </td>
              </tr>

              <tr v-for="book in filteredBooks" :key="book.bookId">
                <td class="text-muted fw-semibold">#{{ book.bookId }}</td>
                
                <td>
                  <div class="d-flex align-items-center">
                    <div class="bg-light rounded d-flex justify-content-center align-items-center me-3" style="width: 45px; height: 60px; object-fit: cover; border: 1px solid #eee;">
                      <img v-if="book.coverImageUrl" :src="book.coverImageUrl" alt="cover" style="width: 100%; height: 100%; border-radius: 4px;">
                      <span v-else class="text-muted" style="font-size: 0.7rem;">No img</span>
                    </div>
                    <div>
                      <div class="fw-bold text-dark">{{ book.title }}</div>
                      <div class="text-muted" style="font-size: 0.8rem;">Mã: SP-{{ book.bookId }}</div>
                    </div>
                  </div>
                </td>

                <td class="text-danger fw-bold">{{ book.price.toLocaleString() }} đ</td>
                
                <td>
                  <span class="badge rounded-pill" :class="book.stockQuantity > 0 ? 'bg-success bg-opacity-25 text-success' : 'bg-danger bg-opacity-25 text-danger'">
                    {{ book.stockQuantity > 0 ? book.stockQuantity : 'Hết hàng' }}
                  </span>
                </td>
                
                <td>
                  <span class="badge bg-light text-dark border">{{ book.category?.name || 'N/A' }}</span>
                </td>
                
                <td class="text-center">
                  <router-link :to="`/edit-book/${book.bookId}`" style="background-color: #ffc107; color: #000; padding: 6px 12px; border-radius: 6px; text-decoration: none; font-size: 0.85rem; margin-right: 8px; font-weight: 500;">
                    Sửa
                  </router-link>
                  <button @click="deleteBook(book.bookId)" style="background-color: #dc3545; color: #fff; padding: 6px 12px; border-radius: 6px; border: none; font-size: 0.85rem; font-weight: 500; cursor: pointer;">
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

const books = ref([])
const searchQuery = ref('') // Biến lưu từ khóa tìm kiếm

// Đừng quên kiểm tra lại đúng cổng C# đang chạy nhé!
const API_URL = 'https://localhost:7260/api/books'

// Hàm lấy dữ liệu
const fetchBooks = async () => {
  const token = localStorage.getItem('token')
  try {
    const response = await fetch(API_URL, {
      headers: { 'Authorization': `Bearer ${token}` }
    })
    if (response.ok) {
      books.value = await response.json()
    }
  } catch (error) {
    console.error('Lỗi lấy dữ liệu:', error)
  }
}

// Xử lý logic Tìm kiếm (Tự động lọc mảng books dựa trên searchQuery)
const filteredBooks = computed(() => {
  if (!searchQuery.value) return books.value
  return books.value.filter(book => 
    book.title.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

// Hàm xóa
const deleteBook = async (id) => {
  if (confirm('Bạn có chắc chắn muốn xóa cuốn sách này?')) {
    const token = localStorage.getItem('token')
    try {
      const response = await fetch(`${API_URL}/${id}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
      })
      if (response.ok) {
        fetchBooks() // Load lại bảng ngay lập tức
      } else {
        alert('Xóa thất bại!')
      }
    } catch (error) {
      console.error('Lỗi xóa:', error)
    }
  }
}

onMounted(() => fetchBooks())
</script>

<style scoped>
/* Reset lại style cơ bản cho input để không vỡ layout */
input:focus {
  outline: none;
  box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.25);
  border-color: #86b7fe;
}
</style>