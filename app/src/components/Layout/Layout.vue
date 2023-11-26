<template>
  <header>
    <div class="container">
      <div class="d-flex">
        <div class="flex-grow-1">
          <h2>Hello Bitches</h2>
        </div>
        <div class="actions">
          <a href="#" @click="logout()">{{ userStore.currentUser?.fullname }} (Logout)</a>
        </div>
      </div>
    </div>
  </header>
  <div id="main">
    <slot />
  </div>
</template>
<script setup>
import { useUserStore } from "../../store/userStore";
const userStore = useUserStore();
function logout() {
  event.preventDefault();
  userStore.logout();
  window.location.href = "/";
}
</script>