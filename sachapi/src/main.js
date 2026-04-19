import { createApp } from 'vue'
import App from './App.vue'
import router from './router/router.js'

const app = createApp(App)
app.use(router).mount('#app')
// Xóa dòng dưới đây nếu nó không cần thiết:
// app.mount('#app')
