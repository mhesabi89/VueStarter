<template>
  <Form @submit="checkForm" :validation-schema="formSchema">
    <div class="mb-3">
      <label class="form-label" for="mobile">جهت ورود به حساب کاربری شماره موبایل خود را وارد نمایید</label>
      <Field
        type="text"
        name="mobile"
        id="mobile"
        class="form-control text-center ltr"
        placeholder="شماره موبایل"
        inputmode="numeric"
      />
      <ErrorMessage name="mobile" v-slot="{ message }">
        <div class="alert alert-danger mt-2">
          {{ message }}
        </div>
      </ErrorMessage>
    </div>
    <div class="text-center">
      <button
        class="btn btn-primary w-100"
        v-bind:class="{ disabled: props.setting.loading }"
        :disabled="props.setting.loading"
      >
        دریافت رمز یکبار مصرف
        <span v-if="props.setting.counter > 0">
          (تا {{ props.setting.counter }} ثانیه دیگر)
        </span>
      </button>
    </div>
  </Form>
</template>

<script setup>
import { ref } from "vue";
import { Field, Form, ErrorMessage } from "vee-validate";
import * as Yup from "yup";
const props = defineProps(["setting"]);
const formSchema = Yup.object({
  mobile: Yup.string()
    .required("تکمیل فیلد الزامی است")
    .matches(/09\d{9}/, "شماره موبایل می بایست 11 رقم و با 09 شروع شود"),
});

const checkForm = (values) => {
  props.setting.SendOTP(values.mobile);
};
</script>