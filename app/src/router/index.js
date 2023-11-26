import { createRouter, createWebHistory } from "vue-router";
import MainView from "../components/Layout/Layout.vue";
import LoginView from "../views/LoginView.vue";
import DashboardView from "../components/Dashboard/test.vue";
import NotFoundView from "../views/NotFoundView.vue";
import { useUserStore } from "../store/userStore";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/login",
      component: LoginView,
    },
    {
      path: "/",
      children: [
        {
          path: "/dashboard",
          alias: "",
          name: "Dashboard",
          component: DashboardView,
        },
      ],
      meta: {
        requiresAuth: true,
      },
    },
    {
      // not found handler
      path: "/:pathMatch(.*)*",
      component: NotFoundView,
    },
  ],
});

router.beforeEach((to, from, next) => {
  if (to.meta.requiresAuth) {
    const storeUser = useUserStore();
    const currentUser = storeUser.currentUser;
    if (currentUser) {
      // User is authenticated, proceed to the route
      next();
    } else {
      // User is not authenticated, redirect to login
      next("/login");
    }
  } else {
    // Non-protected route, allow access
    next();
  }
});

export default router;
