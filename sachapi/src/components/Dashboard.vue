<template>
  <div class="container mt-4">
    <div class="mb-4">
      <h2 class="fw-bold mb-1">Bảng Điều Khiển</h2>
      <p class="text-muted mb-0">Tổng quan tình hình kinh doanh của B.Store</p>
    </div>

    <div class="row g-4 mb-5">
      <div class="col-md-6 col-xl-3">
        <div class="card bg-primary text-white shadow-sm border-0 rounded-4 h-100">
          <div class="card-body p-4">
            <h6 class="text-white-50 fw-bold text-uppercase mb-2">Tổng Doanh Thu</h6>
            <h3 class="fw-bold mb-0">{{ summary.totalRevenue.toLocaleString() }} đ</h3>
          </div>
        </div>
      </div>
      <div class="col-md-6 col-xl-3">
        <div class="card bg-success text-white shadow-sm border-0 rounded-4 h-100">
          <div class="card-body p-4">
            <h6 class="text-white-50 fw-bold text-uppercase mb-2">Tổng Đơn Hàng</h6>
            <h3 class="fw-bold mb-0">{{ summary.totalOrders }} đơn</h3>
          </div>
        </div>
      </div>
      <div class="col-md-6 col-xl-3">
        <div class="card bg-info text-white shadow-sm border-0 rounded-4 h-100">
          <div class="card-body p-4">
            <h6 class="text-white-50 fw-bold text-uppercase mb-2">Tổng Sách Trong Kho</h6>
            <h3 class="fw-bold mb-0">{{ summary.totalBooks }} cuốn</h3>
          </div>
        </div>
      </div>
      <div class="col-md-6 col-xl-3">
        <div class="card bg-danger text-white shadow-sm border-0 rounded-4 h-100">
          <div class="card-body p-4">
            <h6 class="text-white-50 fw-bold text-uppercase mb-2">Sắp Hết Hàng</h6>
            <h3 class="fw-bold mb-0">{{ summary.lowStockCount }} đầu sách</h3>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div class="col-lg-12">
        <div class="card shadow-sm border-0 rounded-4">
          <div class="card-header bg-white border-bottom-0 pt-4 pb-0 px-4">
            <h5 class="fw-bold text-danger">⚠️ Cảnh Báo Sách Sắp Hết Hàng (Tồn kho ≤ 5)</h5>
          </div>
          <div class="card-body p-4">
            <div class="table-responsive">
              <table class="table table-hover align-middle">
                <thead class="text-muted small text-uppercase">
                  <tr>
                    <th class="border-0">Mã Sách</th>
                    <th class="border-0">Tên Sách</th>
                    <th class="border-0 text-center">Số Lượng Còn Lại</th>
                    <th class="border-0 text-center">Hành Động</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="lowStockBooks.length === 0">
                    <td colspan="4" class="text-center py-4 text-muted">Tuyệt vời! Không có sách nào sắp hết hàng.</td>
                  </tr>
                  <tr v-for="book in lowStockBooks" :key="book.bookId">
                    <td class="fw-bold text-secondary">#{{ book.bookId }}</td>
                    <td class="fw-bold text-dark">{{ book.title }}</td>
                    <td class="text-center">
                      <span class="badge rounded-pill" :class="book.stockQuantity === 0 ? 'bg-danger' : 'bg-warning text-dark'">
                        {{ book.stockQuantity === 0 ? 'Hết sạch' : 'Còn ' + book.stockQuantity }}
                      </span>
                    </td>
                    <td class="text-center">
                      <router-link :to="`/edit-book/${book.bookId}`" class="btn btn-sm btn-outline-primary rounded-pill px-3 fw-semibold">
                        Nhập thêm
                      </router-link>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const summary = ref({ totalRevenue: 0, totalOrders: 0, totalBooks: 0, lowStockCount: 0 })
const lowStockBooks = ref([])

const fetchDashboardData = async () => {
  const token = localStorage.getItem('token')
  const headers = { 'Authorization': `Bearer ${token}` }

  try {
    // Gọi song song 2 API lấy dữ liệu tổng quan và danh sách hết hàng
    const [resSummary, resLowStock] = await Promise.all([
      fetch('https://localhost:7260/api/dashboard/summary', { headers }),
      fetch('https://localhost:7260/api/dashboard/low-stock', { headers })
    ])

    if (resSummary.ok) summary.value = await resSummary.json()
    if (resLowStock.ok) lowStockBooks.value = await resLowStock.json()
  } catch (error) {
    console.error('Lỗi tải dữ liệu Dashboard:', error)
  }
}

onMounted(() => fetchDashboardData())
</script>