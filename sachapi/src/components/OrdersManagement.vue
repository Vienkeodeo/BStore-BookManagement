<template>
  <div class="container mt-4">
    <div class="mb-4">
      <h2 class="fw-bold mb-1">Quản Lý Đơn Hàng</h2>
      <p class="text-muted mb-0">Theo dõi và cập nhật trạng thái vận chuyển</p>
    </div>

    <div class="card shadow-sm border-0 rounded-4 mb-4">
      <div class="card-body p-3">
        <div class="row align-items-center">
          <div class="col-md-4">
            <input v-model="filterName" type="text" class="form-control rounded-pill px-4 bg-light border-0" placeholder="🔍 Tìm theo tên khách..." />
          </div>
          <div class="col-md-3">
            <select v-model="filterStatus" class="form-select rounded-pill px-4 bg-light border-0">
              <option value="">Tất cả trạng thái</option>
              <option value="Pending">Chờ xử lý (Pending)</option>
              <option value="Processing">Đang đóng gói (Processing)</option>
              <option value="Shipped">Đang giao (Shipped)</option>
              <option value="Delivered">Đã giao (Delivered)</option>
              <option value="Cancelled">Đã hủy (Cancelled)</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <div class="card shadow-sm border-0 rounded-4">
      <div class="card-body p-4">
        <div class="table-responsive">
          <table class="table table-hover align-middle">
            <thead class="text-muted small text-uppercase">
              <tr>
                <th class="border-0">Mã Đơn</th>
                <th class="border-0">Khách Hàng</th>
                <th class="border-0">Ngày Đặt</th>
                <th class="border-0">Tổng Tiền</th>
                <th class="border-0">Trạng Thái</th>
                <th class="border-0 text-center">Hành Động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="order in filteredOrders" :key="order.orderId">
                <td class="fw-bold text-primary">#{{ order.orderId }}</td>
                <td>
                  <div class="fw-bold">{{ order.customerName }}</div>
                  <div class="text-muted small">{{ order.phoneNumber }}</div>
                </td>
                <td class="text-muted">{{ formatDate(order.orderDate) }}</td>
                <td class="fw-bold text-danger">{{ order.totalAmount.toLocaleString() }} đ</td>
                <td>
                  <select 
                    :value="order.status" 
                    @change="changeStatus(order.orderId, $event.target.value)"
                    class="form-select form-select-sm rounded-pill fw-bold"
                    :class="statusClass(order.status)"
                  >
                    <option value="Pending">Chờ xử lý</option>
                    <option value="Processing">Đang xử lý</option>
                    <option value="Shipped">Đang giao</option>
                    <option value="Delivered">Đã giao</option>
                    <option value="Cancelled">Đã hủy</option>
                  </select>
                </td>
                <td class="text-center">
                  <button class="btn btn-sm btn-light border rounded-pill px-3" @click="viewDetail(order)">Chi tiết</button>
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

const orders = ref([])
const filterName = ref('')
const filterStatus = ref('')
const API_URL = 'https://localhost:7260/api/orders'

const fetchOrders = async () => {
  const token = localStorage.getItem('token')
  const res = await fetch(API_URL, { headers: { 'Authorization': `Bearer ${token}` } })
  if (res.ok) orders.value = await res.json()
}

const changeStatus = async (id, newStatus) => {
  const token = localStorage.getItem('token')
  const res = await fetch(`${API_URL}/${id}/status`, {
    method: 'PUT',
    headers: { 
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}` 
    },
    body: JSON.stringify(newStatus)
  })
  
  if (res.ok) {
    // Cập nhật lại list tại chỗ cho nhanh
    const order = orders.value.find(o => o.orderId === id)
    if (order) order.status = newStatus
  } else {
    alert('Không thể cập nhật trạng thái!')
  }
}

const filteredOrders = computed(() => {
  return orders.value.filter(o => {
    const matchName = o.customerName.toLowerCase().includes(filterName.value.toLowerCase())
    const matchStatus = filterStatus.value === '' || o.status === filterStatus.value
    return matchName && matchStatus
  })
})

const statusClass = (status) => {
  switch (status) {
    case 'Pending': return 'bg-warning bg-opacity-10 text-dark border-warning'
    case 'Processing': return 'bg-info bg-opacity-10 text-info border-info'
    case 'Shipped': return 'bg-primary bg-opacity-10 text-primary border-primary'
    case 'Delivered': return 'bg-success bg-opacity-10 text-success border-success'
    case 'Cancelled': return 'bg-danger bg-opacity-10 text-danger border-danger'
    default: return 'bg-light text-dark'
  }
}

const formatDate = (dateStr) => {
  return new Date(dateStr).toLocaleString('vi-VN')
}

onMounted(() => fetchOrders())
</script>