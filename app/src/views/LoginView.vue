<template>
  <div id="loginBox">
    <div class="inner">
      <SendOTPView
        v-if="step == 'SendOTP'"
        :setting="{
          SendOTP: sendOTP,
          counter: counter,
          loading: loading,
        }"
      />
      <VerifyOTPView
        v-if="step == 'VerifyOTP'"
        :setting="{
          counter: counter,
          mobile: mobile,
          backButton: backButton,
          showResendButton: showResendButton,
          SendOTP: sendOTP,
          Loading: loading2,
          Redirecting: redirecting,
          VerifyOTP: VerifyOTP,
        }"
      />
    </div>
  </div>
</template>
<style scoped>
#loginBox {
  max-width: 400px;
  margin: 6% auto 0;
}
#loginBox > .inner {
  padding: 50px 30px;
  border-radius: 15px;
  border: 1px solid #eee;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}
</style>
<script setup>
import { ref } from "vue";
import { useUserStore } from "../store/userStore";
import api from "axios";
import SendOTPView from "../components/Auth/SendOTP.vue";
import VerifyOTPView from "../components/Auth/VerifyOTP.vue";

const attempt = ref(0),
  counter = ref(0),
  loading = ref(false),
  showResendButton = ref(true),
  mobile = ref(""),
  loading2 = ref(false),
  redirecting = ref(false),
  userStore = useUserStore();
let step = ref("SendOTP");

const sendOTP = (mobileInput) => {
  loading.value = true;
  let timer = 0;
  showResendButton.value = false;
  api.post(`/auth/otp/send?mobile=${mobileInput}`).then((res) => {
    step.value = "VerifyOTP";
    mobile.value = mobileInput;
    attempt.value++;
    counter.value = attempt.value * 15;
    timer = counter.value;
    showResendButton.value = true;

    let counterInterval = setInterval(function () {
      if (counter.value > 0) {
        counter.value--;
      } else {
        counter.value = 0;
        loading.value = false;
      }
    }, 1000);

    setTimeout(function () {
      clearInterval(counterInterval);
      counter.value = 0;
      loading.value = false;
    }, timer * 1000);
  });
};

function backButton() {
  step.value = "SendOTP";
}

const VerifyOTP = (codeInput) => {
  loading2.value = true;
  api
    .post(`/auth/otp/verify?mobile=${mobile.value}&code=${codeInput}`)
    .then((res) => {
      if (res.success == true) {
        userStore.login(res);
        redirecting.value = true;
        window.location.href = "/";
      } else {
        alert(res.error);
      }

      loading2.value = false;
    });
};
</script>