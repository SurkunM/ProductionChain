﻿<template>
    <v-card title="Заказы"
            flat>
        <v-progress-linear v-if="isLoading"
                           indeterminate
                           color="primary"
                           height="4">
        </v-progress-linear>

        <v-snackbar v-model="isShowSuccessAlert"
                    :timeout="2000"
                    color="success">
            {{alertText}}
        </v-snackbar>
        <v-snackbar v-model="isShowErrorAlert"
                    :timeout="2000"
                    color="error">
            {{alertText}}
        </v-snackbar>

        <template v-slot:text>
            <v-text-field v-model="term"
                          label="Найти"
                          autocomplete="off"
                          prepend-inner-icon="mdi-magnify"
                          variant="outlined"
                          hide-details
                          single-line
                          @keyup.enter="search">
                <template v-slot:append-inner>
                    <v-btn icon
                           @click="search"
                           color="primary"
                           size="small">
                        <v-icon>mdi-magnify</v-icon>
                    </v-btn>
                    <v-icon @click="cancelSearch"
                            style="cursor: pointer;"
                            size="x-large"
                            class="ms-1 me-2">
                        mdi-close-circle
                    </v-icon>
                </template>
            </v-text-field>
        </template>

        <v-data-table :headers="headers"
                      :items="orders"
                      hide-default-footer
                      :items-per-page="itemsPerPage"
                      no-data-text="Список пуст">

            <template v-slot:[`header.customer`]="{ column }">
                <button @click="sortBy(`customer`)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.productName`]="{ column }">
                <button @click="sortBy(`product.Name`)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.orderedProductsCount`]="{ column }">
                <button @click="sortBy(`orderedProductsCount`)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.availableProductsCount`]="{ column }">
                <button @click="sortBy(`availableProductsCount`)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`header.status`]="{ column }">
                <button @click="sortBy(column.status)">{{column.title}}</button>
                <v-icon v-if="sortByColumn === column.value">
                    {{ sortDesc ? 'mdi-menu-up' : 'mdi-menu-down' }}
                </v-icon>
            </template>

            <template v-slot:[`item.status`]="{ value }">
                <v-chip :border="`${getColor(value)} thin opacity-25`"
                        :color="getColor(value)"
                        :text="value"
                        size="small"></v-chip>
            </template>

            <template v-slot:[`item.actions`]="{ item }">
                <div>
                    <template v-if="isStatusPending(item.status)">
                        <v-btn size="small" color="info" @click="createProductionOrder(item)">Начать</v-btn>
                    </template>
                </div>
            </template>
        </v-data-table>

        <v-pagination v-model="currentPage"
                      :length="pagesCount"
                      @update:modelValue="switchPage"
                      circle
                      color="primary">
        </v-pagination>
    </v-card>
</template>
<script>
    export default {
        data() {
            return {
                term: "",
                isSearchMode: false,

                currentPage: 1,

                sortByColumn: "customer",
                sortDesc: false,

                headers: [
                    { value: "index", title: "№" },
                    { value: "customer", title: "Заказчик", width: "20%" },
                    { value: "productName", title: "Изделие", width: "20%" },
                    { value: "orderedProductsCount", title: "Заказано (шт)" },
                    { value: "availableProductsCount", title: "В налиии (шт)" },
                    { value: "status", title: "Статус" },
                    { value: "actions", title: "" },
                ],

                isShowSuccessAlert: false,
                isShowErrorAlert: false,
                alertText: "",
            }
        },

        created() {
            this.$store.commit("setSearchParameters", this.term);

            this.$store.dispatch("loadOrders")
                .catch(() => {
                    this.showErrorAlert("Ошибка! Не удалось загрузить список заказов.");
                });
        },

        computed: {
            orders() {
                return this.$store.getters.orders;
            },

            itemsPerPage() {
                return this.$store.getters.pageSize;
            },

            pagesCount() {
                return Math.ceil(this.$store.getters.pageItemsCount / this.itemsPerPage);
            },

            isLoading() {
                return this.$store.getters.isLoading;
            }
        },

        methods: {
            createProductionOrder(order) {
                const parameters = {
                    orderId: order.id,
                    productId: order.productId,
                    productsCount: order.productsCount,
                    availableCount: order.availableCount
                }

                this.$store.dispatch("createProductionOrder", parameters)
                    .then(() => {
                        this.showSuccessAlert("Производственная задача успешно создана.");
                    })
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось создать производственную задачу.");
                    });
            },

            search() {
                if (this.term.length === 0) {
                    return;
                }

                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = true;

                this.$store.dispatch("loadOrders")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список заказов.");
                    });
            },

            cancelSearch() {
                if (!this.isSearchMode) {
                    return;
                }

                this.term = "";
                this.$store.commit("setSearchParameters", this.term);

                this.isSearchMode = false;

                this.$store.dispatch("loadOrders")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список заказов.");
                    });
            },

            sortBy(column) {
                if (this.sortByColumn === column) {
                    this.sortDesc = !this.sortDesc;
                } else {
                    this.sortDesc = false;
                    this.sortByColumn = column;
                }

                this.$store.commit("setSortingParameters", {
                    sortBy: this.sortByColumn,
                    isDesc: this.sortDesc
                });

                this.$store.dispatch("loadOrders")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список заказов.");
                    });
            },

            switchPage(nextPage) {
                this.$store.commit("setPageNumber", nextPage);

                this.$store.dispatch("loadOrders")
                    .catch(() => {
                        this.showErrorAlert("Ошибка! Не удалось загрузить список заказов.");
                    });
            },

            getColor(status) {
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

            isStatusPending(status) {
                if (status === "Pending") {
                    return true;
                }

                return false;
            },

            showSuccessAlert(text) {
                this.alertText = text;
                this.isShowSuccessAlert = true;
            },

            showErrorAlert(text) {
                this.alertText = text;
                this.isShowErrorAlert = true;
            }
        }
    }
</script>