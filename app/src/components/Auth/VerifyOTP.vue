<template>
  <Form @submit="checkForm" :validation-schema="formSchema">
    <div class="mb-3">
      <label for="code" class="form-label"
        >کد ارسال شده به شماره همراه {{ props.setting.mobile }} را وارد
        کنید.</label
      >
      <Field
        type="text"
        name="code"
        id="code"
        class="form-control text-center ltr"
        placeholder="رمز یکبار مصرف"
        inputmode="numeric"
        autocomplete="off"
      />
      <ErrorMessage name="code" v-slot="{ message }">
        <div class="alert alert-danger mt-2">
          {{ message }}
        </div>
      </ErrorMessage>
    </div>
    <div v-if="props.setting.Redirecting" class="alert alert-success">
      احراز هویت با موفقیت انجام شد. شما در حال انتقال به صفحه بعدی هستید. لطفا
      منتظر باشید
    </div>
    <div class="text-center">
      <div class="row">
        <div class="col-6">
          <button
            class="btn btn-primary w-100"
            v-bind:class="{
              disabled: props.setting.Loading || counter.valueOf() == 0,
            }"
            :disabled="props.setting.Loading || counter.valueOf() == 0"
          >
            بررسی کد
          </button>
        </div>
        <div class="col-6">
          <button
            type="button"
            @click="backButton()"
            class="btn btn-secondary w-100"
          >
            بازگشت
          </button>
        </div>
      </div>
      <div class="mt-3">
        <button
          type="button"
          class="btn btn-link flex-grow-1"
          @click="sendOTP()"
          v-bind:class="{ disabled: props.setting.counter > 0 }"
        >
          ارسال مجدد کد
          <span v-if="props.setting.counter > 0"
            >({{ props.setting.counter }} ثانیه دیگر)</span
          >
        </button>
      </div>
      <div class="mt-3" v-if="counter.valueOf() <= 15">
        <div class="alert alert-warning">
          با توجه به پایان مهلت ثبت رمز یکبار مصرف
          <span v-if="counter.valueOf() > 0"
            >تا {{ counter.valueOf() }} دیگر </span
          >، در صورت تمایل جهت دریافت کد جدید از دکمه ارسال مجدد استفاده کنید
        </div>
      </div>
    </div>
  </Form>
</template>

<script setup>
import { ref } from "vue";
import { Field, Form, ErrorMessage } from "vee-validate";
import * as Yup from "yup";
const props = defineProps(["setting"]);
const counterInitValue = 120;
let counter = ref(counterInitValue);
const formSchema = Yup.object({
  code: Yup.string()
    .required("تکمیل فیلد الزامی است")
    .matches(/\d{5}/, "رمز یکبار مصرف 5 رقمی می باشد"),
});

function init() {
  counter.value = counterInitValue;
  const timer = setInterval(() => {
    if (counter.value > 0) {
      counter.value--;
    } else {
      clearInterval(timer);
    }
  }, 1000);
}

init();

const checkForm = (values) => {
  props.setting.VerifyOTP(values.code);
};

const backButton = () => {
  init();
  props.setting.backButton();
};

const sendOTP = () => {
  init();
  props.setting.SendOTP(props.setting.mobile);
};
</script>