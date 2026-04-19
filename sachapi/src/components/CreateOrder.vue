<template>
  <div class="container-fluid mt-4 px-4">
    <div class="mb-4">
      <h2 class="fw-bold mb-1">Lên Đơn Bán Hàng</h2>
      <p class="text-muted mb-0">Tạo đơn hàng mới (POS)</p>
    </div>

    <div class="row">
      <div class="col-lg-7 mb-4">
        <div class="card shadow-sm border-0 rounded-4 h-100">
          <div class="card-body p-4">
            <h5 class="fw-bold mb-3">Chọn Sản Phẩm</h5>
            
            <input 
              v-model="searchQuery" 
              type="text" 
              class="form-control rounded-pill px-4 bg-light border-0 mb-4" 
              placeholder="🔍 Nhập tên sách để tìm nhanh..." 
            />

            <div class="row g-3" style="max-height: 600px; overflow-y: auto;">
              <div v-for="book in filteredBooks" :key="book.bookId" class="col-md-6 col-xl-4">
                <div 
                  class="card h-100 border rounded-3 position-relative" 
                  :class="book.stockQuantity > 0 ? 'cursor-pointer hover-shadow' : 'opacity-50'"
                  @click="book.stockQuantity > 0 ? addToCart(book) : null"
                  style="transition: all 0.2s; cursor: pointer;"
                >
                  <div class="card-body p-3 text-center">
                    <div class="bg-light rounded mb-2 mx-auto d-flex align-items-center justify-content-center" style="height: 120px; width: 80px; object-fit: cover;">
                      <img v-if="book.coverImageUrl" :src="book.coverImageUrl" style="height: 100%; width: 100%; border-radius: 4px;">
                      <span v-else class="text-muted small">No Img</span>
                    </div>
                    <h6 class="fw-bold text-dark mb-1 text-truncate" :title="book.title">{{ book.title }}</h6>
                    <div class="text-danger fw-bold mb-2">{{ book.price.toLocaleString() }} đ</div>
                    <span class="badge rounded-pill" :class="book.stockQuantity > 0 ? 'bg-success bg-opacity-10 text-success' : 'bg-danger bg-opacity-10 text-danger'">
                      Tồn: {{ book.stockQuantity }}
                    </span>
                  </div>
                </div>
              </div>
            </div>

          </div>
        </div>
      </div>

      <div class="col-lg-5 mb-4">
        <div class="card shadow-sm border-0 rounded-4 h-100">
          <div class="card-body p-4 d-flex flex-column">
            <h5 class="fw-bold mb-4">Chi Tiết Đơn Hàng</h5>

            <div class="mb-4">
              <input v-model="customerName" type="text" class="form-control px-3 py-2 bg-light border-0 rounded-3" placeholder="Tên khách hàng (Không bắt buộc)..." />
            </div>

            <div class="flex-grow-1" style="max-height: 400px; overflow-y: auto;">
              <div v-if="cart.length === 0" class="text-center text-muted mt-5">
                <p>Chưa có sản phẩm nào trong đơn.</p>
              </div>
              
              <div v-for="(item, index) in cart" :key="item.bookId" class="d-flex align-items-center justify-content-between mb-3 pb-3 border-bottom">
                <div class="flex-grow-1 pe-2">
                  <div class="fw-bold text-dark">{{ item.title }}</div>
                  <div class="text-danger fw-semibold" style="font-size: 0.9rem;">{{ item.price.toLocaleString() }} đ</div>
                </div>
                
                <div class="d-flex align-items-center">
                  <button @click="updateQty(index, -1)" class="btn btn-sm btn-light border rounded-circle fw-bold px-2">-</button>
                  <span class="mx-2 fw-bold">{{ item.quantity }}</span>
                  <button @click="updateQty(index, 1)" class="btn btn-sm btn-light border rounded-circle fw-bold px-2">+</button>
                </div>
              </div>
            </div>

            <div class="mt-auto pt-3 border-top">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <span class="fw-bold text-secondary fs-5">Tổng tiền:</span>
                <span class="fw-bold text-danger fs-3">{{ totalPrice.toLocaleString() }} đ</span>
              </div>
              <button 
                @click="submitOrder" 
                class="btn btn-primary w-100 py-3 rounded-pill fw-bold fs-5"
                :disabled="cart.length === 0"
              >
                Chốt Đơn Bán Hàng
              </button>
            </div>

          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const books = ref([])
const searchQuery = ref('')

// Biến cho Đơn hàng
const cart = ref([])
const customerName = ref('')

// API Lấy sách
const fetchBooks = async () => {
  const token = localStorage.getItem('token')
  const response = await fetch('https://localhost:7260/api/books', {
    headers: { 'Authorization': `Bearer ${token}` }
  })
  if (response.ok) books.value = await response.json()
}

// Lọc sách theo thanh tìm kiếm
const filteredBooks = computed(() => {
  if (!searchQuery.value) return books.value
  return books.value.filter(b => b.title.toLowerCase().includes(searchQuery.value.toLowerCase()))
})

// Tính tổng tiền giỏ hàng
const totalPrice = computed(() => {
  return cart.value.reduce((total, item) => total + (item.price * item.quantity), 0)
})

// Thêm sách vào giỏ
const addToCart = (book) => {
  const existingItem = cart.value.find(item => item.bookId === book.bookId)
  if (existingItem) {
    if (existingItem.quantity < book.stockQuantity) {
      existingItem.quantity++
    } else {
      alert('Đã đạt số lượng tồn kho tối đa của sách này!')
    }
  } else {
    cart.value.push({
      bookId: book.bookId,
      title: book.title,
      price: book.price,
      quantity: 1,
      maxStock: book.stockQuantity
    })
  }
}

// Tăng/giảm số lượng trong giỏ
const updateQty = (index, change) => {
  const item = cart.value[index]
  const newQty = item.quantity + change
  
  if (newQty <= 0) {
    cart.value.splice(index, 1) // Xóa khỏi giỏ nếu giảm về 0
  } else if (newQty <= item.maxStock) {
    item.quantity = newQty
  } else {
    alert('Không đủ tồn kho!')
  }
}

// Gửi API tạo đơn hàng (Chốt đơn)
const submitOrder = async () => {
  const token = localStorage.getItem('token')
  
  // Tạo dữ liệu gửi xuống API
  const orderData = {
    customerName: customerName.value || 'Khách lẻ',
    orderDetails: cart.value.map(item => ({
      bookId: item.bookId,
      quantity: item.quantity,
      price: item.price
    }))
  }

  try {
    // Lưu ý: Đường dẫn API này lát nữa mình phải code bên C#
    const response = await fetch('https://localhost:7260/api/orders', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(orderData)
    })

    if (response.ok) {
      alert('Tạo đơn hàng thành công!')
      cart.value = [] // Làm sạch giỏ hàng
      customerName.value = ''
      fetchBooks() // Tải lại sách để cập nhật lại số lượng tồn kho mới
    } else {
      alert('Có lỗi xảy ra khi tạo đơn hàng!')
    }
  } catch (error) {
    console.error('Lỗi:', error)
  }
}

onMounted(() => fetchBooks())
</script>

<style scoped>
.hover-shadow:hover {
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15) !important;
  transform: translateY(-2px);
}
</style>