<template>
  <div class="bg-light min-vh-100 pb-5">
    <nav class="navbar navbar-expand-lg bg-white shadow-sm py-3 mb-4">
      <div class="container">
        <router-link class="navbar-brand fw-bold text-primary fs-3" to="/home">📚 B.Store</router-link>
        <router-link to="/home" class="btn btn-outline-secondary rounded-pill px-4 fw-medium">⬅ Tiếp tục mua sắm</router-link>
      </div>
    </nav>

    <div class="container">
      <h2 class="fw-bold mb-4">Giỏ Hàng</h2>
      <div class="row">
        <div class="col-lg-8 mb-4">
          <div class="card border-0 shadow-sm rounded-4 p-4">
            <div v-if="cart.length === 0" class="text-center py-5 text-muted">
              <h4>Giỏ hàng trống!</h4>
              <router-link to="/home" class="btn btn-primary rounded-pill px-4 mt-3">Mua sắm ngay</router-link>
            </div>
            <div v-else>
              <div v-for="(item, index) in cart" :key="item.bookId" class="d-flex align-items-center mb-4 pb-4 border-bottom">
                <img :src="item.coverImageUrl || 'https://via.placeholder.com/100x140'" class="rounded-3 me-4" style="width: 80px; height: 120px; object-fit: cover;">
                <div class="flex-grow-1">
                  <h5 class="fw-bold mb-1">{{ item.title }}</h5>
                  <div class="text-danger fw-bold mb-3 fs-5">{{ item.price.toLocaleString() }} đ</div>
                  <div class="d-flex justify-content-between align-items-center">
                    <div class="input-group input-group-sm" style="width: 120px;">
                      <button @click="updateQty(index, -1)" class="btn btn-outline-secondary fw-bold">-</button>
                      <input type="text" class="form-control text-center bg-white" :value="item.quantity" readonly>
                      <button @click="updateQty(index, 1)" class="btn btn-outline-secondary fw-bold">+</button>
                    </div>
                    <button @click="removeItem(index)" class="btn btn-link text-danger p-0 text-decoration-none">Xóa</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="col-lg-4" v-if="cart.length > 0">
          <div class="card border-0 shadow-sm rounded-4 p-4 sticky-top" style="top: 20px;">
            <h5 class="fw-bold mb-4">Tóm Tắt Đơn</h5>
            <div class="d-flex justify-content-between mb-2"><span>Tạm tính</span><span class="fw-medium">{{ totalPrice.toLocaleString() }} đ</span></div>
            <hr class="my-3">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <span class="fw-bold fs-5">Tổng cộng:</span>
              <span class="fw-bold fs-3 text-danger">{{ totalPrice.toLocaleString() }} đ</span>
            </div>
            <router-link to="/checkout" class="btn btn-primary w-100 py-3 rounded-pill fw-bold fs-5 shadow-sm">Thanh Toán</router-link>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const cart = ref([])
const loadCart = () => { cart.value = JSON.parse(localStorage.getItem('clientCart')) || [] }
const saveCart = () => { localStorage.setItem('clientCart', JSON.stringify(cart.value)) }

const totalPrice = computed(() => cart.value.reduce((sum, i) => sum + (i.price * i.quantity), 0))

const updateQty = (index, change) => {
  const item = cart.value[index]
  const newQty = item.quantity + change
  if (newQty <= 0) removeItem(index)
  else if (newQty <= item.maxStock) { item.quantity = newQty; saveCart() }
  else alert('Đã đạt giới hạn kho!')
}

const removeItem = (index) => {
  if (confirm('Bỏ cuốn này khỏi giỏ?')) { cart.value.splice(index, 1); saveCart() }
}
onMounted(loadCart)
</script>

<!-- //heloooooooooooooooooooooooooooooooooooooooooooooo-->


<!-- Long code -->