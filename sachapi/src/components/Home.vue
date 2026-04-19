<template>
  <div class="bg-light min-vh-100">
    <div v-if="toast.show" class="toast-container position-fixed bottom-0 end-0 p-3" style="z-index: 2000;">
      <div class="toast show align-items-center text-white bg-success border-0 rounded-3 shadow" role="alert">
        <div class="d-flex">
          <div class="toast-body fw-bold">✅ {{ toast.message }}</div>
          <button @click="toast.show = false" type="button" class="btn-close btn-close-white me-2 m-auto"></button>
        </div>
      </div>
    </div>

    <nav class="navbar navbar-expand-lg bg-white shadow-sm py-3 sticky-top">
      <div class="container">
        <router-link class="navbar-brand fw-bold text-primary fs-3" to="/home">📚 B.Store</router-link>
        <button class="navbar-toggler border-0" type="button" data-bs-toggle="collapse" data-bs-target="#clientNavbar">
          <span class="navbar-toggler-icon"></span>
        </button>
        <div class="collapse navbar-collapse" id="clientNavbar">
          <div class="d-flex align-items-center ms-auto mt-3 mt-lg-0">
            <router-link to="/my-orders" class="nav-link px-3 fw-medium text-secondary me-2">🔍 Tra cứu đơn hàng</router-link>
            
            <router-link to="/cart" class="btn btn-outline-primary rounded-pill px-4 fw-bold position-relative me-3 transition-all cart-btn">
              🛒 Giỏ Hàng
              <span v-if="cartItemCount > 0" class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger border border-white">
                {{ cartItemCount }}
              </span>
            </router-link>
            
            <router-link to="/profile" class="btn btn-light rounded-circle p-2 shadow-sm" title="Tài khoản của tôi">👤</router-link>
          </div>
        </div>
      </div>
    </nav>

    <div class="container mt-4 mb-4">
      <div class="p-5 text-center text-white rounded-4 shadow-lg hero-bg">
        <h1 class="display-4 fw-bold mb-3">Thế Giới Sách Trong Tầm Tay</h1>
        <div class="row justify-content-center mt-4">
          <div class="col-md-7">
            <div class="input-group input-group-lg shadow-sm rounded-pill overflow-hidden bg-white">
              <span class="input-group-text border-0 bg-white ps-4">🔍</span>
              <input v-model="searchQuery" type="text" class="form-control border-0 px-2" placeholder="Nhập tên sách bạn muốn tìm..." />
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="container mb-4 overflow-auto category-scroll pb-2">
      <div class="d-flex gap-2">
        <button @click="selectedCategory = 0" :class="['btn rounded-pill px-4 fw-semibold text-nowrap transition-all', selectedCategory === 0 ? 'btn-primary shadow-sm' : 'btn-white border text-secondary']">Tất cả</button>
        <button v-for="cat in categories" :key="cat.categoryId" @click="selectedCategory = cat.categoryId" :class="['btn rounded-pill px-4 fw-semibold text-nowrap transition-all', selectedCategory === cat.categoryId ? 'btn-primary shadow-sm' : 'btn-white border text-secondary']">{{ cat.name }}</button>
      </div>
    </div>

    <div class="container mb-5">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h4 class="fw-bold mb-0">Sách Nổi Bật</h4>
        <select v-model="sortOrder" class="form-select w-auto rounded-pill border-0 shadow-sm fw-medium">
          <option value="new">🌟 Mới nhất</option>
          <option value="price_asc">📈 Giá thấp đến cao</option>
          <option value="price_desc">📉 Giá cao đến thấp</option>
        </select>
      </div>

      <div v-if="isLoading" class="text-center py-5">
        <div class="spinner-border text-primary mb-3" role="status" style="width: 3rem; height: 3rem;"></div>
        <h5 class="text-muted">Đang tải kệ sách...</h5>
      </div>

      <div v-else-if="filteredBooks.length === 0" class="text-center py-5 bg-white rounded-4 shadow-sm border border-light">
        <div style="font-size: 4rem; opacity: 0.5;" class="mb-3">📚❓</div>
        <h4 class="text-muted fw-bold">Không tìm thấy cuốn sách nào!</h4>
        <p class="text-secondary">Vui lòng thử tìm với từ khóa khác hoặc chọn thể loại khác nhé.</p>
        <button @click="resetFilters" class="btn btn-outline-primary rounded-pill px-4 mt-2">Xóa bộ lọc</button>
      </div>

      <div v-else class="row g-4">
        <div v-for="book in filteredBooks" :key="book.bookId" class="col-6 col-md-4 col-lg-3">
          <div class="card h-100 border-0 shadow-sm rounded-4 product-card overflow-hidden">
            <router-link :to="`/book/${book.bookId}`" class="text-decoration-none">
              <div class="bg-light d-flex align-items-center justify-content-center position-relative" style="height: 280px;">
                <img :src="book.coverImageUrl || 'https://via.placeholder.com/200x300'" class="book-img" style="max-height: 85%; transition: transform 0.3s ease;">
                <div v-if="book.stockQuantity <= 0" class="position-absolute bg-dark text-white px-3 py-1 rounded-pill fw-bold" style="background: rgba(0,0,0,0.7);">Hết hàng</div>
              </div>
            </router-link>
            
            <div class="card-body d-flex flex-column p-3">
              <h6 class="fw-bold text-dark mb-1 book-title">{{ book.title }}</h6>
              <p class="text-muted small mb-3">{{ book.author?.fullName || 'Nhiều tác giả' }}</p>
              
              <div class="mt-auto d-flex justify-content-between align-items-center">
                <span class="fw-bold text-danger fs-5">{{ book.price.toLocaleString() }}đ</span>
                <button @click="addToCart(book)" class="btn btn-primary rounded-circle d-flex align-items-center justify-content-center add-btn shadow-sm" :disabled="book.stockQuantity <= 0" title="Thêm vào giỏ">
                  <span class="fw-bold fs-5">+</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'

