<template>
  <div class="bg-light min-vh-100 pb-5">
    <nav class="navbar navbar-expand-lg bg-white shadow-sm py-3 mb-4 sticky-top">
      <div class="container">
        <router-link class="navbar-brand fw-bold text-primary fs-3" to="/home">📚 B.Store</router-link>
        <router-link to="/home" class="btn btn-outline-secondary rounded-pill px-4 fw-medium">Tiếp tục mua sắm</router-link>
      </div>
    </nav>

    <div class="container" style="max-width: 800px;">
      <div class="text-center mb-4">
        <h2 class="fw-bold">Lịch Sử Đơn Hàng</h2>
        <p class="text-muted">Quản lý các đơn hàng của tài khoản <strong class="text-primary">{{ currentUsername }}</strong></p>
      </div>

      <div class="d-flex gap-2 mb-4 overflow-auto pb-2 justify-content-center" v-if="orders.length > 0">
        <button @click="selectedStatus = 'All'" :class="['btn rounded-pill px-4 fw-medium text-nowrap transition-all', selectedStatus === 'All' ? 'btn-dark' : 'btn-outline-secondary bg-white']">
          Tất cả
        </button>
        <button @click="selectedStatus = 'Pending'" :class="['btn rounded-pill px-4 fw-medium text-nowrap transition-all', selectedStatus === 'Pending' ? 'btn-warning text-dark border-warning' : 'btn-outline-secondary bg-white']">
          Chờ xử lý
        </button>
        <button @click="selectedStatus = 'Shipped'" :class="['btn rounded-pill px-4 fw-medium text-nowrap transition-all', selectedStatus === 'Shipped' ? 'btn-primary border-primary' : 'btn-outline-secondary bg-white']">
          Đang giao
        </button>
        <button @click="selectedStatus = 'Delivered'" :class="['btn rounded-pill px-4 fw-medium text-nowrap transition-all', selectedStatus === 'Delivered' ? 'btn-success border-success' : 'btn-outline-secondary bg-white']">
          Đã nhận
        </button>
        <button @click="selectedStatus = 'Cancelled'" :class="['btn rounded-pill px-4 fw-medium text-nowrap transition-all', selectedStatus === 'Cancelled' ? 'btn-danger border-danger' : 'btn-outline-secondary bg-white']">
          Đã hủy
        </button>
      </div>

      <div v-if="isLoading" class="text-center py-5 text-muted">
        <div class="spinner-border text-primary mb-3" role="status"></div>
        <h5>Đang tải dữ liệu...</h5>
      </div>

      <div v-else-if="filteredOrders.length > 0">
        <div v-for="order in filteredOrders" :key="order.orderId" class="card border-0 shadow-sm rounded-4 mb-4 order-card">
          <div class="card-header bg-white border-bottom p-4 d-flex justify-content-between align-items-center">
            <div>
              <span class="text-muted small">Mã đơn: </span><span class="fw-bold text-primary">#{{ order.orderId }}</span>
            </div>
            <span class="badge rounded-pill px-3 py-2" :class="statusClass(order.status)">{{ formatStatus(order.status) }}</span>
          </div>
          <div class="card-body p-4">
            <div v-for="item in order.orderDetails" :key="item.orderDetailId" class="d-flex align-items-center mb-3">
              <img :src="item.book?.coverImageUrl || 'https://via.placeholder.com/50x70'" class="rounded-2 me-3 shadow-sm" style="width: 50px; height: 70px; object-fit: cover;">
              <div class="flex-grow-1">
                <div class="fw-bold small">{{ item.book?.title }}</div>
                <div class="text-muted small">SL: {{ item.quantity }} x {{ item.unitPrice?.toLocaleString() || item.price?.toLocaleString() }}đ</div>
              </div>
            </div>
            <hr>
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <span class="fw-bold text-secondary">Tổng thanh toán: </span>
                <span class="fw-bold fs-5 text-danger">{{ order.totalAmount.toLocaleString() }}đ</span>
              </div>
              
              <div class="d-flex gap-2">
                <button v-if="order.status === 'Pending'" @click="openPaymentModal(order)" class="btn btn-primary btn-sm rounded-pill px-3 fw-bold shadow-sm">
                  💳 Thanh toán lại
                </button>
                <button v-if="order.status === 'Pending'" @click="cancelOrder(order.orderId)" class="btn btn-outline-danger btn-sm rounded-pill px-3 fw-bold">
                  Hủy đơn
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div v-else class="text-center py-5 text-muted">
        <h5 class="mb-3" v-if="orders.length === 0">Bạn chưa có đơn hàng nào!</h5>
        <h5 class="mb-3" v-else>Không có đơn hàng nào ở trạng thái này.</h5>
        <router-link to="/home" class="btn btn-primary rounded-pill px-4 mt-2">Mua sắm ngay</router-link>
      </div>
    </div>

    <div v-if="selectedOrder" class="modal-backdrop fade show" style="background-color: rgba(0,0,0,0.5);"></div>
    <div v-if="selectedOrder" class="modal fade show d-block" tabindex="-1">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content rounded-4 border-0 shadow-lg">
          <div class="modal-header border-bottom-0 pb-0">
            <h5 class="modal-title fw-bold">Chọn phương thức thanh toán</h5>
            <button type="button" class="btn-close" @click="closePaymentModal"></button>
          </div>
          <div class="modal-body p-4">
            <p class="text-muted small mb-4">Mã đơn: <strong>#{{ selectedOrder.orderId }}</strong> - Tổng: <strong class="text-danger">{{ selectedOrder.totalAmount.toLocaleString() }}đ</strong></p>
            
            <button @click="retryPayment('COD')" class="btn btn-light border w-100 py-3 mb-3 fw-bold text-start rounded-3 hover-btn">
              💵 Thanh toán khi nhận hàng (COD)
            </button>
            <button @click="retryPayment('MOMO')" class="btn border border-danger w-100 py-3 fw-bold text-start rounded-3 hover-btn text-danger">
              <img src="https://upload.wikimedia.org/wikipedia/vi/f/fe/MoMo_Logo.png" style="width: 25px; margin-right: 10px;">
              Thanh toán qua Ví MoMo
            </button>
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
const orders = ref([])
const isLoading = ref(true)
const currentUsername = ref('')
const selectedStatus = ref('All') // Trạng thái mặc định là "Tất cả"
const selectedOrder = ref(null)

