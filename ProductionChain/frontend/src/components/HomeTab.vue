<template>
    <v-snackbar v-model="isShowErrorAlert"
                :timeout="2000"
                location="bottom right"
                color="error">
        Не удалось загрузить данные
    </v-snackbar>

    <template v-if="isAuthorized">
        <v-container fluid>
            <v-row>
                <v-col cols="12" md="4">
                    <v-card title="Заказы в производстве" subtitle="В работе" prepend-icon="mdi-cart-plus">
                        <v-card-text class="text-h5 ms-2">
                            {{productionOrders.length}}
                        </v-card-text>
                    </v-card>
                </v-col>
                <v-col cols="12" md="4">
                    <v-card title="Активные задачи" subtitle="В работе" prepend-icon="mdi-checkbox-marked-circle">
                        <v-card-text class="text-h5 ms-2">
                            {{tasks.length}}
                        </v-card-text>
                    </v-card>
                </v-col>
                <v-col cols="12" md="4">
                    <v-card title="На складе" subtitle="Готовой продукции" prepend-icon="mdi-package-variant">
                        <v-card-text class="text-h5 ms-2">
                            {{assemblyWarehouseItemsCount}}
                        </v-card-text>
                    </v-card>
                </v-col>
            </v-row>

            <v-progress-linear v-if="isLoading"
                               indeterminate
                               color="primary">
            </v-progress-linear>

            <v-row class="mt-5">
                <v-col cols="12" md="6">
                    <v-card title="Последние заказы">
                        <v-table>
                            <thead>
                                <tr>
                                    <th>№</th>
                                    <th>Клиент</th>
                                    <th>Статус</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="order in orders" :key="order.id">
                                    <td>{{ order.index }}</td>
                                    <td>{{ order.customer }}</td>
                                    <td><v-chip :color="getOrderStatusColor(order.status)">{{ order.statusMap }}</v-chip></td>
                                </tr>
                            </tbody>
                        </v-table>
                    </v-card>
                </v-col>

                <v-col cols="12" md="6">
                    <v-card title="Сотрудники">
                        <v-table>
                            <thead>
                                <tr>
                                    <th>№</th>
                                    <th>Сотрудник</th>
                                    <th>Должность</th>
                                    <th>Статус</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="employee in employees" :key="employee.id">
                                    <td>{{ employee.index }}</td>
                                    <td>{{ employee.lastName }}</td>
                                    <td>{{ employee.positionMap }}</td>
                                    <td><v-chip :color="getEmployeeStatusColor(employee.status)">{{ employee.statusMap }}</v-chip></td>
                                </tr>
                            </tbody>
                        </v-table>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </template>

    <template v-else>
        <v-container class="fill-height" fluid>
            <v-row align="center" justify="center">
                <v-col cols="12" sm="8" md="6" lg="4">
                    <v-card class="text-center pa-8">
                        <v-icon size="64" color="grey-lighten-1" class="mb-4">
                            mdi-account-lock
                        </v-icon>
                        <v-card-title class="text-h5 justify-center">
                            Требуется авторизация
                        </v-card-title>
                        <v-card-text>
                            <p class="text-body-1 mb-4">
                                Для просмотра этой страницы необходимо войти в систему
                            </p>
                            <v-btn color="primary" @click="showLoginModal()">
                                Войти
                            </v-btn>
                        </v-card-text>
                    </v-card>
                </v-col>
            </v-row>
        </v-container>
    </template>
</template>

<script>
    export default {
        data() {
            return {
                isShowErrorAlert: false
            }
        },

        created() {
            if (!this.isAuthorized) {
                return;
            }

            this.loadData();
        },

        computed: {
            employees() {
                return this.$store.getters.employees;
            },

            tasks() {
                return this.$store.getters.tasks;
            },

            orders() {
                return this.$store.getters.orders;
            },

            productionOrders() {
                return this.$store.getters.productionOrders;
            },

            assemblyWarehouseItemsCount() {
                return this.$store.getters.assemblyWarehouseItemsCount;
            },

            isLoading() {
                return this.$store.getters.isLoading;
            },

            isAuthorized() {
                return this.$store.getters.isAuthorized;
            }
        },

        methods: {
            loadData() {
                this.$store.dispatch("loadEmployees")
                    .catch(() => this.isShowErrorAlert = true);

                this.$store.dispatch("loadProductionOrders")
                    .catch(() => this.isShowErrorAlert = true);

                this.$store.dispatch("loadProductionTasks")
                    .catch(() => this.isShowErrorAlert = true);

                this.$store.dispatch("loadAssemblyWarehouseItems")
                    .catch(() => this.isShowErrorAlert = true);

                this.$store.dispatch("loadOrders")
                    .catch(() => this.isShowErrorAlert = true);
            },

            getOrderStatusColor(status) {
                if (status === "Pending") {
                    return "error";
                }

                if (status === "InProgress") {
                    return "warning";
                }

                if (status === "Done") {
                    return "success";
                }
            },

            getEmployeeStatusColor(status) {
                if (status === "OnLeave") {
                    return "grey";
                }

                if (status === "Busy") {
                    return "blue"
                }

                if (status === "Available") {
                    return "green";
                }
            },

            showLoginModal() {
                this.$store.commit("setIsShowLoginModal", true)
            }
        }
    }
</script>