import { createRouter, createWebHistory } from 'vue-router'

// --- Auth ---
import Login from '../auth/Login.vue' // Form đăng nhập của Admin
import CustomerAuth from '../auth/CustomerAuth.vue' // Form đăng ký/nhập Khách hàng

// --- Khách Hàng ---
import Home from '../components/Home.vue'
import ClientCart from '../components/ClientCart.vue'
import BookDetail from '../components/BookDetail.vue'
import Checkout from '../components/Checkout.vue'
import CustomerOrders from '../components/CustomerOrders.vue'

// --- Admin ---
import Dashboard from '../components/Dashboard.vue'
import Books from '../components/Books.vue'
import AddBook from '../components/AddBook.vue' 
import EditBook from '../components/EditBook.vue'
import Categories from '../components/Categories.vue'
import AddCategory from '../components/AddCategory.vue'
import EditCategory from '../components/EditCategory.vue'
import Authors from '../components/Authors.vue'
import AddAuthor from '../components/AddAuthor.vue'
import EditAuthor from '../components/EditAuthor.vue'
import Publishers from '../components/Publishers.vue'
import AddPublisher from '../components/AddPublisher.vue'
import EditPublisher from '../components/EditPublisher.vue'
import CreateOrder from '../components/CreateOrder.vue'
import ManageOrder from '../components/OrdersManagement.vue'
import CustomerProfile from '../components/CustomerProfile.vue'

const routes = [
  // Luồng Khách hàng
  { path: '/', component: CustomerAuth }, // Mở web là vào đăng nhập khách
  { path: '/home', component: Home }, // Đăng nhập xong vào đây
  { path: '/cart', component: ClientCart },
  { path: '/book/:id', component: BookDetail },
  { path: '/checkout', component: Checkout },
  { path: '/my-orders', component: CustomerOrders },
  { path: '/profile', component: CustomerProfile },

  // Luồng Admin
  { path: '/admin', component: Login }, // Cửa sau cho Admin
  { path: '/dashboard', component: Dashboard },
  { path: '/books', component: Books },
  { path: '/add-book', component: AddBook },
  { path: '/edit-book/:id', component: EditBook },
  { path: '/categories', component: Categories },
  { path: '/add-category', component: AddCategory },
  { path: '/edit-category/:id', component: EditCategory },
  { path: '/authors', component: Authors },
  { path: '/add-author', component: AddAuthor },
  { path: '/edit-author/:id', component: EditAuthor },
  { path: '/publishers', component: Publishers },
  { path: '/add-publisher', component: AddPublisher },
  { path: '/edit-publisher/:id', component: EditPublisher },
  { path: '/create-order', component: CreateOrder },
  { path: '/manage-orders', component: ManageOrder },
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// Các trang không cần token Admin
const publicPages = ['/', '/home', '/admin', '/cart', '/checkout', '/my-orders', '/profile'];

router.beforeEach((to, from, next) => {
  const isPublic = publicPages.includes(to.path) || to.path.startsWith('/book/');
  const token = localStorage.getItem('token');

  if (!isPublic && !token) {
    next('/admin'); // Chưa có token mà đòi vào admin thì bắt về /admin
  } else {
    next();
  }
})

export default router