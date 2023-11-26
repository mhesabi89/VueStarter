import "./assets/main.css";
import "../node_modules/bootstrap/dist/css/bootstrap.rtl.min.css";

import { createApp } from "vue";
import { createPinia } from "pinia";
import axios from "axios";
import VueAxios from "vue-axios";
import App from "./App.vue";
import router from "./router";

const app = createApp(App);
const store = createPinia();

// axios
axios.defaults.baseURL = import.meta.env.VITE_API_BASE_URL;
axios.interceptors.request.use((config) => {
  const token = localStorage.getItem("access_token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
axios.interceptors.response.use(
  (response) => {
    return response.data;
  },
  (error) => {
    console.log(error);
    if (error.response == null) {
      router.push(history.state.current);
      return Promise.reject(error);
      // history.push(history.state.current);
      //window.location.href = "/error/network-error";
    } else {
      switch (error.response.status) {
        case 403:
          // history.push(history.state.current);
          window.location.href = "/error/403";
          return Promise.reject(error);
          break;
        case 401:
          // agar error az noe refresh_token bood
          if (error.response.data.error === "refresh_token") {
            // mojadadan state ra ba data jadid ke az api mikhoonim por mikonim
            store.dispatch(
              authRedux.actions.login(error.response.data.access_token)
            );
          } else {
            if (error.response.data.error === "invalid_token") {
              // token valid naboode va user bayad dobare login kone
              store.dispatch(authRedux.actions.logout());
            }
            window.location.href = "/login";
            return Promise.reject(error);
          }
          break;
        default:
          return Promise.reject(error);
      }
    }
  }
);

app.use(store).use(router);
app.config.globalProperties.$api = axios;
app.mount("#app");