const books = ref([])
const categories = ref([])
const searchQuery = ref('')
const selectedCategory = ref(0)
const sortOrder = ref('new')
const cartItemCount = ref(0)
const isLoading = ref(true) // Thêm biến trạng thái loading
const toast = ref({ show: false, message: '' })

const fetchInitialData = async () => {
  isLoading.value = true // Bắt đầu loading
  try {
    const [resBooks, resCats] = await Promise.all([
      fetch('https://localhost:7260/api/books'),
      fetch('https://localhost:7260/api/categories')
    ])
    if (resBooks.ok) books.value = await resBooks.json()
    if (resCats.ok) categories.value = await resCats.json()
  } catch (error) {
    console.error("Lỗi tải dữ liệu:", error)
  } finally {
    isLoading.value = false // Tắt loading dù thành công hay thất bại
  }
}

const filteredBooks = computed(() => {
  let result = books.value.filter(b => {
    const matchSearch = b.title.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchCat = selectedCategory.value === 0 || b.categoryId === selectedCategory.value
    return matchSearch && matchCat
  })
  if (sortOrder.value === 'price_asc') result.sort((a, b) => a.price - b.price)
  if (sortOrder.value === 'price_desc') result.sort((a, b) => b.price - a.price)
  return result
})

const resetFilters = () => {
  searchQuery.value = ''
  selectedCategory.value = 0
}

const showToast = (msg) => {
  toast.value.message = msg; toast.value.show = true
  setTimeout(() => toast.value.show = false, 3000)
}

const addToCart = (book) => {
  let cart = JSON.parse(localStorage.getItem('clientCart')) || []
  const item = cart.find(i => i.bookId === book.bookId)
  if (item) {
    if (item.quantity < book.stockQuantity) item.quantity++
    else return alert('Đã đạt giới hạn số lượng trong kho!')
  } else {
    cart.push({ ...book, quantity: 1, maxStock: book.stockQuantity })
  }
  localStorage.setItem('clientCart', JSON.stringify(cart))
  
  // Phát ra một sự kiện (event) để tự thân tab này cũng nhận biết được storage đã thay đổi
  window.dispatchEvent(new Event('storage')) 
  showToast(`Đã thêm "${book.title}" vào giỏ`)
}

const updateCartCount = () => {
  const cart = JSON.parse(localStorage.getItem('clientCart')) || []
  cartItemCount.value = cart.reduce((total, i) => total + i.quantity, 0)
}

// Lắng nghe sự thay đổi của LocalStorage để nhảy số giỏ hàng (Áp dụng khi mở nhiều tab)
const handleStorageChange = () => {
  updateCartCount()
}

onMounted(() => { 
  fetchInitialData(); 
  updateCartCount();
  window.addEventListener('storage', handleStorageChange)
})

onUnmounted(() => {
  window.removeEventListener('storage', handleStorageChange)
})
</script>

<style scoped>
/* Gradient cho Banner */
.hero-bg {
  background: linear-gradient(135deg, #0d6efd 0%, #00d2ff 100%);
}

/* Card Sản phẩm */
.product-card { transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1); cursor: pointer; }
.product-card:hover { transform: translateY(-8px); box-shadow: 0 15px 30px rgba(0,0,0,0.1) !important; }
.product-card:hover .book-img { transform: scale(1.08) !important; }

/* Tiêu đề sách giới hạn 2 dòng */
.book-title {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  min-height: 2.8rem;
  line-height: 1.4rem;
}

/* Các nút bấm */
.transition-all { transition: all 0.2s ease; }
.cart-btn:hover { transform: translateY(-2px); }
.add-btn { width: 40px; height: 40px; transition: transform 0.2s, background-color 0.2s; }
.add-btn:active { transform: scale(0.9); }

/* Custom Scrollbar cho danh mục để giao diện sạch sẽ trên Mobile */
.category-scroll::-webkit-scrollbar { height: 4px; }
.category-scroll::-webkit-scrollbar-thumb { background: #dee2e6; border-radius: 10px; }
.category-scroll::-webkit-scrollbar-track { background: transparent; }
</style>