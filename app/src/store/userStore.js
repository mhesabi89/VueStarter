import { defineStore } from "pinia";
import { jwtDecode } from "jwt-decode";
export const useUserStore = defineStore("users", {
  state: () => ({}),
  getters: {
    currentUser: () => {
      const access_token = localStorage.getItem("access_token");
      if (access_token == null) return null;
      try {
        const jwtPayload = jwtDecode(access_token);
        if (jwtPayload.exp >= Date.now() / 1000) {
          const user = {
            mobile: jwtPayload.unique_name,
            fullname: jwtPayload.fullname,
          };

          return user;
        }
      } catch (e) {
        console.error(e);
      }

      localStorage.removeItem("access_token");
      return null;
    },
  },
  actions: {
    login(res) {
      // this.userData = res.user;
      localStorage.setItem("access_token", res.access_token);
    },
    logout() {
      // this.userData = null;
      localStorage.removeItem("access_token");
    },
  },
});