// Computed property tự động lọc đơn hàng
const filteredOrders = computed(() => {
  if (selectedStatus.value === 'All') {
    return orders.value
  }
  return orders.value.filter(order => order.status === selectedStatus.value)
})

const fetchMyOrders = async () => {
  isLoading.value = true
  const customerInfo = JSON.parse(localStorage.getItem('customerInfo'))
  if (!customerInfo || !customerInfo.username) {
    alert('Vui lòng đăng nhập để xem đơn hàng!')
    router.push('/')
    return
  }
  currentUsername.value = customerInfo.username
  try {
    const res = await fetch(`https://localhost:7260/api/orders/my-orders/${currentUsername.value}`)
    if (res.ok) orders.value = await res.json()
    else orders.value = []
  } catch (e) { console.error('Lỗi tải đơn hàng:', e) } 
  finally { isLoading.value = false }
}

const cancelOrder = async (orderId) => {
  if (!confirm('Bạn có chắc chắn muốn hủy đơn hàng này không? Sách sẽ được hoàn lại kho.')) return;
  try {
    const res = await fetch(`https://localhost:7260/api/orders/${orderId}/cancel`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username: currentUsername.value })
    });
    if (res.ok) {
      alert('Đã hủy đơn hàng thành công!');
      fetchMyOrders();
    } else {
      const err = await res.json();
      alert(err.message || 'Lỗi hủy đơn!');
    }
  } catch (e) { alert('Lỗi kết nối máy chủ!'); }
}

const openPaymentModal = (order) => selectedOrder.value = order
const closePaymentModal = () => selectedOrder.value = null

const retryPayment = async (method) => {
  if (!selectedOrder.value) return
  if (method === 'COD') {
    alert('Đơn hàng của bạn đã được ghi nhận thanh toán tiền mặt (COD)!')
    closePaymentModal()
  } 
  else if (method === 'MOMO') {
    try {
      const res = await fetch('https://localhost:7260/api/payment/momo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ 
          orderId: selectedOrder.value.orderId.toString(), 
          amount: selectedOrder.value.totalAmount 
        })
      })
      if (res.ok) {
        const data = await res.json()
        window.location.href = data.payUrl 
      } else {
        alert('Lỗi tạo link thanh toán MoMo!')
      }
    } catch (e) {
      alert('Lỗi kết nối đến máy chủ thanh toán!')
    }
  }
}

// Hàm format Tiếng Việt cho Badge trạng thái
const formatStatus = (status) => {
  switch (status) {
    case 'Pending': return 'Chờ xử lý'
    case 'Shipped': return 'Đang giao'
    case 'Delivered': return 'Đã nhận'
    case 'Cancelled': return 'Đã hủy'
    default: return status
  }
}

// Đổi màu Badge
const statusClass = (status) => {
  switch (status) {
    case 'Pending': return 'bg-warning text-dark'
    case 'Shipped': return 'bg-primary'
    case 'Delivered': return 'bg-success'
    case 'Cancelled': return 'bg-danger'
    default: return 'bg-secondary'
  }
}

onMounted(() => fetchMyOrders())
</script>

<style scoped>
.transition-all { transition: all 0.2s ease-in-out; }
.hover-btn { transition: all 0.2s; }
.hover-btn:hover { background-color: #f8f9fa; transform: translateY(-2px); }
.modal-backdrop { z-index: 1040; }
.modal { z-index: 1050; }
.order-card { transition: box-shadow 0.2s; }
.order-card:hover { box-shadow: 0 10px 20px rgba(0,0,0,0.05) !important; }
::-webkit-scrollbar { height: 6px; }
::-webkit-scrollbar-thumb { background: #dee2e6; border-radius: 10px; }
</style>