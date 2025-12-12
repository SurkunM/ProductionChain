<template>
    <v-dialog v-model="isShow" @keydown.esc="hide" persistent max-width="600px">
        <v-card>
            <v-toolbar dark color="primary">
                <v-toolbar-title>Создание задачи</v-toolbar-title>

                <v-btn icon dark @click="hide">
                    <v-icon>mdi-close</v-icon>
                </v-btn>
            </v-toolbar>

            <v-form @submit.prevent="submitForm()">
                <v-card-text>
                    <v-select :items="productionOrders"
                              :item-props="productionOrderProps"
                              @update:model-value="onSelectProductionOrder"
                              label="Номер производсвтенного заказа">
                    </v-select>

                    <v-text-field v-model.trim="task.productName"
                                  label="Изделие"
                                  readonly>
                    </v-text-field>

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

                <v-card-actions class="me-2 mb-2">
                    <v-spacer></v-spacer>
                    <v-btn color="error" variant="flat" @click="hide">Отменить</v-btn>
                    <v-btn color="info" variant="flat" type="submit">Создать</v-btn>
                </v-card-actions>
            </v-form>
        </v-card>
    </v-dialog>
</template>

<script>
    export default {//TODO: Добавить инпуты для productionOrder (id, productId). Сделать валидацию для !id<=0
        data() {
            return {
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

            isShow() {
                return this.$store.getters.isShowTaskCreateModal;
            },

            productionOrders() {
                return this.$store.getters.productionOrders;
            }
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

                this.$store.dispatch("createProductionTask", this.task)
                    .then(() => {
                        this.hide();
                        this.$store.commit("setAlertMessage", "Задача успешно создана");
                        this.$store.commit("isShowSuccessAlert", true);
                    })
                    .catch(() => {
                        this.$store.commit("setAlertMessage", "Не удалось создать задачу");
                        this.$store.commit("isShowErrorAlert", true);
                    });
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

            productionOrderProps(item) {
                return {
                    title: item.id
                }
            },

            onSelectProductionOrder(item) {
                this.task.productId = item.productId;
                this.task.productName = item.productName;
                this.task.productionOrderId = item.id;
                this.task.productId = item.productId;
            },

            hide() {
                this.resetErrors();
                this.resetFields();

                this.$store.commit("isShowTaskCreateModal", false);
            },

            resetErrors() {
                this.errors = {
                    count: "",
                    employee: ""
                };
            },

            resetFields() {
                this.task = {
                    id: 0,
                    productName: "",
                    productsCount: "",
                    productId: 0,
                    productionOrderId: 0,
                    employeeId: 0,
                    employee: null
                }
            }
        },

        emits: ["save"]
    }
</script>