<template>
    <v-dialog v-model="isShow" persistent max-width="600px">
        <v-card>
            <v-toolbar dark color="primary">
                <v-toolbar-title>Создание задачи</v-toolbar-title>

                <v-btn icon dark @click="hide">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>

            <v-form @submit.prevent="submitForm">
                <v-card-text>
                    <div class="mb-2">
                        <span class="text-h6 font-weight-bold ms-1">Изделие: </span>
                        <span class="text-h5"> {{ task.productName }} </span>
                    </div>

                    <v-text-field v-model.trim="task.productsCount"
                                  label="Количество"
                                  :error-messages="errors.count"
                                  autocomplete="off"
                                  @change="checkProductCountFieldComplete">
                    </v-text-field>

                    <v-select :items="employees"
                              :item-props="employeeProps"
                              @update:model-value="onSelectEmployee"
                              label="Монтажник РЭА">
                    </v-select>
                </v-card-text>

                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="info" type="submit">Создать</v-btn>
                    <v-btn color="error" @click="hide">Отменить</v-btn>
                </v-card-actions>
            </v-form>
        </v-card>
    </v-dialog>
</template>

<script>
    export default {
        data() {
            return {
                isShow: false,

                task: {
                    id: 0,
                    productName: "",
                    productsCount: "",
                    productId: 0,
                    productionOrderId: 0,
                    employeeId: 0,
                    employee: null
                },

                errors: {
                    productName: "",
                    count: "",
                    employee: ""
                }
            };
        },

        computed: {
            employees() {
                return this.$store.getters.employees;
            },
        },

        methods: {
            checkEditingFieldsIsvalid(task) {
                this.resetErrors();
                let isValid = true;

                if (task.productsCount.length === 0) {
                    this.errors.count = "Заполните поле count";
                    isValid = false;
                }

                return isValid;
            },

            checkProductCountFieldComplete() {
                if (this.task.productsCount.length > 0) {
                    this.errors.count = "";
                }
            },

            checkEmployeeFieldComplete() {
                if (!this.task.employee) {
                    this.errors.employee = "Выберите сотрудника";
                } else {
                    this.errors.employee = "";
                }
            },

            submitForm() {
                if (!this.checkEditingFieldsIsvalid(this.task)) {
                    return;
                }

                this.$emit('save', this.task)
            },

            onSelectEmployee(employee) {
                this.task.employeeId = employee.id;
            },

            employeeProps(employee) {
                return {
                    title: `${employee.lastName} ${employee.firstName || ""} ${employee.middleName || " "}`,
                    subtitle: employee.status,
                }
            },

            show(productionOrder) {
                this.resetErrors();

                this.task.productionOrderId = productionOrder.id;
                this.task.productId = productionOrder.productId;

                this.task.productName = `${productionOrder.productName || ''} (${productionOrder.productModel || ''})`;

                this.$store.dispatch("loadEmployees");

                this.isShow = true;
            },

            hide() {
                this.isShow = false;
            },

            resetErrors() {
                this.errors = {
                    count: "",
                    employee: ""
                };
            }
        },

        emits: ["save"]
    }
</script>