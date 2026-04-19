<template>
  <div class="bg-light min-vh-100 pb-5">
    <nav class="navbar navbar-expand-lg bg-white shadow-sm py-3 mb-4">
      <div class="container">
        <router-link class="navbar-brand fw-bold text-primary" to="/home">📚 B.Store</router-link>
      </div>
    </nav>
    <div class="container py-4" style="max-width: 1000px;">
      <h2 class="fw-bold text-center mb-5">Hoàn Tất Đơn Hàng</h2>
      <div class="row g-4">
        <div class="col-lg-7">
          <div class="card border-0 shadow-sm rounded-4 p-4">
            <h5 class="fw-bold mb-4">Thông tin giao hàng</h5>
            <form @submit.prevent="handleCheckout">
              <div class="mb-3">
                <label class="form-label fw-semibold">Họ và tên</label>
                <input v-model="order.customerName" type="text" class="form-control bg-light border-0 py-2" required>
              </div>
              <div class="mb-3">
                <label class="form-label fw-semibold">Số điện thoại</label>
                <input v-model="order.phoneNumber" type="tel" class="form-control bg-light border-0 py-2" required>
              </div>
              <div class="mb-4">
                <label class="form-label fw-semibold">Địa chỉ nhận hàng</label>
                <textarea v-model="order.shippingAddress" class="form-control bg-light border-0 py-2" rows="3" required></textarea>
              </div>
              
              <h5 class="fw-bold mb-3 border-top pt-4">Phương thức thanh toán</h5>
              <div class="card border-primary mb-3">
                <div class="card-body">
                  <div class="form-check mb-3">
                    <input class="form-check-input" type="radio" v-model="paymentMethod" value="COD" id="cod">
                    <label class="form-check-label fw-bold" for="cod">
                      💵 Thanh toán khi nhận hàng (COD)
                    </label>
                  </div>
                  <div class="form-check">
                    <input class="form-check-input" type="radio" v-model="paymentMethod" value="MOMO" id="momo">
                    <label class="form-check-label fw-bold text-danger" for="momo">
                      <img src="https://upload.wikimedia.org/wikipedia/vi/f/fe/MoMo_Logo.png" alt="Momo" style="width: 25px; margin-right: 5px;">
                      Thanh toán qua Ví MoMo
                    </label>
                  </div>
                </div>
              </div>

              <button type="submit" class="btn btn-success w-100 py-3 rounded-pill fw-bold mt-2 shadow" :disabled="isSubmitting">
                {{ isSubmitting ? 'ĐANG XỬ LÝ...' : (paymentMethod === 'MOMO' ? 'THANH TOÁN BẰNG MOMO' : 'XÁC NHẬN ĐẶT HÀNG') }}
              </button>
            </form>
          </div>
        </div>

        <div class="col-lg-5">
          <div class="card border-0 shadow-sm rounded-4 p-4 sticky-top" style="top: 20px;">
            <h5 class="fw-bold mb-4">Đơn hàng của bạn</h5>
            <div v-for="item in cart" :key="item.bookId" class="d-flex justify-content-between mb-3 border-bottom pb-2">
              <div>
                <div class="fw-bold small">{{ item.title }}</div>
                <div class="text-muted small">SL: {{ item.quantity }}</div>
              </div>
              <div class="fw-bold text-danger">{{ (item.quantity * item.price).toLocaleString() }}đ</div>
            </div>
            <div class="d-flex justify-content-between mt-4">
              <h5 class="fw-bold">TỔNG CỘNG:</h5>
              <h4 class="fw-bold text-danger">{{ totalAmount.toLocaleString() }}đ</h4>
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
const cart = ref([])
const isSubmitting = ref(false)
const paymentMethod = ref('COD') // Mặc định là COD
const order = ref({ customerName: '', phoneNumber: '', shippingAddress: '' })

const totalAmount = computed(() => cart.value.reduce((sum, i) => sum + (i.price * i.quantity), 0))

const handleCheckout = async () => {
  if (cart.value.length === 0) return
  isSubmitting.value = true
  
  const customerInfo = JSON.parse(localStorage.getItem('customerInfo'))
  const orderData = { 
    ...order.value, 
    username: customerInfo ? customerInfo.username : null,
    orderDetails: cart.value.map(i => ({ bookId: i.bookId, quantity: i.quantity, price: i.price })) 
  }
  
  try {
    // 1. Lưu đơn hàng vào Database trước
    const res = await fetch('https://localhost:7260/api/orders/public', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(orderData)
    })
    
    if (res.ok) {
      const resultData = await res.json()
      const savedOrderId = resultData.orderId // Lấy ID đơn hàng vừa lưu
      
      localStorage.removeItem('clientCart') // Xóa giỏ hàng

      // 2. Xử lý theo phương thức thanh toán
      if (paymentMethod.value === 'MOMO') {
        // GỌI API MOMO
        const momoRes = await fetch('https://localhost:7260/api/payment/momo', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ orderId: savedOrderId.toString(), amount: totalAmount.value })
        })
        
        if (momoRes.ok) {
          const momoData = await momoRes.json()
          // Chuyển hướng khách sang trang thanh toán của MoMo
          window.location.href = momoData.payUrl 
        } else {
          alert('Lỗi tạo link MoMo, đơn hàng sẽ được chuyển sang dạng COD.')
          router.push('/my-orders')
        }
      } else {
        // Nếu là COD thì báo thành công rồi sang lịch sử đơn luôn
        alert('Đặt hàng thành công!')
        router.push('/my-orders') 
      }
    } else {
      alert('Lỗi khi chốt đơn từ Server!')
    }
  } catch (e) { 
    console.error(e)
    alert('Không thể kết nối đến Server!') 
  }
  finally { 
    isSubmitting.value = false 
  }
}

onMounted(() => { cart.value = JSON.parse(localStorage.getItem('clientCart')) || [] })
</script>