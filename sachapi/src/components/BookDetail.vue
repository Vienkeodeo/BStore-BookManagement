<template>
  <div class="bg-light min-vh-100 pb-5">
    <nav class="navbar navbar-expand-lg bg-white shadow-sm py-3 mb-4 sticky-top">
      <div class="container">
        <router-link class="navbar-brand fw-bold text-primary fs-3" to="/home">📚 B.Store</router-link>
        <div class="ms-auto"><router-link to="/cart" class="btn btn-outline-primary rounded-pill px-4 fw-bold">🛒 Giỏ Hàng</router-link></div>
      </div>
    </nav>

    <div v-if="book" class="container mt-2">
      <div class="card border-0 shadow-sm rounded-4 overflow-hidden">
        <div class="row g-0">
          <div class="col-md-5 bg-white d-flex align-items-center justify-content-center p-5 border-end">
            <img :src="book.coverImageUrl || 'https://via.placeholder.com/300x450'" class="img-fluid shadow-lg rounded-3 book-cover">
          </div>
          <div class="col-md-7 p-5">
            <nav aria-label="breadcrumb" class="mb-3">
              <ol class="breadcrumb">
                <li class="breadcrumb-item"><router-link to="/home" class="text-decoration-none">Trang chủ</router-link></li>
                <li class="breadcrumb-item active text-primary">{{ book.category?.name || '---' }}</li>
              </ol>
            </nav>
            <h1 class="fw-bold mb-2">{{ book.title }}</h1>
            <div class="mb-4 text-muted">Tác giả: <span class="fw-bold text-primary fs-5">{{ book.author?.fullName || 'Đang cập nhật' }}</span></div>
            <div class="fs-2 fw-bold text-danger mb-4">{{ book.price?.toLocaleString() }} đ</div>
            
            <div class="bg-light p-4 rounded-4 mb-4 row g-3">
              <div class="col-sm-6"><span class="text-muted small d-block">Thể loại</span><strong>{{ book.category?.name || '---' }}</strong></div>
              <div class="col-sm-6"><span class="text-muted small d-block">Nhà xuất bản</span><strong>{{ book.publisher?.name || '---' }}</strong></div>
              <div class="col-sm-12"><span class="text-muted small d-block">Tình trạng</span>
                <strong :class="book.stockQuantity > 0 ? 'text-success' : 'text-danger'">{{ book.stockQuantity > 0 ? `Còn hàng (${book.stockQuantity})` : 'Hết hàng' }}</strong>
              </div>
            </div>

            <div class="mb-5">
               <h6 class="fw-bold border-bottom pb-2 mb-3">Mô tả nội dung</h6>
               <p class="text-muted" style="white-space: pre-wrap; line-height: 1.8;">{{ book.description || 'Chưa có mô tả cho cuốn sách này.' }}</p>
            </div>

            <button @click="addToCart(book)" class="btn btn-primary btn-lg w-100 rounded-pill py-3 fw-bold shadow-sm" :disabled="book.stockQuantity <= 0">
              {{ book.stockQuantity > 0 ? 'Thêm Vào Giỏ Hàng' : 'Tạm Hết Hàng' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const book = ref(null)

const fetchBookDetail = async () => {
  const res = await fetch(`https://localhost:7260/api/books/${route.params.id}`)
  if (res.ok) book.value = await res.json()
}

const addToCart = (item) => {
  let cart = JSON.parse(localStorage.getItem('clientCart')) || []
  const existing = cart.find(i => i.bookId === item.bookId)
  if (existing) {
    if (existing.quantity < item.stockQuantity) existing.quantity++
    else return alert('Đã đạt giới hạn kho!')
  } else {
    cart.push({ ...item, quantity: 1, maxStock: item.stockQuantity })
  }
  localStorage.setItem('clientCart', JSON.stringify(cart))
  alert(`Đã thêm "${item.title}" vào giỏ!`)
}

onMounted(() => { fetchBookDetail(); window.scrollTo(0,0); })
</script>
<style scoped>.book-cover { transition: transform 0.3s; max-width: 100%; } .book-cover:hover { transform: scale(1.02); }</style>